using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(Equipment), typeof(Inventory), typeof(UnitStats))]
public class Entity : MonoBehaviour, IDamageable, IEffectable {

	[Tooltip("The movement controller")]
	public CharController m_controller;

	[HideInInspector] public CollisionRelay m_collisionRelay;

	[Tooltip("If this entity can die")]
	public bool m_canDie;
	[HideInInspector] public bool m_isDead;

	[Tooltip("The type associated to this entity, it will take damage as such")]
	public DamageType m_type;

	[Tooltip("This entity's equipment")]
	public Equipment m_equipment;

	[Tooltip("The entity's viewing variables, only set if this entity uses AI, otherwise leave it empty")]
	public Look m_look;

	[Tooltip("The inventory carried by this entity")]
	public Inventory m_inventory;

	[Tooltip("Whether or not this entity stays as a corpse in-game, necessary to drop any loot")]
	public bool m_hasCorpse;

	[Tooltip("The item table dropped by the entity on death")]
	[ConditionalField("m_hasCorpse", "true")] public DropRuntimeSet m_lootTable;

	[Tooltip("Should the entity's inventory be dropped on death?")]
	[ConditionalField("m_hasCorpse", "true")] public bool m_dropInventoryOnDeath;

	[Tooltip("Event called when interacting with the corpse")]
	[ConditionalField("m_hasCorpse", "true")] public GameEvent m_interactCorpseEvent;

	[Tooltip("All runtime sets this entity is a part of")]
	public List<EntityRuntimeSet> m_runtimeSets;

	[Tooltip("This entity's feedback template")]
	public GameObject m_feedbackTemplate;

	[Tooltip("How random the feedback's position should be on both axis")]
	public Vector2 m_feedbackPositionRandomness;

	// TODO: check for duplicate effects maybe not stacking? unsure if it will happen but needs to be tested
	[HideInInspector] public Dictionary<Effect, float> m_effectsActive; // float = application time
	[HideInInspector] public UnitHealth m_health;
	[HideInInspector] public UnitStats m_stats;
	[HideInInspector] public Shooter m_shooter;
	public List<Ability> m_temp;
	[HideInInspector] public List<AbilityWrapper> m_abilities;
	public List<SkillWrapper> m_skills;
	[HideInInspector] public Modifiers m_modifiers;
	[HideInInspector] public StateController m_ai;
	[HideInInspector] public NPC m_npc;
	[HideInInspector] public Color m_feedbackColor; // transparent = green/red

	public virtual void Start() {
		m_effectsActive = new Dictionary<Effect, float>();
		m_health = GetComponent<UnitHealth>();
		m_stats = GetComponent<UnitStats>();
		m_shooter = GetComponent<Shooter>();
		m_abilities = new List<AbilityWrapper>();
		//m_skills = new List<SkillWrapper>();
		m_modifiers = gameObject.AddComponent<Modifiers>();
		m_feedbackColor = Constants.YELLOW;

		if(m_stats) m_stats.Init(this);
		if(m_shooter) m_shooter.Init(this);
		if(m_health) m_health.Init(this);
		if(m_inventory) m_inventory.Init(this);
		if(m_equipment) m_equipment.Init(this);

		if(m_temp.Count > 0) // very temporary, just for testing
			for(int i = 0; i < m_temp.Count; i++) {
				AbilityWrapper wrapper = new AbilityWrapper();

				wrapper.Ability = m_temp[i];
				wrapper.Learned = true;
				wrapper.TrainingLevel = 1;
				wrapper.HotkeySlot = i + 1;

				m_abilities.Add(wrapper);
			}

		// TODO: load abilities, skills and modifiers?

		foreach(SkillWrapper wrapper in m_skills)
			if(wrapper.Skill.m_isPassive)
                wrapper.Skill.Use(this, wrapper.TrainingLevel);

		InvokeRepeating("TickEffects", Constants.EFFECT_TICK_RATE, Constants.EFFECT_TICK_RATE);
		InvokeRepeating("UpdateCharacterSpeed", Constants.CHARACTER_SPEED_UPDATE_RATE, Constants.CHARACTER_SPEED_UPDATE_RATE);
	}

	void OnEnable() { 
		foreach(EntityRuntimeSet set in m_runtimeSets)
			set.Add(this);
	}

	void OnDisable() {
		foreach(EntityRuntimeSet set in m_runtimeSets)
			set.Remove(this);
	}

