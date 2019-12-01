using UnityEngine;

public abstract class CharController : MonoBehaviour {

	// The reason these two are separate is because of the possibility of facing north-east, south-east... etc.
	[HideInInspector] public bool m_directionX; // Whether or not the character is facing right
	[HideInInspector] public bool m_directionY; // Whether or not the character is facing up

	[Tooltip("The smoothing delay added to movement")]
	[Range(0, 1f)] [SerializeField] protected float m_smoothTime = 0.3f;

	[Tooltip("The speed the character moves at")]
	[Range(0, 10f)] [SerializeField] public float m_speed = 1f;

	[Tooltip("Percentage of max speed the character can move at in a given direction while moving in two directions")]
	[Range(0, 1f)] public float m_diagonalSpeedPercentage = 0.75f;

    [HideInInspector] public Vector3 m_velocity;
    [HideInInspector] public Rigidbody2D m_rigidbody2D;

	void Awake() {
		m_rigidbody2D = GetComponent<Rigidbody2D>();
		m_directionX = true;
		m_directionY = true;

		OnAwake();
	}

	protected abstract void OnAwake();

	public abstract void Move(Vector2 p_move);

	public void Stop() {
		m_rigidbody2D.velocity = Vector2.zero;
	}
}
