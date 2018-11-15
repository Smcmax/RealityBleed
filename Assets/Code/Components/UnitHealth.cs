using UnityEngine;
using UnityEngine.Events;

public class UnitHealth : MonoBehaviour {

	[Tooltip("The health mode that should be used")]
	public bool m_useSharedHealth;

	[Tooltip("Unit's health")]
	[ConditionalField("m_useSharedHealth", false)][Range(0, 1000)] public float m_health;

	[Tooltip("Unit's health")]
	[ConditionalField("m_useSharedHealth", true)] public FloatVariable m_refHealth;

	[Tooltip("Unit's max health")]
	public FloatReference m_maxHealth;

	[Tooltip("The time after taking damage where the unit is immune to damage.")]
	[Range(0, 2)] public float m_immunityWindow;
	private float m_lastHit;

	[Tooltip("Event called when the entity takes damage.")]
	public UnityEvent m_damageEvent;

	[Tooltip("Event called when the entity dies.")]
	public UnityEvent m_deathEvent;

	private float GetHealth() {
		return (m_refHealth) ? m_refHealth.Value : m_health;
	}

	private void SetHealth(float p_value) {
		float value = p_value;

		if(value > m_maxHealth.Value) value = m_maxHealth;
		else if(value < 0) value = 0;

		if(m_refHealth) m_refHealth.Value = value;
		else m_health = value;
	}

	public bool IsImmune() {
		return Time.time * 1000 < m_lastHit + m_immunityWindow * 1000;
	}

	public void Damage(float p_amount, bool p_bypassImmunityWindow) {
		if(!p_bypassImmunityWindow && IsImmune()) return;

		SetHealth(GetHealth() - p_amount);
		m_lastHit = Time.time * 1000;

		m_damageEvent.Invoke();

		if(GetHealth() <= 0.0f) {
			GetComponent<Entity>().Kill();
			m_deathEvent.Invoke();
		}
	}
}
