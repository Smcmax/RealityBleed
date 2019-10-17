using UnityEngine;

public class Destructible : MonoBehaviour, IDamageable {

	[Tooltip("The type associated to this object, it will take damage as such")]
	public DamageType m_type;

	[Tooltip("The defense stat applied to incoming damage (hardness)")]
	public int m_defense;

	[Tooltip("This object's feedback template")]
	public GameObject m_feedbackTemplate;

	[Tooltip("How random the feedback's position should be on both axis")]
	public Vector2 m_feedbackPositionRandomness;

	[HideInInspector] public HealthInfo m_health;

	public virtual void Start() { 
		m_health = GetComponent<HealthInfo>();
		m_health.Init(this);
	}

	public void OnDamage(Shooter p_damager, DamageType p_type, int p_damage, bool p_bypassDefense, bool p_bypassImmunityWindow) {
		int finalDamage = p_damage;
		int effective = p_type.IsEffectiveAgainst(m_type);

		if(!p_bypassDefense) finalDamage -= m_defense;
		if(effective == 1) finalDamage = Mathf.CeilToInt(finalDamage * 1.5f);
		else if(effective == -1) finalDamage /= 2;

		if(m_feedbackTemplate && (!m_health.IsImmune() || p_bypassImmunityWindow))
			FeedbackGenerator.GenerateFeedback(transform, m_feedbackTemplate, p_type, finalDamage <= 0 ? 0 : -finalDamage, 
											   Constants.YELLOW, p_bypassDefense ? Constants.PURPLE : Constants.TRANSPARENT,
											   m_feedbackPositionRandomness.x, m_feedbackPositionRandomness.y);

		if(finalDamage > 0) m_health.Damage(finalDamage, p_bypassImmunityWindow);
	}

	public virtual void OnDeath() {
		// fire sound effects

		Destroy(gameObject);
	}
}
