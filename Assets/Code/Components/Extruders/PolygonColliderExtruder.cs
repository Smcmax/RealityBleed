using UnityEngine;

public class PolygonColliderExtruder : Extruder {

	[Tooltip("The collider of the sprite to extrude")]
	public PolygonCollider2D m_collider;

	public override void Extrude() {
		if(!CanExtrude()) return;

		GameObject extruded = Create3DMeshObject(m_collider.points, transform, gameObject.name + "Extrusion");
		extruded.transform.position += transform.position;
		extruded.transform.localScale = new Vector3(1, 1, 1);
	}
}
