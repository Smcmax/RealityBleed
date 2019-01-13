using UnityEngine;

public class PlayerController : MonoBehaviour {

	public CharController m_controller;

	void FixedUpdate() {
		Vector2 move = new Vector2(SimpleInput.GetAxisRaw("MoveX") * Time.fixedDeltaTime,
										  SimpleInput.GetAxisRaw("MoveY") * Time.fixedDeltaTime);

		m_controller.Move(move);
	}
}
