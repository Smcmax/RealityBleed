using UnityEngine;

[CreateAssetMenu(menuName = "Shot Patterns/Shape")]
public class ShapePattern : ShotPattern {

	[Header("Specific Attributes")]
	[Tooltip("When parallel, will shift both ways simultaneously")]
	public bool m_symmetrical;

	[Tooltip("The shifting angle for the parallel shots, 0 is going towards target (0-360)")]
	public float m_shiftAngle;

	[Tooltip("How much the shift angle changes shot by shot, enables some cool shapes")]
	public float m_shiftAngleIncrement;

	[Tooltip("The distance that each individual shot shifts over by")]
	public float m_shiftDistance;

	[Tooltip("How far from the entity the shot starts, this helps curb shapes starting from far behind the entity")]
	[Range(-2.5f, 2.5f)] public float m_startDistance;
	
	public override void Init(Shooter p_shooter) {
		p_shooter.SetPatternInfo(this, "shift", Vector2.zero);
		p_shooter.SetPatternInfo(this, "shiftReverse", false);
		p_shooter.SetPatternInfo(this, "localShiftAngle", m_shiftAngle);

		if(m_shots % 2 == 0) {
			p_shooter.SetPatternInfo(this, "shiftOverHalf", true);
			p_shooter.SetPatternInfo(this, "shiftReverse", true);
		} else p_shooter.SetPatternInfo(this, "shiftOverHalf", false);
	}

	public override void Step(Shooter p_shooter) {
		Projectile proj = SpawnProjectile(p_shooter);
		Vector2 target = FetchTarget(p_shooter, proj);
		Vector2 direction = (target - (Vector2) proj.transform.position).normalized;

		proj.transform.position += (Vector3) (m_startDistance * direction);

		if((bool) p_shooter.GetPatternInfo(this, "shiftOverHalf")) {
			p_shooter.SetPatternInfo(this, "shiftOverHalf", false);
			Shift(p_shooter, direction, m_shiftDistance / 2, false);
		}

		Vector2 shift = (Vector2) p_shooter.GetPatternInfo(this, "shift");

		proj.transform.position += (Vector3) shift;
		proj.Shoot(p_shooter, target, direction);

		Shift(p_shooter, direction, m_shiftDistance, true);
	}

	private void Shift(Shooter p_shooter, Vector2 p_direction, float p_distance, bool p_reverse) {
		float localShiftAngle = (float) p_shooter.GetPatternInfo(this, "localShiftAngle");
		Vector2 rotation = p_direction.Rotate(localShiftAngle) * p_distance;
		Vector2 shift = (Vector2) p_shooter.GetPatternInfo(this, "shift");
		bool shiftReverse = (bool) p_shooter.GetPatternInfo(this, "shiftReverse");

		if(p_reverse) {
			shift = -shift;

			if(m_symmetrical) shift = Vector2.Reflect(shift, p_direction);
			if(shiftReverse) rotation = Vector2.zero;

			p_shooter.SetPatternInfo(this, "shiftReverse", !shiftReverse);
		}

		p_shooter.SetPatternInfo(this, "localShiftAngle", localShiftAngle + m_shiftAngleIncrement);
		p_shooter.SetPatternInfo(this, "shift", shift + rotation);
	}
}
