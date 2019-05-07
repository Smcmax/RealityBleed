using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HealthInfo : MonoBehaviour {

	[Tooltip("The time after taking damage where this is immune to damage")]
	[Range(0, 2)] public float m_immunityWindow;
	private float m_lastHit;

	[Tooltip("This is more for inheritance purposes, if you're using HealthInfo this does nothing (leave it checked anyway)")]
	public bool m_useLocalHealth;

	[Tooltip("Maximum health")]
	[ConditionalField("m_useLocalHealth", "true")] public int m_maxHealth = 0;

	[Tooltip("Current health")]
	[ConditionalField("m_useLocalHealth", "true")] public int m_health = 0;

	[Tooltip("Health bar template")]
	public GameObject m_healthBarTemplate;

	[Tooltip("The offset to give the health bar")]
	public Vector3 m_healthBarOffset;

	[Tooltip("Event called when this takes damage")]
	public UnityEvent m_damageEvent;

	[Tooltip("Event called when this dies")]
	public UnityEvent m_deathEvent;

	[HideInInspector] public IDamageable m_damageable;
	private Slider m_healthBarSlider;

	public void Init(IDamageable p_damageable) {
		m_health = m_maxHealth;
		m_damageable = p_damageable;

		if(m_healthBarTemplate) LoadHealthBar();
	}

	void OnDisable() { 
		if(m_healthBarSlider) Destroy(m_healthBarSlider.gameObject);
	}

	void Update() {
		UpdateHealthBar();
	}

	public virtual int GetHealth() {
		return m_health;
	}

	public virtual int GetMaxHealth() { 
		return m_maxHealth;
	}

	public virtual void SetHealth(int p_value) {
		int value = p_value;

		if(value > GetMaxHealth()) value = GetMaxHealth();
		else if(value < 0) value = 0;

		m_health = value;
	}

	public void UpdateHealthBar() {
		if(m_healthBarSlider) {
			m_healthBarSlider.maxValue = GetMaxHealth();
			m_healthBarSlider.value = GetHealth();

			if(GetHealth() == GetMaxHealth() && m_healthBarSlider.gameObject.activeSelf)
				m_healthBarSlider.gameObject.SetActive(false);
			else if(GetHealth() < GetMaxHealth() && !m_healthBarSlider.gameObject.activeSelf) 
				m_healthBarSlider.gameObject.SetActive(true);
		}
	}

	public bool IsImmune() {
		return m_immunityWindow > 0 ? Time.time * 1000 < m_lastHit + m_immunityWindow * 1000 : false;
	}

	public bool Damage(int p_amount, bool p_bypassImmunityWindow) {
		if(!p_bypassImmunityWindow && IsImmune()) return false;

		SetHealth(GetHealth() - p_amount);
		m_lastHit = Time.time * 1000;
		m_damageEvent.Invoke();

		if(GetHealth() <= 0) {
			m_damageable.OnDeath();
			m_deathEvent.Invoke();
		}

		return true;
	}

	private void LoadHealthBar() {
		GameObject healthBar = Instantiate(m_healthBarTemplate, m_healthBarTemplate.transform.parent);
		UIWorldSpaceFollower follow = healthBar.GetComponent<UIWorldSpaceFollower>();
		m_healthBarSlider = healthBar.GetComponent<Slider>();

		follow.m_offset = m_healthBarOffset;
		follow.m_parent = transform;

		healthBar.SetActive(true);

		UpdateHealthBar();
	}
}
