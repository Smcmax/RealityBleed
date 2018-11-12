using UnityEngine;

public class ExplodeOnDeathBehaviour : ProjectileBehaviour {

	[Tooltip("The pattern that executes itself when the projectile dies")]
	public ShotPattern m_explosionPattern;

	[Tooltip("Chance for this behaviour to trigger on death")]
	[Range(0, 100)] public float m_chanceToExplode;

	public override void Move(Projectile p_projectile, BehaviourData p_data) {}

	public override void Die(Projectile p_projectile) {
		if(Random.Range(0f, 100f) <= m_chanceToExplode) {
			m_explosionPattern.StartPattern(p_projectile.m_shooter, p_projectile.transform.position);
		}
	}
}