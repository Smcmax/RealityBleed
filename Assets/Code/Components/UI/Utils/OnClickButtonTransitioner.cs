using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnClickButtonTransitioner : MonoBehaviour {

	[Tooltip("The button to click when transitioning")]
	public Button m_button;

	private bool m_selected;
	private TabMenu m_tabMenu;

	void OnEnable() {
		m_selected = false;
		m_tabMenu = GetComponentInParent<TabMenu>();
	}

	void Update() {
		if(EventSystem.current.currentSelectedGameObject == gameObject && !m_selected) Select();
		else m_selected = false;
	}

	public void Select() {
		m_selected = true;
		Transition();
	}

	private void Transition() { 
		if(m_tabMenu != null) m_tabMenu.m_currentTab = gameObject.name;

		m_button.onClick.Invoke();
	}
}
