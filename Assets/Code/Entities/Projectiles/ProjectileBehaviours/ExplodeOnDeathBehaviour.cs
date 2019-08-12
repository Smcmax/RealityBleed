using UnityEngine;

[System.Serializable]
public class ExplodeOnDeathBehaviour : ProjectileBehaviour {

	[Tooltip("The pattern that executes itself when the projectile dies")]
	public string m_explosionPattern;

	[Tooltip("Chance for this behaviour to trigger on death")]
	public float m_chanceToExplode;

	public override void Move(Projectile p_projectile) {}

	public override void Die(Projectile p_projectile) {
		if(Random.Range(0f, 100f) <= m_chanceToExplode && !p_projectile.m_shooter.m_entity.m_isDead) {
			ShotPattern pattern = ShotPattern.Get(m_explosionPattern, false);

			pattern.m_spawnLocation = p_projectile.transform.position;
			p_projectile.m_shooter.Shoot(pattern);
		}
	}

	public override ProjectileBehaviour Clone() {
		ExplodeOnDeathBehaviour behaviour = (ExplodeOnDeathBehaviour) base.Clone();

		behaviour.m_explosionPattern = m_explosionPattern;
		behaviour.m_chanceToExplode = m_chanceToExplode;

		return behaviour;
	}
}