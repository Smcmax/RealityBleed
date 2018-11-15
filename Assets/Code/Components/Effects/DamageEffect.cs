using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Damage")]
public class DamageEffect : Effect {

	[Tooltip("Damage inflicted to the target entity every time this effect ticks")]
	[Range(0, 100)] public float m_damage;

	public override void Tick(Entity p_target){ 
		p_target.Damage(m_damage, true);
	}
}
