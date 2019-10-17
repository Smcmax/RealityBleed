using UnityEngine;

[System.Serializable]
public class FireShotPatternBehaviour : ProjectileBehaviour {

	[Tooltip("The pattern that executes itself after the delay has past")]
	public string m_shotPattern;

	[Tooltip("Delay (in seconds) before the pattern starts shooting")]
	public float m_delay;

	private ShotPattern m_pattern;
	private float m_initTime;
	private bool m_started;

	public override void Init(Projectile p_projectile) {
		m_pattern = ShotPattern.Get(m_shotPattern, false);
		m_started = false;
		m_initTime = Time.time;
	}

	public override void Move(Projectile p_projectile) {
		if(Time.time - m_initTime >= m_delay) {
			if(!m_started) StartShooting(p_projectile);
			else m_pattern.m_spawnLocation = p_projectile.transform.position;
		}
	}

	public override void Die(Projectile p_projectile) {
		if(m_started) p_projectile.m_shooter.StopShooting(m_pattern);
	}

	private void StartShooting(Projectile p_projectile) {
		m_pattern.m_spawnLocation = p_projectile.transform.position;
		p_projectile.m_shooter.Shoot(m_pattern);
	}

	public override ProjectileBehaviour Clone() {
		FireShotPatternBehaviour behaviour = (FireShotPatternBehaviour) base.Clone();

		behaviour.m_shotPattern = m_shotPattern;

		return behaviour;
	}
}