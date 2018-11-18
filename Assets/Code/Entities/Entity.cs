using UnityEngine;
using System.Collections.Generic;

public class Entity : MonoBehaviour {

	[Tooltip("The movement controller")]
	public CharController m_controller;

	[Tooltip("If this entity can die")]
	public bool m_canDie;
	protected bool m_isDead;

	[Tooltip("This entity's equipment")]
	public Equipment m_equipment;

	[Tooltip("The entity's viewing variables, only set if this entity uses AI, otherwise leave it empty")]
	public Look m_look;

	[Tooltip("Every inventory carried by this entity")]
	public List<Inventory> m_inventories; // TODO: do something better for extra bags and shit? maybe just a simple wrapper for it

	[Tooltip("All runtime sets this entity is a part of")]
	public List<EntityRuntimeSet> m_runtimeSets;

	// check for duplicate effects maybe not stacking? unsure if it will happen but needs to be tested
	[HideInInspector] public Dictionary<Effect, float> m_effectsActive; // float = application time
	[HideInInspector] public Class m_class;
	[HideInInspector] public UnitHealth m_health;
	[HideInInspector] public UnitStats m_stats;
	[HideInInspector] public Shooter m_shooter;
	[HideInInspector] public StateController m_ai;

	void Awake() {
		m_effectsActive = new Dictionary<Effect, float>();
		m_health = GetComponent<UnitHealth>();
		m_stats = GetComponent<UnitStats>();
		m_shooter = GetComponent<Shooter>();

		if(m_shooter) m_shooter.Init(this);
		if(m_equipment) m_equipment.SetEntity(this);

		InvokeRepeating("TickEffects", Constants.EFFECT_TICK_RATE, Constants.EFFECT_TICK_RATE);
	}

	void OnEnable() { 
		foreach(EntityRuntimeSet set in m_runtimeSets)
			set.Add(this);
	}

	void OnDisable() {
		foreach (EntityRuntimeSet set in m_runtimeSets)
			set.Remove(this);
	}

	public bool AddToInventory(Item p_item){ 
		foreach(Inventory inventory in m_inventories)
			if(inventory.Add(p_item)) return true;
		
		return false;
	}

	public bool RemoveFromInventory(Item p_item){
		foreach(Inventory inventory in m_inventories)
			if(inventory.Remove(p_item)) return true;

		return false;
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

	public void Damage(Entity p_entity, float p_damage, bool p_bypassDefense, bool p_bypassImmunityWindow){
		if(m_health) {
			float finalDamage = p_damage;

			if(!p_bypassDefense) {
				float defense = m_stats ? m_stats.m_defense : 0f;

				finalDamage -= defense;
			}

			if(finalDamage > 0) m_health.Damage(finalDamage, p_bypassImmunityWindow);
		}

		if(m_ai && p_entity) m_ai.m_target = p_entity;
	}

	public void Kill() {
		if (m_canDie) {
			m_isDead = true;
			Die();
		}
	}

	protected virtual void Die() {
		Destroy(gameObject);
	}
}
