using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class HideUIOnMouseover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public static List<GameObject> ObjectsHiddenOnMouseover = new List<GameObject>();

	[Tooltip("The alpha value to use for Image components")]
	[Range(0, 255)] public float m_imageAlpha;

	[Tooltip("The alpha value to use for Text components")]
	[Range(0, 255)] public float m_textAlpha;

	[Tooltip("The alpha value to use for RawImage components")]
	[Range(0, 255)] public float m_rawImageAlpha;

	private List<Component> m_componentsToHide;

	void Start() {
		m_componentsToHide = new List<Component>();

		m_componentsToHide.Add(GetComponent<Image>());
		m_componentsToHide.Add(GetComponent<Text>());
		m_componentsToHide.Add(GetComponent<RawImage>());

		if(transform.childCount > 0) {
			m_componentsToHide.AddRange(GetComponentsInChildren<Image>(true));
			m_componentsToHide.AddRange(GetComponentsInChildren<Text>(true));
			m_componentsToHide.AddRange(GetComponentsInChildren<RawImage>(true));
		}

		m_componentsToHide.RemoveAll(c => c == null);

		foreach(Component component in m_componentsToHide)
			if(!ObjectsHiddenOnMouseover.Contains(component.gameObject))
				ObjectsHiddenOnMouseover.Add(component.gameObject);
	}

	public void OnPointerEnter(PointerEventData p_eventData) {
		foreach(Component component in m_componentsToHide)
			SetAlphaOnComponent(component, false);
	}

	public void OnPointerExit(PointerEventData p_eventData) {
		foreach(Component component in m_componentsToHide)
			SetAlphaOnComponent(component, true);
	}

	private void SetAlphaOnComponent(Component p_component, bool p_white) {
		if(p_component is Image) {
			Color color = ((Image) p_component).color;
			((Image) p_component).color = new Color(color.r, color.g, color.b, (p_white ? 255 : m_imageAlpha) / 255f);
		} else if(p_component is Text) {
			Color color = ((Text) p_component).color;
			((Text) p_component).color = new Color(color.r, color.g, color.b, (p_white ? 255 : m_textAlpha) / 255f);
		} else if(p_component is RawImage) {
			Color color = ((RawImage) p_component).color;
			((RawImage) p_component).color = new Color(color.r, color.g, color.b, (p_white ? 255 : m_rawImageAlpha) / 255f);
		}
	}
}
