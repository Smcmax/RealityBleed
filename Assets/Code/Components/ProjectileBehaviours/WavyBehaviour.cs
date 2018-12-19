using UnityEngine;

public class WavyBehaviour : ProjectileBehaviour {

	[Tooltip("The range to move back and forth in")]
	[Range(0, 5)] public float m_range;

	[Tooltip("How granular and fast the wave is, how fine it is, lower value means more choppiness but also a faster speed")]
	[Range(1, 250)] public int m_steps;

	private void LoadData(DataHolder p_data, out float p_distance, out bool p_reverse) {
		p_distance = p_data.Has("m_distance") ? (float) p_data.Get("m_distance") : m_range / 2;
		p_reverse = p_data.Has("m_reverse") ? (bool) p_data.Get("m_reverse") : false;
	}

	private void SaveData(DataHolder p_data, float p_distance, bool p_reverse) {
		p_data.Set("m_distance", p_distance);
		p_data.Set("m_reverse", p_reverse);
	}

	public override void Move(Projectile p_projectile, DataHolder p_data) {
		float distance = 0f;
		bool reverse = false;
		Vector3 moveVector = p_projectile.m_direction * p_projectile.m_speed * Time.fixedDeltaTime;
		Vector2 perpendicular = Vector2.Perpendicular(moveVector) * p_projectile.m_speed * Time.fixedDeltaTime;
		float perpX = Mathf.Abs(perpendicular.x);
		float perpY = Mathf.Abs(perpendicular.y);
		float stepDistance = m_range / m_steps;
		float totalPerpendicularMovement = perpX + perpY;
		float targetX = stepDistance * (perpX / totalPerpendicularMovement);
		float targetY = stepDistance * (perpY / totalPerpendicularMovement);

		if(perpX < 0) targetX *= -1;
		if(perpY < 0) targetY *= -1;

		LoadData(p_data, out distance, out reverse);

		if(reverse) {
			targetX = -targetX;
			targetY = -targetY;
		}

		Vector2 sideMovement = new Vector2(targetX, targetY);
		float sideDist = Vector2.Distance(moveVector, sideMovement);

		distance += reverse ? -sideDist : sideDist;
		moveVector += (Vector3) sideMovement;
		p_projectile.transform.position += moveVector;

		if(distance >= m_range) reverse = true;
		else if(distance <= 0) reverse = false;

		SaveData(p_data, distance, reverse);
	}

	public override void Die(Projectile p_projectile) { }
}