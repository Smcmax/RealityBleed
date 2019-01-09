using UnityEngine;

public class TopDownCamera : MonoBehaviour {

	[Tooltip("The target being followed by this camera")]
	public Transform m_target;

	[Tooltip("The smoothing delay added to the camera's movement")]
	[Range(0, 1f)] [SerializeField] protected float m_smoothTime = 0.3f;

	[Tooltip("Rounding scale for the camera's location, 0 = rounding to int, 4 = rounding to 1/10000ths of an int. NOTE: bleeding occurs on 3 and above")]
	[Range(0, 4)] public int m_cameraLocationPrecision;
	private Vector3 m_velocity;

	void FixedUpdate() {
		Vector3 goal = m_target.position;

		goal.z = transform.position.z;
		transform.position = Vector3.SmoothDamp(transform.position, goal, ref m_velocity, m_smoothTime);

		float rounding = Mathf.Pow(10, m_cameraLocationPrecision);
		transform.position = new Vector3((Mathf.RoundToInt(transform.position.x * rounding) / rounding),
											   (Mathf.RoundToInt(transform.position.y * rounding) / rounding),
											   (Mathf.RoundToInt(transform.position.z)));     //used to keep camera bounded to integers
	}
}
