using UnityEngine;

[CreateAssetMenu(menuName = "Shot Patterns/Wave")]
public class WavePattern : ShotPattern {

	[Header("Specific Attributes")]
	[Tooltip("The angle range of the circle section to shoot into")]
	[Range(0, 360)] public float m_angleRange;

	[Tooltip("The angle at which the shot starts starting where the entity is looking")]
	[Range(0, 360)] public float m_angleOffset;

	[Tooltip("Whether or not the range should center itself on the entity's direction")]
	public bool m_centeredOnDirection;
	
	public override void Init(Shooter p_shooter) {
		float angleStart = m_centeredOnDirection ? m_angleOffset - m_angleRange / 2 : m_angleOffset;
		
		p_shooter.SetPatternInfo(this, "angleStart", angleStart);
		p_shooter.SetPatternInfo(this, "angle", angleStart);
		p_shooter.SetPatternInfo(this, "reverse", false);
	}

	public override void Step(Shooter p_shooter) {
		Projectile proj = SpawnProjectile(p_shooter);
		Vector2 target = FetchTarget(p_shooter, proj);
		Vector2 direction = (target - (Vector2) proj.transform.position).normalized;
		float angle = (float) p_shooter.GetPatternInfo(this, "angle");
		bool reverse = (bool) p_shooter.GetPatternInfo(this, "reverse");
		float angleStart = (float) p_shooter.GetPatternInfo(this, "angleStart");

		direction = direction.Rotate(angle);
		proj.Shoot(p_shooter, target, direction);

		if (m_shots > 1) {
			float angleStep = m_angleRange / (m_shots / 2);
			angle += reverse ? -angleStep : angleStep;
		}

		if (angle >= angleStart + m_angleRange) reverse = true;
		if (angle <= angleStart) reverse = false;

		p_shooter.SetPatternInfo(this, "angle", angle);
		p_shooter.SetPatternInfo(this, "reverse", reverse);
	}
}
