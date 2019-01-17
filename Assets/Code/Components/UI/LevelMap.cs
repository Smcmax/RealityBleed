using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelMap : MonoBehaviour, IDragHandler, IScrollHandler {
	
	[Tooltip("Color used for the map's background")]
	public Color m_mapBackgroundColor;

	[Tooltip("The camera used to render the map")]
	public Camera m_mapCamera;

	[Tooltip("Inverts the drag's movement")]
	public bool m_invertDrag;

	void Awake() { 
		m_mapCamera.backgroundColor = m_mapBackgroundColor;

		Vector2 size = GetComponent<RectTransform>().sizeDelta;
		float scale = GetComponentInParent<Canvas>().scaleFactor;
		RenderTexture newRender = new RenderTexture((int) (size.x * scale), (int) (size.y * scale), 24);
		newRender.filterMode = FilterMode.Point;

		m_mapCamera.targetTexture = newRender;
		GetComponent<RawImage>().texture = newRender;
	}

	public void OnDrag(PointerEventData p_eventData) {
		Vector3 delta = p_eventData.delta / 300 * m_mapCamera.orthographicSize;
		if(m_invertDrag) delta = new Vector2(-delta.x, -delta.y);

		m_mapCamera.transform.position += delta;
	}

	public void OnScroll(PointerEventData p_eventData) {
		float size = m_mapCamera.orthographicSize - p_eventData.scrollDelta.y;

		if(size < 1) m_mapCamera.orthographicSize = 1;
		else if(size > 35) m_mapCamera.orthographicSize = 35;
		else m_mapCamera.orthographicSize = size;
	}
}
