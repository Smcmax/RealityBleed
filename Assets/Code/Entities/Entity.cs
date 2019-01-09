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

	[Tooltip("This entity's equipment")]
	public Equipment m_equipment;

	[Tooltip("The entity's viewing variables, only set if this entity uses AI, otherwise leave it empty")]
	public Look m_look;

	[Tooltip("The inventory carried by this entity")]
	public Inventory m_inventory;

	[Tooltip("All runtime sets this entity is a part of")]
	public List<EntityRuntimeSet> m_runtimeSets;

	[Tooltip("This entity's feedback canvas")]
	public Canvas m_feedbackCanvas;

	[Tooltip("This entity's feedback template")]
	public GameObject m_feedbackTemplate;

	[Tooltip("How random the feedback's position should be on both axis")]
	public Vector2 m_feedbackPositionRandomness;

	// check for duplicate effects maybe not stacking? unsure if it will happen but needs to be tested
	[HideInInspector] public Dictionary<Effect, float> m_effectsActive; // float = application time
	[HideInInspector] public UnitHealth m_health;
	[HideInInspector] public UnitStats m_stats;
	[HideInInspector] public Shooter m_shooter;
	[HideInInspector] public StateController m_ai;
	[HideInInspector] public Color m_feedbackColor; // transparent = green/red

	public virtual void Awake() {
		m_effectsActive = new Dictionary<Effect, float>();
		m_health = GetComponent<UnitHealth>();
		m_stats = GetComponent<UnitStats>();
		m_shooter = GetComponent<Shooter>();
		m_feedbackColor = Constants.YELLOW;

		if(m_shooter) m_shooter.Init(this);
		if(m_health) m_health.m_entity = this;
		if(m_inventory) m_inventory.m_entity = this;
		if(m_equipment) m_equipment.Init(this);

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

	public void Damage(Entity p_entity, int p_damage, bool p_bypassDefense, bool p_bypassImmunityWindow){
		if(m_health) {
			int finalDamage = p_damage;

			if(!p_bypassDefense) finalDamage -= m_stats.GetStatEffect(Stats.DEF);
			if(finalDamage > 0) {
				GenerateFeedback(-finalDamage, p_bypassDefense ? Constants.PURPLE : Constants.TRANSPARENT);
				m_health.Damage(finalDamage, p_bypassImmunityWindow);
			}
		}

		// make sure the AI starts targeting its last damager
		if(m_ai && p_entity) m_ai.m_target = p_entity;
	}

	// display color is transparent if no specified color
	public void GenerateFeedback(int p_amount, Color p_displayColor) {
		GameObject feedback = Instantiate(m_feedbackTemplate, m_feedbackCanvas.transform);
		Text feedbackText = feedback.GetComponent<Text>();
		UIWorldSpaceFollower follow = feedback.GetComponent<UIWorldSpaceFollower>();
		Color feedbackColor = m_feedbackColor;

		if(p_displayColor == Constants.TRANSPARENT) { // transparent means nothing specified
			if(m_feedbackColor == Constants.TRANSPARENT)
				feedbackColor = p_amount > 0 ? Constants.GREEN : Constants.RED;
		} else feedbackColor = p_displayColor;

		feedbackText.text = p_amount > 0 ? p_amount.ToString() : Mathf.Abs(p_amount).ToString();
		feedbackText.color = feedbackColor;
		follow.m_offset += new Vector3(Random.Range(-m_feedbackPositionRandomness.x / 2, m_feedbackPositionRandomness.x / 2), 
										      Random.Range(-m_feedbackPositionRandomness.y / 2, m_feedbackPositionRandomness.y / 2));

		feedback.SetActive(true);
	}

	public void Kill() {
		if (m_canDie) {
			m_isDead = true;
			
			Die();
		}
	}

	// TODO: not destroy
	protected virtual void Die() {
		Destroy(gameObject);
	}
}
