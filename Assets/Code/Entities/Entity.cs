using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Equipment), typeof(Inventory), typeof(UnitStats))]
public class Entity : MonoBehaviour {

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
	[ConditionalField("m_hasCorpse", "true")] public EntityDropRuntimeSet m_lootTable;

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
	public List<AbilityWrapper> m_abilities;
	public List<SkillWrapper> m_skills;
	[HideInInspector] public Modifiers m_modifiers;
	[HideInInspector] public StateController m_ai;
	[HideInInspector] public Color m_feedbackColor; // transparent = green/red

	public virtual void Awake() {
		m_effectsActive = new Dictionary<Effect, float>();
		m_health = GetComponent<UnitHealth>();
		m_stats = GetComponent<UnitStats>();
		m_shooter = GetComponent<Shooter>();
		//m_abilities = new List<AbilityWrapper>();
		//m_skills = new List<SkillWrapper>();
		m_modifiers = gameObject.AddComponent<Modifiers>();
		m_feedbackColor = Constants.YELLOW;

		if(m_shooter) m_shooter.Init(this);
		if(m_health) m_health.Init(this);
		if(m_inventory) m_inventory.m_entity = this;
		if(m_equipment) m_equipment.Init(this);

		// TODO: load abilities, skills and modifiers?

		foreach(AbilityWrapper ability in m_abilities)
			ability.SetOwner(this);

		foreach(SkillWrapper skill in m_skills)
			skill.SetOwner(this);

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

	public void ApplyEffects(List<Effect> p_effects) { 
		foreach(Effect effect in p_effects)
			ApplyEffect(effect);
	}

	public void ApplyEffect(Effect p_effect) {
		if(!p_effect.TriggerCheck()) return;

		// effects are ticked in the repeating tick loop FIRST to sync it up with everything else
		// otherwise you could have a 1ms delay between 2 effect ticks
		m_effectsActive.Add(p_effect, Time.time * 1000);
	}

	public void UseAbility(Ability p_ability) { 
		AbilityWrapper ability = m_abilities.Find(a => a.Ability == p_ability);

		int cost = ability.Ability.m_manaCosts.Find(m => m.TrainingLevel == ability.TrainingLevel).Value;
		int leftoverMana = m_stats.GetStat(Stats.MP) - cost;

		if(ability.Ability && leftoverMana >= 0 && ability.Use()) {
			m_stats.AddModifier(Stats.MP, -cost, 0);
			p_ability.Use(this, ability.TrainingLevel);
		}
	}

	public void Damage(Entity p_entity, DamageType p_type, int p_damage, bool p_bypassDefense, bool p_bypassImmunityWindow){
		if(m_health) {
			int finalDamage = p_damage;
			int effective = p_type.IsEffectiveAgainst(m_type);

			if(!p_bypassDefense) finalDamage -= m_stats.GetStatEffect(Stats.DEF);
			if(effective == 1) finalDamage = Mathf.CeilToInt(finalDamage * 1.5f);
			else if(effective == -1) finalDamage /= 2;

			int resistance = Mathf.FloorToInt(m_modifiers.GetModifier(p_type.m_name + " Resistance"));
			finalDamage -= resistance;

			if(finalDamage > 0) {
				if(m_feedbackTemplate) GenerateFeedback(p_type, -finalDamage, p_bypassDefense ? Constants.PURPLE : Constants.TRANSPARENT);
				m_health.Damage(finalDamage, p_bypassImmunityWindow);
			} else if(m_feedbackTemplate) GenerateFeedback(p_type, 0, p_bypassDefense ? Constants.PURPLE : Constants.TRANSPARENT);
		}

		// make sure the AI starts targeting its last damager
		if(m_ai && p_entity) m_ai.m_target = p_entity;
	}

	// display color is transparent if no specified color
	public void GenerateFeedback(DamageType p_type, int p_amount, Color p_displayColor) {
		GameObject feedback = Instantiate(m_feedbackTemplate, m_feedbackTemplate.transform.parent);
		Text feedbackText = feedback.GetComponent<Text>();
		Image damageTypeIcon = feedback.GetComponentInChildren<Image>();
		UIWorldSpaceFollower follow = feedback.GetComponent<UIWorldSpaceFollower>();
		Color feedbackColor = m_feedbackColor;

		if(p_displayColor == Constants.TRANSPARENT) { // transparent means nothing specified
			if(m_feedbackColor == Constants.TRANSPARENT)
				feedbackColor = p_amount > 0 ? Constants.GREEN : Constants.RED;
		} else feedbackColor = p_displayColor;

		if(p_type.m_icon) damageTypeIcon.sprite = p_type.m_icon;
		else damageTypeIcon.enabled = false;

		feedbackText.text = p_amount > 0 ? p_amount.ToString() : Mathf.Abs(p_amount).ToString();
		feedbackText.color = feedbackColor;
		follow.m_parent = transform;
		follow.m_offset += new Vector3(Random.Range(-m_feedbackPositionRandomness.x / 2, m_feedbackPositionRandomness.x / 2), 
										      Random.Range(-m_feedbackPositionRandomness.y / 2, m_feedbackPositionRandomness.y / 2));

		feedback.SetActive(true);
	}

	public void Kill() {
		if(m_canDie && !m_isDead) {
			m_isDead = true;
			Die();
		}
	}

	protected virtual void Die() {
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

