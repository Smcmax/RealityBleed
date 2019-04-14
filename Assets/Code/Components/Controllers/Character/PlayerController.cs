using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour {

	public CharController m_controller;

	[HideInInspector] public Player m_player;

	void FixedUpdate() {
		float xMove = m_player.m_rewiredPlayer.GetAxisRaw("MoveX");
		float yMove = m_player.m_rewiredPlayer.GetAxisRaw("MoveY");

		if(m_player.m_mouse.m_currentMode == CursorModes.CURSOR) {
			xMove = 0;
			yMove = 0;
		}
		
		if(float.IsNaN(xMove)) xMove = 0;
		if(float.IsNaN(yMove)) yMove = 0;

		Vector2 move = new Vector2(xMove * Time.deltaTime, yMove * Time.deltaTime);

		m_controller.Move(move);
	}
}
