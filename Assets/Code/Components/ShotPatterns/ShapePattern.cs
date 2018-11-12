using UnityEngine;

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
	[Range(-2.5f, 2.5f)] public float m_startDistance;

	private Vector2 m_shift;
	private float m_localShiftAngle;
	private bool m_shiftReverse;
	private bool m_shiftOverHalf;
	
	public override void Init() {
		m_shift = Vector2.zero;
		m_shiftReverse = false;
		m_localShiftAngle = m_shiftAngle;

		if(m_shots % 2 == 0){
			m_shiftOverHalf = true;
			m_shiftReverse = true;
		}
	}

	public override void Step(){
		Projectile proj = SpawnProjectile();
		Vector2 target = FetchTarget(proj);
		Vector2 direction = (target - (Vector2) proj.transform.position).normalized;

		proj.transform.position += (Vector3) (m_startDistance * direction);

		if(m_shiftOverHalf) {
			m_shiftOverHalf = false;
			Shift(direction, m_shiftDistance / 2, false);
		}

		proj.transform.position += (Vector3) m_shift;
		proj.Shoot(m_shooter, target, direction);

		Shift(direction, m_shiftDistance, true);
	}

	private void Shift(Vector2 p_direction, float p_distance, bool p_reverse){ 
		Vector2 rotation = p_direction.Rotate(m_localShiftAngle) * p_distance;

		if(p_reverse){ 
			m_shift = -m_shift;

			if(m_symmetrical) m_shift = Vector2.Reflect(m_shift, p_direction);
			if(m_shiftReverse) rotation = Vector2.zero;

			m_shiftReverse = !m_shiftReverse;
		}

		m_localShiftAngle += m_shiftAngleIncrement;
		m_shift += rotation;
	}
}
