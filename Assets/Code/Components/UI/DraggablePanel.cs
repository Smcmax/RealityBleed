using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IDragHandler {

	public Canvas m_canvas;

	public void OnDrag(PointerEventData p_eventData) {
		transform.position += (Vector3) p_eventData.delta;
	}

	public void BringToFront() { 
		m_canvas.sortingOrder = 1;
	}

	public void SendToBack() { 
		m_canvas.sortingOrder = 0;
	}
}
