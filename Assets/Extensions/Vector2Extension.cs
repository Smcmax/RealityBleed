using UnityEngine;

public static class Vector2Extension {

	public static Vector2 Rotate(this Vector2 p_vector, float p_degrees) {
		float radians = p_degrees * Mathf.Deg2Rad;
		float sin = Mathf.Sin(radians);
		float cos = Mathf.Cos(radians);

		float tx = p_vector.x;
		float ty = p_vector.y;

		return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
	}
}