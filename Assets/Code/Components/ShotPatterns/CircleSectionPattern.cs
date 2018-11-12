using UnityEngine;

public class CircleSectionPattern : ShotPattern {

	[Tooltip("The angle range of the circle section to shoot into")]
	[Range(0, 360)] public float m_angleRange;

	[Tooltip("The angle at which the shot starts starting where the entity is looking")]
	[Range(0, 360)] public float m_angleOffset;

	[Tooltip("Whether or not the range should center itself on the entity's direction")]
	public bool m_centeredOnDirection;

	private float m_angleStart;
	private float m_angle;
	
	public override void Init() {
		m_angleStart = m_centeredOnDirection ? m_angleOffset - m_angleRange / 2 : m_angleOffset;
		m_angle = m_angleStart;
	}

	public override void Step(){
		Projectile proj = SpawnProjectile();
		Vector2 target = FetchTarget(proj);
		Vector2 direction = (target - (Vector2) proj.transform.position).normalized;

		direction = direction.Rotate(m_angle);
		proj.Shoot(m_shooter, target, direction);

		if(m_shots > 1) m_angle += m_angleRange / (m_shots - 1);
		if(m_angle > m_angleStart + m_angleRange) m_angle = m_angleStart;
	}
}
