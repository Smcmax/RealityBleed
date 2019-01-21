using UnityEngine;
using UnityEngine.EventSystems;

public class ClickHandler : MonoBehaviour, IPointerClickHandler {

	public void OnPointerClick(PointerEventData p_eventData) {
		GameObject clicked = p_eventData.pointerPress;

		if(p_eventData.button == PointerEventData.InputButton.Left) {
			if(p_eventData.clickCount == 1) OnLeftSingleClick(clicked);
			else if(p_eventData.clickCount == 2) OnLeftDoubleClick(clicked);
		} else if(p_eventData.button == PointerEventData.InputButton.Right) {
			if(p_eventData.clickCount == 1) OnRightSingleClick(clicked);
			else if(p_eventData.clickCount == 2) OnRightDoubleClick(clicked);
			else if(p_eventData.clickCount > 2) OnRightDoubleClick(clicked);
		} else if(p_eventData.button == PointerEventData.InputButton.Middle) {
			if(p_eventData.clickCount == 1) OnMiddleSingleClick(clicked);
			else if(p_eventData.clickCount == 2) OnMiddleDoubleClick(clicked);
		}
	}

	protected virtual void OnLeftSingleClick(GameObject p_clicked) { }
	protected virtual void OnLeftDoubleClick(GameObject p_clicked) { }
	protected virtual void OnRightSingleClick(GameObject p_clicked) { }
	protected virtual void OnRightDoubleClick(GameObject p_clicked) { }
	protected virtual void OnMiddleSingleClick(GameObject p_clicked) { }
	protected virtual void OnMiddleDoubleClick(GameObject p_clicked) { }
}