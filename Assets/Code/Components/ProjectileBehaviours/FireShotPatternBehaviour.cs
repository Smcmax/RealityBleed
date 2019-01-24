using UnityEngine;

[CreateAssetMenu(menuName = "Projectile Behaviours/Fire Shot Pattern")]
public class FireShotPatternBehaviour : ProjectileBehaviour {

	[Tooltip("The pattern that executes itself after the delay has past")]
	public ShotPattern m_shotPattern;

	[Tooltip("Time in seconds before the shot pattern is fired")]
	[Range(0, 60)] public float m_timeBeforeFire;

	public override void Init(Projectile p_projectile, DataHolder p_data) {
		p_projectile.m_shooter.SetPatternInfo(m_shotPattern, "spawnLocation", p_projectile.transform.position);
		p_projectile.m_shooter.Shoot(m_shotPattern);
	}

	public override void Move(Projectile p_projectile, DataHolder p_data) {
		p_projectile.m_shooter.SetPatternInfo(m_shotPattern, "spawnLocation", p_projectile.transform.position);
	}

	public override void Die(Projectile p_projectile) {
		p_projectile.m_shooter.StopShooting(m_shotPattern);
	}
}