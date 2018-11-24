using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IBeginDragHandler, IDragHandler {

	public Canvas m_canvas;

	public void OnBeginDrag(PointerEventData p_eventData) {
		m_canvas.sortingOrder = 1;
	}

	public void OnDrag(PointerEventData p_eventData) {
		transform.position += (Vector3) p_eventData.delta;

		foreach(GameObject hovered in p_eventData.hovered)
			if(hovered.name.Contains("Canvas")) {
				Canvas hover = hovered.GetComponent<Canvas>();
				
				if(hovered != m_canvas) hover.sortingOrder = 0;

				break;
			}
	}
}
