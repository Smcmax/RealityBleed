using UnityEngine;
using UnityEngine.Events;

public class UnitHealth : MonoBehaviour {

	[Tooltip("The time after taking damage where the unit is immune to damage.")]
	[Range(0, 2)] public float m_immunityWindow;
	private float m_lastHit;

	[Tooltip("This entity's health bar template")]
	public GameObject m_healthBarTemplate;

	[Tooltip("Event called when the entity takes damage.")]
	public UnityEvent m_damageEvent;

	[Tooltip("Event called when the entity dies.")]
	public UnityEvent m_deathEvent;

	[HideInInspector] public Entity m_entity;
	private StatSliderUpdater m_healthBarUpdater;

	public void Init(Entity p_entity) {
		m_entity = p_entity;

		if(m_healthBarTemplate) LoadHealthBar();
	}

	void OnDisable() { 
		if(m_healthBarUpdater) Destroy(m_healthBarUpdater.gameObject);
	}

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
		if(m_healthBarUpdater) m_healthBarUpdater.UpdateSlider();

		if(GetHealth() <= 0) {
			GetComponent<Entity>().Kill();
			m_deathEvent.Invoke();
		}
	}

	private void LoadHealthBar() {
		GameObject healthBar = Instantiate(m_healthBarTemplate, m_healthBarTemplate.transform.parent);
		UIWorldSpaceFollower follow = healthBar.GetComponent<UIWorldSpaceFollower>();
		m_healthBarUpdater = healthBar.GetComponent<StatSliderUpdater>();

		m_healthBarUpdater.m_entity = m_entity;
		follow.m_parent = transform;

		healthBar.SetActive(true);
	}
}
