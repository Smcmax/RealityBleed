using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public Canvas m_canvas;
	public static bool HOLDING = false;
	private bool m_currentlyHolding = false;

	public void OnBeginDrag(PointerEventData p_eventData) {
		HOLDING = true;
		m_currentlyHolding = true;
		m_canvas.sortingOrder = 2;
	}

	public void OnDrag(PointerEventData p_eventData) {
		transform.position = transform.position + (Vector3) p_eventData.delta;
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
