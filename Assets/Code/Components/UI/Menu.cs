using UnityEngine;

public class Menu : MonoBehaviour {

	[Tooltip("The menu to go back to from the current menu, null if none")]
	public Menu m_previousMenu;

	[Tooltip("Can this menu be closed?")]
	public bool m_closeable;

	private Language m_lastUpdatedLanguage;

	void Awake() {
		m_lastUpdatedLanguage = Game.m_languages.m_languages.Find(l => l.m_name == "English");
	}

	void OnEnable() {
		UpdateMenuLanguage();
	}

	public void UpdateMenuLanguage() {
		if(m_lastUpdatedLanguage == Game.m_languages.GetCurrentLanguage()) return;

        Game.m_languages.UpdateMenuLanguage(this, m_lastUpdatedLanguage);
        m_lastUpdatedLanguage = Game.m_languages.GetCurrentLanguage();
	}

	public void SetPrevious(Menu p_menu) { 
		m_previousMenu = p_menu;
	}

	public void CloseControlMapper() { // needed cause otherwise you don't get a handler...
		MenuHandler.Instance.CloseControlMapper();
	}
}
