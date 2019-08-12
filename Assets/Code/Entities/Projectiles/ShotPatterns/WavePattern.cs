using UnityEngine;

[System.Serializable]
public class WavePattern : ShotPattern {

	[Tooltip("The angle range of the circle section to shoot into")]
	public float m_angleRange;

	[Tooltip("The angle at which the shot starts starting where the entity is looking")]
	public float m_angleOffset;

	[Tooltip("Whether or not the range should center itself on the entity's direction")]
	public bool m_centeredOnDirection;

	private float m_angleStart;
	private float m_angle;
	private bool m_reverse;
	
	public override void Init(Shooter p_shooter) {
		float angleStart = m_centeredOnDirection ? m_angleOffset - m_angleRange / 2 : m_angleOffset;
		
		m_angleStart = angleStart;
		m_angle = angleStart;
		m_reverse = false;
	}

	public override void Step(Shooter p_shooter) {
		Projectile proj = SpawnProjectile(p_shooter);
		Vector2 target = FetchTarget(p_shooter, proj);
		Vector2 direction = (target - (Vector2) proj.transform.position).normalized;

		direction = direction.Rotate(m_angle);
		proj.Shoot(p_shooter, target, direction);

		if(m_shots > 1) {
			float angleStep = m_angleRange / (m_shots / 2);
			m_angle += m_reverse ? -angleStep : angleStep;
		}

		if(m_angle >= m_angleStart + m_angleRange) m_reverse = true;
		if(m_angle <= m_angleStart) m_reverse = false;
	}
}
