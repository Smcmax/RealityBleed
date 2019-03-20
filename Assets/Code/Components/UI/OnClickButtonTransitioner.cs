using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnClickButtonTransitioner : MonoBehaviour {

	[Tooltip("The button to click when transitioning")]
	public Button m_button;

	private bool m_selected;

	void OnEnable() {
		m_selected = false;
	}

	void Update() {
		if(EventSystem.current.currentSelectedGameObject == gameObject && !m_selected) {
			m_selected = true;
			Transition();
		} else m_selected = false;
	}

	private void Transition() { 
		m_button.onClick.Invoke();
	}
}
