using UnityEngine;

[CreateAssetMenu(menuName = "Shot Patterns/Circle")]
public class CirclePattern : ShotPattern {

	[Header("Specific Attributes")]
	[Tooltip("The angle at which the shot starts starting where the entity is looking")]
	[Range(0, 360)] public float m_angleOffset;

	public override void Init(Shooter p_shooter) {
		p_shooter.SetPatternInfo(this, "angle", m_angleOffset + 360f / m_shots);
	}

	public override void Step(Shooter p_shooter) {
		Projectile proj = SpawnProjectile(p_shooter);
		Vector2 target = FetchTarget(p_shooter, proj);
		Vector2 direction = (target - (Vector2) proj.transform.position).normalized;
		float angle = (float) p_shooter.GetPatternInfo(this, "angle");

		direction = direction.Rotate(angle);
		proj.Shoot(p_shooter, target, direction);

		if(m_shots > 1) angle += 360f / m_shots;
		if(angle >= 360 + m_angleOffset) angle = m_angleOffset;

		p_shooter.SetPatternInfo(this, "angle", angle);
	}
}
