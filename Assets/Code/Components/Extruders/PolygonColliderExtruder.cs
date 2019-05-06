using UnityEngine;
using System.Collections.Generic;

public class PolygonColliderExtruder : Extruder {

	[Tooltip("The collider of the sprite to extrude, if not using any, use the sprite's physics shape")]
	public PolygonCollider2D m_collider;

	[Tooltip("If not using a collider, should we reverse the path order to look better?")]
	public bool m_reverse;

	public override void Extrude() {
		if(!CanExtrude()) return;

		SpriteRenderer renderer = GetComponent<SpriteRenderer>();
		List<Vector2> points = new List<Vector2>();

		if(!m_collider && renderer) {
			Sprite sprite = renderer.sprite;
			List<Vector2> currentPhysicsPath = new List<Vector2>();
			Vector2 prev = new Vector2(0, 0);
			float x1 = 0;
			float x2 = 0;
			float y1 = 0;
			float y2 = 0;
			bool xCommonPrev = true;
			int set = 0;

			sprite.GetPhysicsShape(0, currentPhysicsPath);

			// converting sprite's physics shape into a singular cohesive path
			for(int i = 0; i < sprite.GetPhysicsShapeCount() * 4; i++) {
				if(i % 4 == 0 && i != 0) {
					bool add = false;
					if(prev != Vector2.zero) add = true;

                    prev = GetPoint(x1, x2, y1, y2, prev);
					xCommonPrev = y2 == 0;
					if(add) points.Add(prev);

					set++;
					x1 = 0;
					x2 = 0;
					y1 = 0;
					y2 = 0;

					currentPhysicsPath.Clear();
					sprite.GetPhysicsShape(set, currentPhysicsPath);
				}

				Vector2 vertex = currentPhysicsPath[i % 4];

				if(x1 == 0) x1 = vertex.x;
				else if(vertex.x != x1 && x2 == 0) x2 = vertex.x;

				if(y1 == 0) y1 = vertex.y;
				else if(vertex.y != y1 && y2 == 0) y2 = vertex.y;
			}

            prev = GetPoint(x1, x2, y1, y2, prev);
            points.Add(prev);
            points.Add(new Vector2(xCommonPrev ? points[0].x : prev.x, xCommonPrev ? prev.y : points[0].y));
			if(m_reverse) points.Reverse();
		} else if(!m_collider) return;
		else points.AddRange(m_collider.points);

		GameObject extruded = Create3DMeshObject(points.ToArray(), transform, gameObject.name + "Extrusion");
		extruded.transform.position += transform.position;
		extruded.transform.localScale = new Vector3(1, 1, 1);
	}

	private Vector2 GetPoint(float p_x1, float p_x2, float p_y1, float p_y2, Vector2 p_prev) {
        float common = p_x2 == 0 ? p_x1 : p_y1;
        float other1 = p_x2 == 0 ? p_y1 : p_x1;
        float other2 = p_x2 == 0 ? p_y2 : p_x2;
        float prevOtherValue = p_x2 == 0 ? p_prev.y : p_prev.x;
        float other = other1 == prevOtherValue ? other2 : other1;

        return new Vector2(p_x2 == 0 ? common : other, p_x2 == 0 ? other : common);
	}
}
