using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class HideUIOnEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public static List<GameObject> ObjectsHidden = new List<GameObject>();

	[Tooltip("The alpha value to use for Image components")]
	[Range(0, 255)] public float m_imageAlpha;

	[Tooltip("The alpha value to use for Text components")]
	[Range(0, 255)] public float m_textAlpha;

	[Tooltip("The alpha value to use for RawImage components")]
	[Range(0, 255)] public float m_rawImageAlpha;

	[Tooltip("Should the player be able to click through the object?")]
	public bool m_allowClickThrough;

	[Tooltip("Should this UI hide whenever the pointer enters it?")]
	public bool m_hideOnPointerEntry;

	[Tooltip("If the transform is set, this UI will hide whenever the transform starts moving")]
	public Transform m_hideOnTargetMovement;
	private Vector3 m_lastTargetPosition;
	private float m_lastMoveTime;
	private float m_hideDelayAfterMove = 0.1f;

	private bool m_hidden;
	private List<Component> m_componentsToHide;

	void Start() {
		m_componentsToHide = new List<Component>();
		if(m_hideOnTargetMovement) m_lastTargetPosition = m_hideOnTargetMovement.transform.position;

		m_componentsToHide.Add(GetComponent<Image>());
		m_componentsToHide.Add(GetComponent<TextMeshProUGUI>());
		m_componentsToHide.Add(GetComponent<RawImage>());

		if(transform.childCount > 0) {
			m_componentsToHide.AddRange(GetComponentsInChildren<Image>(true));
			m_componentsToHide.AddRange(GetComponentsInChildren<TextMeshProUGUI>(true));
			m_componentsToHide.AddRange(GetComponentsInChildren<RawImage>(true));
		}

		m_componentsToHide.RemoveAll(c => c == null);

		if(m_allowClickThrough)
			foreach(Component component in m_componentsToHide)
				if(!ObjectsHidden.Contains(component.gameObject))
					ObjectsHidden.Add(component.gameObject);
	}

	public void OnPointerEnter(PointerEventData p_eventData) {
		if(!m_hideOnPointerEntry && m_hidden) return;

		Hide();
	}

	public void OnPointerExit(PointerEventData p_eventData) {
		if(!m_hideOnPointerEntry && !m_hidden) return;

		Show();
	}

	void Update() { 
		if(!m_hideOnTargetMovement) return;

		bool moved = Vector2.Distance(m_hideOnTargetMovement.transform.position, m_lastTargetPosition) > 0.05;

		if(moved) m_lastMoveTime = Time.time * 1000;
		m_lastTargetPosition = m_hideOnTargetMovement.transform.position;
		moved = moved ? true : Time.time * 1000 < m_lastMoveTime + m_hideDelayAfterMove * 1000;

		if(!m_hidden && moved) Hide();
		else if(m_hidden && !moved) Show();
	}

	private void Hide() {
		m_hidden = true;

		foreach(Component component in m_componentsToHide)
			SetAlphaOnComponent(component, false);
	}

	private void Show() {
		m_hidden = false;

		foreach(Component component in m_componentsToHide)
			SetAlphaOnComponent(component, true);
	}

	private void SetAlphaOnComponent(Component p_component, bool p_white) {
		if(p_component is Image) {
			Color color = ((Image) p_component).color;
			((Image) p_component).color = new Color(color.r, color.g, color.b, (p_white ? 255 : m_imageAlpha) / 255f);
		} else if(p_component is TextMeshProUGUI) {
			Color color = ((TextMeshProUGUI) p_component).color;
			((TextMeshProUGUI) p_component).color = new Color(color.r, color.g, color.b, (p_white ? 255 : m_textAlpha) / 255f);
		} else if(p_component is RawImage) {
			Color color = ((RawImage) p_component).color;
			((RawImage) p_component).color = new Color(color.r, color.g, color.b, (p_white ? 255 : m_rawImageAlpha) / 255f);
		}
	}
}
