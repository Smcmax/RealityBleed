using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public Canvas m_canvas;
	public static bool HOLDING = false;
	private bool m_currentlyHolding = false;
	private RectTransform m_rect;

	void Awake() { 
		m_rect = GetComponent<RectTransform>();
	}

	public void OnBeginDrag(PointerEventData p_eventData) {
		HOLDING = true;
		m_currentlyHolding = true;
		m_canvas.sortingOrder = 2;
	}

	public void OnDrag(PointerEventData p_eventData) {
		Vector3 newLoc = transform.position + (Vector3) p_eventData.delta;

		if(newLoc.x - m_rect.sizeDelta.x < 0) newLoc.x = m_rect.sizeDelta.x;
		if(newLoc.x + m_rect.sizeDelta.x > Screen.width) newLoc.x = Screen.width - m_rect.sizeDelta.x;
		if(newLoc.y - m_rect.sizeDelta.y < 0) newLoc.y = m_rect.sizeDelta.y;
		if(newLoc.y + m_rect.sizeDelta.y > Screen.height) newLoc.y = Screen.height - m_rect.sizeDelta.y;

		transform.position = newLoc;
	}

	public void OnEndDrag(PointerEventData p_eventData) {
		m_canvas.sortingOrder = 1;
		HOLDING = false;
		m_currentlyHolding = false;
	}

	public void BringToFront() { 
		if(HOLDING && !m_currentlyHolding) { SendToBack(); return; }

		m_canvas.sortingOrder = 1;
	}

	public void SendToBack() { 
		if(HOLDING && m_currentlyHolding) return;

		m_canvas.sortingOrder = 0;
	}
}
