using UnityEngine;
using UnityEngine.SceneManagement;
using System;

#pragma warning disable 0162

public class Menu : MonoBehaviour {

	[Tooltip("The menu to go back to from the current menu, null if none")]
	public Menu m_previousMenu;

	[Tooltip("Can this menu be closed?")]
	public bool m_closeable;

    [Tooltip("Is this menu supposed to be the only player menu opened on screen?")]
    public bool m_singleOpenedMenu;

    [Tooltip("The name of the button required to open this menu")]
    public string m_menuButtonName;

    [Tooltip("Event fired upon menu open, if null, will use generic")]
    public GameEvent m_openEvent;

    [Tooltip("Event fired upon menu close, if null, will use generic")]
    public GameEvent m_closeEvent;

	private Language m_lastUpdatedLanguage;

	public void Awake() {
		#if UNITY_EDITOR
        	if(PreloadLoadNextScene.m_loaded)
				m_lastUpdatedLanguage = Game.m_languages.GetLanguage("English");

			return;
		#endif

        m_lastUpdatedLanguage = Game.m_languages.GetLanguage("English");
	}

	public void OnEnable() {
		#if UNITY_EDITOR
        	if(PreloadLoadNextScene.m_loaded) UpdateMenuLanguage();

			return;
		#endif

        UpdateMenuLanguage();
	}

	public void UpdateMenuLanguage() {
		if(m_lastUpdatedLanguage == Game.m_languages.GetCurrentLanguage()) return;

        Game.m_languages.UpdateObjectLanguage(gameObject, m_lastUpdatedLanguage);
        m_lastUpdatedLanguage = Game.m_languages.GetCurrentLanguage();
	}

	public void SetPrevious(Menu p_menu) { 
		m_previousMenu = p_menu;
	}

	public void CloseControlMapper() { // needed cause otherwise you don't get a handler...
		MenuHandler.Instance.CloseControlMapper();
	}
}
