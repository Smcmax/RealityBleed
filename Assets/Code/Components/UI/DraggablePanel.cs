using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public Canvas m_canvas;
	public static bool HOLDING = false;
	private bool m_currentlyHolding = false;

	private RectTransform m_canvasRect;
	private RectTransform m_rect;

	void Start() { 
		m_canvasRect = m_canvas.GetComponent<RectTransform>();
		m_rect = GetComponent<RectTransform>();
	}

	public void OnBeginDrag(PointerEventData p_eventData) {
		HOLDING = true;
		m_currentlyHolding = true;
		m_canvas.sortingOrder = 2;
	}

	public void OnDrag(PointerEventData p_eventData) {
		Vector3 newPosition = transform.position + (Vector3) p_eventData.delta;

		Vector3[] canvasCorners = new Vector3[4];
		m_canvasRect.GetWorldCorners(canvasCorners);

		float clampedX = Mathf.Clamp(newPosition.x, canvasCorners[0].x + m_rect.sizeDelta.x * m_canvas.scaleFactor / 2, 
														   canvasCorners[2].x - m_rect.sizeDelta.x * m_canvas.scaleFactor / 2);
		float clampedY = Mathf.Clamp(newPosition.y, canvasCorners[0].y + m_rect.sizeDelta.y * m_canvas.scaleFactor / 2, 
														   canvasCorners[2].y - m_rect.sizeDelta.y * m_canvas.scaleFactor / 2);

		Vector2 pointerPosition = new Vector2(clampedX, clampedY);
		Vector2 localPointerPosition;

		if(RectTransformUtility.ScreenPointToLocalPointInRectangle(m_canvasRect, pointerPosition, p_eventData.pressEventCamera, out localPointerPosition))
			m_rect.localPosition = localPointerPosition;
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
