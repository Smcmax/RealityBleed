using UnityEngine;

public class PlayerController : MonoBehaviour {

	public CharController m_controller;

	[HideInInspector] public Player m_player;

	void FixedUpdate() {
		Vector2 move = new Vector2(m_player.m_rewiredPlayer.GetAxisRaw("MoveX") * Time.fixedDeltaTime,
								   m_player.m_rewiredPlayer.GetAxisRaw("MoveY") * Time.fixedDeltaTime);

		m_controller.Move(move);
	}
}
