using UnityEngine;

public class ExplodeOnDeathBehaviour : ProjectileBehaviour {

	[Tooltip("The pattern that executes itself when the projectile dies")]
	public ShotPattern m_explosionPattern;

	[Tooltip("Chance for this behaviour to trigger on death")]
	[Range(0, 100)] public float m_chanceToExplode;

	public override void Move(Projectile p_projectile, DataHolder p_data) {}

	public override void Die(Projectile p_projectile) {
		if(Random.Range(0f, 100f) <= m_chanceToExplode && !p_projectile.m_shooter.m_entity.m_isDead) {
			p_projectile.m_shooter.SetPatternInfo(m_explosionPattern, "spawnLocation", p_projectile.transform.position);
			p_projectile.m_shooter.Shoot(m_explosionPattern);
		}
	}
}