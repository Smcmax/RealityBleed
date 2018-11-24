using UnityEngine;

public class FireShotPatternBehaviour : ProjectileBehaviour {

	[Tooltip("The pattern that executes itself when the projectile fires")]
	public ShotPattern m_shotPattern;

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