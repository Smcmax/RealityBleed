using UnityEngine;
using System.Collections.Generic;

public class PolygonColliderExtruder : Extruder {

	[Tooltip("The collider of the sprite to extrude, if not using any, use the sprite's physics shape")]
	public PolygonCollider2D m_collider;

	[Tooltip("If not using a collider, should we reverse the path order to look better?")]
	public bool m_reverse;

	public override void Extrude() {
		if(!CanExtrude()) return;

		if(!m_collider) {
			m_collider = gameObject.AddComponent<PolygonCollider2D>();

			List<Vector2> f = new List<Vector2>();
			Vector2 prev = new Vector2(0, 0);
			float x1 = 0;
			float x2 = 0;
			float y1 = 0;
			float y2 = 0;
			bool xCommonPrev = true;
			int set = 0;

			// converting sprite's physics shape into a singular cohesive path
			for(int i = 0; i < m_collider.GetTotalPointCount(); i++) {
				if(i % 4 == 0 && i != 0) {
					bool add = false;
					if(prev != Vector2.zero) add = true;

                    prev = GetPoint(x1, x2, y1, y2, prev);
					xCommonPrev = y2 == 0;
					if(add) f.Add(prev);

					set++;
					x1 = 0;
					x2 = 0;
					y1 = 0;
					y2 = 0;
				}

				Vector2 vertex = m_collider.GetPath(set)[i % 4];

				if(x1 == 0) x1 = vertex.x;
				else if(vertex.x != x1 && x2 == 0) x2 = vertex.x;

				if(y1 == 0) y1 = vertex.y;
				else if(vertex.y != y1 && y2 == 0) y2 = vertex.y;
			}

            prev = GetPoint(x1, x2, y1, y2, prev);
            f.Add(prev);
			f.Add(new Vector2(xCommonPrev ? f[0].x : prev.x, xCommonPrev ? prev.y : f[0].y));
			if(m_reverse) f.Reverse();

			m_collider.pathCount = 1;
            m_collider.SetPath(0, f.ToArray());
			m_collider.enabled = false;
		}

		GameObject extruded = Create3DMeshObject(m_collider.points, transform, gameObject.name + "Extrusion");
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
