using UnityEngine;

public class CirclePattern : ShotPattern {

	[Tooltip("The angle at which the shot starts starting where the entity is looking")]
	[Range(0, 360)] public float m_angleOffset;

	private float m_angle;

	public override void Init() {
		m_angle = m_angleOffset + 360f / m_shots;
	}

	public override void Step() {
		Projectile proj = SpawnProjectile();
		Vector2 target = FetchTarget(proj);
		Vector2 direction = (target - (Vector2) proj.transform.position).normalized;

		direction = direction.Rotate(m_angle);
		proj.Shoot(m_shooter, target, direction);

		if(m_shots > 1) m_angle += 360f / m_shots;
		if(m_angle >= 360 + m_angleOffset) m_angle = m_angleOffset;
	}
}
