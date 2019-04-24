using UnityEngine;

public class UIWorldSpaceFollower : MonoBehaviour {
	
	[Tooltip("The object to follow")]
	public Transform m_parent;

	[Tooltip("The offset to add to the parent location (world space)")]
	public Vector3 m_offset;

	[Tooltip("If checked, the game object will destroy itself when the parent dies")]
	public bool m_destroyWithObject;

	[HideInInspector] public Vector3 m_freezePosition;

	void LateUpdate() {
		if(m_freezePosition != Vector3.zero)
			transform.position = Camera.main.WorldToScreenPoint(m_freezePosition + m_offset);
		else if(m_parent != null)
			transform.position = Camera.main.WorldToScreenPoint(m_parent.position + m_offset);
		else if(m_destroyWithObject) Destroy(gameObject);
	}
}
