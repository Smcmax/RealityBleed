using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnClickButtonTransitioner : MonoBehaviour {

	[Tooltip("The button to click when transitioning")]
	public Button m_button;

    [Tooltip("Event fired upon transition")]
    public GameEvent m_eventToFire;

    private bool m_selected;
	private TabMenu m_tabMenu;

	void OnEnable() {
		m_selected = false;
		m_tabMenu = GetComponentInParent<TabMenu>();
	}

	void Update() {
		if(EventSystem.current.currentSelectedGameObject == gameObject && !m_selected) Select(true);
		else if(EventSystem.current.currentSelectedGameObject != gameObject && m_selected) 
            m_selected = false;
	}

	public void Select(bool p_fireEvent) {
		m_selected = true;

        if(m_tabMenu != null && m_tabMenu.m_currentTab != gameObject.name) Transition(p_fireEvent);
	}

	private void Transition(bool p_fireEvent) { 
		m_tabMenu.m_currentTab = gameObject.name;

        if(m_eventToFire && p_fireEvent) m_eventToFire.Raise();

        m_button.onClick.Invoke();
	}
}