	private void UpdateCharacterSpeed() { 
		m_controller.m_speed = m_stats.GetStatEffectFloat(Stats.SPD);
	}

	public void ApplyEffect(Effect p_effect) {
		if(!p_effect.TriggerCheck()) return;

		// effects are ticked in the repeating tick loop FIRST to sync it up with everything else
		// otherwise you could have a 1ms delay between 2 effect ticks
		m_effectsActive.Add(p_effect, Time.time * 1000);
	}

	public void ApplyEffects(List<Effect> p_effects) {
		foreach(Effect effect in p_effects)
			ApplyEffect(effect);
	}

	private void TickEffects() {
		foreach(Effect effect in new List<Effect>(m_effectsActive.Keys)) {
			effect.Tick(this);

			bool remove = false;

			if(effect.m_duration > 0) {
				float activationTime = 0f;

				m_effectsActive.TryGetValue(effect, out activationTime);

				if(Time.time * 1000 > activationTime + effect.m_duration * 1000)
					remove = true;
			} else remove = true;

			if(remove) m_effectsActive.Remove(effect);
		}
	}

	public void UseAbility(Ability p_ability) { 
		AbilityWrapper ability = m_abilities.Find(a => a.Ability == p_ability);

		if(ability == null) return;

		int cost = ability.Ability.m_manaCosts.Find(m => m.TrainingLevel == ability.TrainingLevel).Value;
		int leftoverMana = m_stats.GetStat(Stats.MP) - cost;

		if(leftoverMana >= 0 && ability.Use()) {
			m_stats.AddModifier(Stats.MP, -cost, 0);
			p_ability.Use(this, ability.TrainingLevel);

			if(ability.ChainedAbilities != null && ability.ChainedAbilities.Count > 0) { 
				foreach(Ability chain in ability.ChainedAbilities) {
					AbilityWrapper wrapper = m_abilities.Find(a => a.Ability == chain);

					if(wrapper == null) continue;

					cost = chain.m_manaCosts.Find(m => m.TrainingLevel == wrapper.TrainingLevel).Value;
					leftoverMana = m_stats.GetStat(Stats.MP) - cost;

					if(leftoverMana >= 0 && wrapper.Use()) {
						m_stats.AddModifier(Stats.MP, -cost, 0);
						chain.Use(this, wrapper.TrainingLevel);
					}
				}
			}
		}
	}

	public void OnDamage(Shooter p_damager, DamageType p_type, int p_damage, bool p_bypassDefense, bool p_bypassImmunityWindow) {
		if(m_health) {
			int finalDamage = p_damage;
			int effective = p_type.IsEffectiveAgainst(m_type);

			if(!p_bypassDefense) finalDamage -= m_stats.GetStatEffect(Stats.DEF);
			if(effective == 1) finalDamage = Mathf.CeilToInt(finalDamage * 1.5f);
			else if(effective == -1) finalDamage /= 2;

			int resistance = Mathf.FloorToInt(m_modifiers.GetModifier(p_type.m_name + " Resistance"));
			finalDamage -= resistance;

			if(m_feedbackTemplate && (!m_health.IsImmune() || p_bypassImmunityWindow))
				FeedbackGenerator.GenerateFeedback(transform, m_feedbackTemplate, p_type, finalDamage <= 0 ? 0 : -finalDamage, 
												   m_feedbackColor, p_bypassDefense ? Constants.PURPLE : Constants.TRANSPARENT,
												   m_feedbackPositionRandomness.x, m_feedbackPositionRandomness.y);

			if(finalDamage > 0) m_health.Damage(finalDamage, p_bypassImmunityWindow);
		}

		// make sure the AI starts targeting its last damager (if it's an entity)
		if(m_ai && p_damager && p_damager.m_entity) m_ai.m_target = p_damager.m_entity;
	}

	public void OnDeath() {
		if(m_canDie && !m_isDead) {
			m_isDead = true;
			Die();
		}
	}

	protected virtual void Die() {
		if(m_npc) m_npc.Die();
		
		Destroy(m_ai);
		Destroy(m_shooter);

		if(m_hasCorpse) {
			Corpse corpse = gameObject.AddComponent<Corpse>();
			corpse.Init(this, Constants.CORPSE_LIFETIME);
		}

		// fire sound effects

		m_controller.Stop();
		Destroy(m_controller);
		Destroy(m_health);
		Destroy(this);
		Destroy(m_stats);
	}
}