using UnityEngine;

public class BoxColliderExtruder : Extruder {

	[Tooltip("The collider of the sprite to extrude")]
	public BoxCollider2D m_collider;

	public override void Extrude(){
		if(!CanExtrude()) return;

		Vector2[] vertices = new Vector2[4];

		vertices[0] = m_collider.bounds.min;
		vertices[1] = new Vector2(m_collider.bounds.min.x, m_collider.bounds.max.y);
		vertices[2] = m_collider.bounds.max;
		vertices[3] = new Vector2(m_collider.bounds.max.x, m_collider.bounds.min.y);

		Create3DMeshObject(vertices, transform, gameObject.name + "Extrusion");
	}
}
