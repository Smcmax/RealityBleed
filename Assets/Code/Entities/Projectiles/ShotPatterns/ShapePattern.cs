using UnityEngine;

[System.Serializable]
public class ShapePattern : ShotPattern {

	[Tooltip("When parallel, will shift both ways simultaneously")]
	public bool m_symmetrical;

	[Tooltip("The shifting angle for the parallel shots, 0 is going towards target (0-360)")]
	public float m_shiftAngle;

	[Tooltip("How much the shift angle changes shot by shot, enables some cool shapes")]
	public float m_shiftAngleIncrement;

	[Tooltip("The distance that each individual shot shifts over by")]
	public float m_shiftDistance;

	[Tooltip("How far from the entity the shot starts, this helps curb shapes starting from far behind the entity")]
	public float m_startDistance;

	private Vector2 m_shift;
	private float m_localShiftAngle;
	private bool m_shiftReverse;
	private bool m_shiftOverHalf;
	
	public override void Init(Shooter p_shooter) {
		m_shift = Vector2.zero;
		m_shiftReverse = false;
		m_localShiftAngle = m_shiftAngle;

		if(m_shots % 2 == 0) {
			m_shiftOverHalf = true;
			m_shiftReverse = true;
		} else m_shiftOverHalf = false;
	}

	public override void Step(Shooter p_shooter) {
		Projectile proj = SpawnProjectile(p_shooter);
		Vector2 target = FetchTarget(p_shooter, proj);
		Vector2 direction = (target - (Vector2) proj.transform.position).normalized;

		proj.transform.position += (Vector3) (m_startDistance * direction);

		if(m_shiftOverHalf) {
			m_shiftOverHalf = false;
			Shift(p_shooter, direction, m_shiftDistance / 2, false);
		}

		proj.transform.position += (Vector3) m_shift;
		proj.Shoot(p_shooter, target, direction);

		Shift(p_shooter, direction, m_shiftDistance, true);
	}

	private void Shift(Shooter p_shooter, Vector2 p_direction, float p_distance, bool p_reverse) {
		Vector2 rotation = p_direction.Rotate(m_localShiftAngle) * p_distance;

		if(p_reverse) {
			m_shift = -m_shift;

			if(m_symmetrical) m_shift = Vector2.Reflect(m_shift, p_direction);
			if(m_shiftReverse) rotation = Vector2.zero;

			m_shiftReverse = !m_shiftReverse;
		}

		m_localShiftAngle += m_shiftAngleIncrement;
		m_shift += rotation;
	}

	public override ShotPattern Clone() {
		ShapePattern pattern = (ShapePattern) base.Clone();

		pattern.m_symmetrical = m_symmetrical;
		pattern.m_shiftAngle = m_shiftAngle;
		pattern.m_shiftAngleIncrement = m_shiftAngleIncrement;
		pattern.m_shiftDistance = m_shiftDistance;
		pattern.m_startDistance = m_startDistance;

		m_shift = Vector2.zero;
		m_localShiftAngle = 0;
		m_shiftReverse = false;
		m_shiftOverHalf = false;

		return pattern;
	}
}
