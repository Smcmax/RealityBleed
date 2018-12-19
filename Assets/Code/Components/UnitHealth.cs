using UnityEngine;
using UnityEngine.Events;

public class UnitHealth : MonoBehaviour {

	[Tooltip("The time after taking damage where the unit is immune to damage.")]
	[Range(0, 2)] public float m_immunityWindow;
	private float m_lastHit;

	[Tooltip("Event called when the entity takes damage.")]
	public UnityEvent m_damageEvent;

	[Tooltip("Event called when the entity dies.")]
	public UnityEvent m_deathEvent;

	[HideInInspector] public Entity m_entity;

	public int GetHealth() {
		return m_entity.m_stats.GetStat(Stats.HP);
	}

	public int GetMaxHealth() { 
		return m_entity.m_stats.GetBaseStatWithGear(Stats.HP);
	}

	private void SetHealth(int p_value) {
		int value = p_value;

		if(value > GetMaxHealth()) value = GetMaxHealth();
		else if(value < 0) value = 0;

		m_entity.m_stats.AddModifier(Stats.HP, p_value - GetHealth(), 0);
	}

	public bool IsImmune() {
		return Time.time * 1000 < m_lastHit + m_immunityWindow * 1000;
	}

	public void Damage(int p_amount, bool p_bypassImmunityWindow) {
		if(!p_bypassImmunityWindow && IsImmune()) return;

		m_entity.m_stats.AddModifier(Stats.HP, -p_amount, 0);
		m_lastHit = Time.time * 1000;
		m_damageEvent.Invoke();

		if(GetHealth() <= 0) {
			GetComponent<Entity>().Kill();
			m_deathEvent.Invoke();
		}
	}
}
