using UnityEngine;

public class PlayerController : MonoBehaviour {

	public CharController m_controller;

	void FixedUpdate() {
		Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal") * Time.fixedDeltaTime, 
										  Input.GetAxisRaw("Vertical") * Time.fixedDeltaTime);

		m_controller.Move(move);
	}
}
