using UnityEngine;

[System.Serializable]
public class CircleSectionPattern : ShotPattern {

	[Tooltip("The angle range of the circle section to shoot into")]
	public float m_angleRange;

	[Tooltip("The angle at which the shot starts starting where the entity is looking")]
	public float m_angleOffset;

	[Tooltip("Whether or not the range should center itself on the entity's direction")]
	public bool m_centeredOnDirection;

	private float m_angleStart;
	private float m_angle;
	
	public override void Init(Shooter p_shooter) {
		float angleStart = m_centeredOnDirection ? m_angleOffset - m_angleRange / 2 : m_angleOffset;
		
		m_angleStart = angleStart;
		m_angle = angleStart;
	}

	public override void Step(Shooter p_shooter) {
		Projectile proj = SpawnProjectile(p_shooter);
		Vector2 target = FetchTarget(p_shooter, proj);
		Vector2 direction = (target - (Vector2) proj.transform.position).normalized;

		direction = direction.Rotate(m_angle);
		proj.Shoot(p_shooter, target, direction);

		if(m_shots > 1) m_angle += m_angleRange / (m_shots - 1);
		if(m_angle > m_angleStart + m_angleRange) m_angle = m_angleStart;
	}

	public override ShotPattern Clone() {
		CircleSectionPattern pattern = (CircleSectionPattern) base.Clone();

		pattern.m_angleRange = m_angleRange;
		pattern.m_angleOffset = m_angleOffset;
		pattern.m_centeredOnDirection = m_centeredOnDirection;

		m_angleStart = 0;
		m_angle = 0;

		return pattern;
	}
}
