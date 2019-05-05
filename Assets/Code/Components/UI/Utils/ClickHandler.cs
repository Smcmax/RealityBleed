using Rewired.Integration.UnityUI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour, IPointerClickHandler {

	public void OnPointerClick(PointerEventData p_eventData) {
		if(p_eventData is PlayerPointerEventData) {
			PlayerPointerEventData playerEventData = (PlayerPointerEventData) p_eventData;
			GameObject clicked = playerEventData.pointerPress;
			Player player = Player.GetPlayerFromId(playerEventData.playerId);

			if(playerEventData.button == PointerEventData.InputButton.Left) {
				if(playerEventData.clickCount == 1) OnLeftSingleClick(clicked, player);
				else if(playerEventData.clickCount >= 2) OnLeftDoubleClick(clicked, player);

				OnAnyClick(clicked, player);
			} else if(playerEventData.button == PointerEventData.InputButton.Right) {
				if(playerEventData.clickCount == 1) OnRightSingleClick(clicked, player);
				else if(p_eventData.clickCount >= 2) OnRightDoubleClick(clicked, player);

				OnAnyClick(clicked, player);
			} else if(p_eventData.button == PointerEventData.InputButton.Middle) {
				if(p_eventData.clickCount == 1) OnMiddleSingleClick(clicked, player);
				else if(p_eventData.clickCount >= 2) OnMiddleDoubleClick(clicked, player);

				OnAnyClick(clicked, player);
			}
		}
	}

	protected virtual void OnLeftSingleClick(GameObject p_clicked, Player p_clicker) { }
	protected virtual void OnLeftDoubleClick(GameObject p_clicked, Player p_clicker) { }
	protected virtual void OnRightSingleClick(GameObject p_clicked, Player p_clicker) { }
	protected virtual void OnRightDoubleClick(GameObject p_clicked, Player p_clicker) { }
	protected virtual void OnMiddleSingleClick(GameObject p_clicked, Player p_clicker) { }
	protected virtual void OnMiddleDoubleClick(GameObject p_clicked, Player p_clicker) { }
	protected virtual void OnAnyClick(GameObject p_clicked, Player p_clicker) { }
}