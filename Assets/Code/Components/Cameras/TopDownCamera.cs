using UnityEngine;

public class TopDownCamera : MonoBehaviour {

	public Transform m_target; // The target followed by the camera
	[Range(0, 1f)] [SerializeField] protected float m_smoothTime = 0.3f; // The smoothing delay added to the camera's movement
	private Vector3 m_velocity;

	void FixedUpdate() {
		Vector3 goal = m_target.position;

		goal.z = transform.position.z;
		transform.position = Vector3.SmoothDamp(transform.position, goal, ref m_velocity, m_smoothTime);
	}
}
