using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Damage")]
public class DamageEffect : Effect {

	[Tooltip("Damage inflicted to the target entity every time this effect ticks")]
	[Range(0, 100)] public int m_damage;

	[Tooltip("The type of damage inflicted to the entity every time this effects ticks")]
	public DamageType m_type;

	public override void Tick(IEffectable p_target) { 
		if(p_target is IDamageable)
			((IDamageable) p_target).OnDamage(null, m_type, m_damage, true, true);
	}
}
