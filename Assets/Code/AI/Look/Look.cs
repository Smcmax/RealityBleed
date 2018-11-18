using UnityEngine;

[CreateAssetMenu(menuName = "AI/Look")]
public class Look : ScriptableObject {
	[Tooltip("Entity's viewing distance")]
	[Range(0, 15)] public float m_lookRange;

	[Tooltip("Entity's detection area's radius")]
	[Range(0, 15)] public float m_lookSphereRadius;

	[Tooltip("Entity's viewing angles, how wide its vision is in degrees")]
	[Range(0, 360)] public float m_fieldOfView;

	[Tooltip("Entity's target scanning speed, how fast it rotates")]
	[Range(0, 10)] public float m_scanSpeed;

	[Tooltip("Length of time (in seconds) before the entity stops scanning and switches states")]
	[Range(0, 30)] public float m_scanDuration;

	public static void LookAt(Entity p_entity, Vector2 p_direction) {
		float angle = Mathf.Atan2(p_direction.y, p_direction.x) * Mathf.Rad2Deg;
		p_entity.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
