using UnityEngine;

[System.Serializable]
public class CirclePattern : ShotPattern {

	[Tooltip("The angle at which the shot starts starting where the entity is looking")]
	public float m_angleOffset;

	private float m_angle;

	public override void Init(Shooter p_shooter) {
		m_angle = m_angleOffset + 360f / m_shots;
	}

	public override void Step(Shooter p_shooter) {
		Projectile proj = SpawnProjectile(p_shooter);
		Vector2 target = FetchTarget(p_shooter, proj);
		Vector2 direction = (target - (Vector2) proj.transform.position).normalized;

		direction = direction.Rotate(m_angle);
		proj.Shoot(p_shooter, target, direction, m_impactSound);

		if(m_shots > 1) m_angle += 360f / m_shots;
		if(m_angle >= 360 + m_angleOffset) m_angle = m_angleOffset;
	}

	public override ShotPattern Clone() {
		CirclePattern pattern = (CirclePattern) base.Clone();

		pattern.m_angleOffset = m_angleOffset;
		pattern.m_angle = 0;

		return pattern;
	}
}
