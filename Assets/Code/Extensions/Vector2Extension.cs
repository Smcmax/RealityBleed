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

	public static Vector2 Center(this Vector2 p_first, Vector2 p_second){ 
		Vector2 diff = new Vector2(p_first.x - p_second.x, p_first.y - p_second.y);
		Vector2 lowest = p_first.x <= p_second.x && p_first.y <= p_second.y ? p_first : p_second;

		return new Vector2(lowest.x + diff.x / 2, lowest.y + diff.y / 2);
	}

	public static bool IsRectangle(this Vector2 p_a, Vector2 p_b, Vector2 p_c, Vector2 p_d) {
		return IsRectangleOrdered(p_a, p_b, p_c, p_d) ||
					IsRectangleOrdered(p_b, p_c, p_a, p_d) ||
					IsRectangleOrdered(p_c, p_a, p_b, p_d);
	}

	private static bool IsOrthogonal(Vector2 p_a, Vector2 p_b, Vector2 p_c) {
		return (p_b.x - p_a.x) * (p_b.x - p_c.x) + (p_b.y - p_a.y) * (p_b.y - p_c.y) == 0;
	}

	private static bool IsRectangleOrdered(Vector2 p_a, Vector2 p_b, Vector2 p_c, Vector2 p_d) {
		return IsOrthogonal(p_a, p_b, p_c) && 
					IsOrthogonal(p_b, p_c, p_d) && 
					IsOrthogonal(p_c, p_d, p_a);
	}
}