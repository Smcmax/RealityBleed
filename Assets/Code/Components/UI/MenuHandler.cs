using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections.Generic;

public class MenuHandler : MonoBehaviour {

	[Tooltip("The currently opened menu. If null, the game is not currently paused")]
	public GameObject m_currentMenu;

	[Tooltip("The game event raised when the game is paused")]
	public GameEvent m_pauseEvent;

	[Tooltip("The game event raised when the game is resumed")]
	public GameEvent m_resumeEvent;

	[Tooltip("The game event raised when the menu changes")]
	public GameEvent m_onMenuChangedEvent;

	[Tooltip("The game event raised when the inventory is brought up")]
	public GameEvent m_onInventoryEvent;

	[Tooltip("The game event raised when the character screen is brought up")]
	public GameEvent m_onCharacterEvent;
	
	private GameObject m_previousMenu; // The previously opened menu, for use with the pause menu only
	[HideInInspector] public List<GameObject> m_openedMenus;

	void OnEnable() {
		m_openedMenus = new List<GameObject>();
		SceneManager.sceneLoaded += OnSceneLoad;
	}

	void OnDisable() { 
		SceneManager.sceneLoaded -= OnSceneLoad;
	}

	void Update() {
		bool isPaused = m_currentMenu;

		if(Game.m_keybinds.GetButtonDown("Pause")) GoBack();
		if(Game.m_keybinds.GetButtonDown("Inventory") && !isPaused) m_onInventoryEvent.Raise();
		if(Game.m_keybinds.GetButtonDown("Character") && !isPaused) m_onCharacterEvent.Raise();
	}

	public void GoBack() {
		bool isPaused = m_currentMenu;

		if(m_openedMenus.Count > 0 && !(m_openedMenus.Count == 1 && isPaused)) ClearMenu();
		else if(isPaused) {
			if(m_previousMenu) OpenMenu(m_previousMenu, true);
			else m_resumeEvent.Raise();
		} else m_pauseEvent.Raise();
	}

	public void OpenMenuPause(GameObject p_menu) { 
		OpenMenu(p_menu, true);
	}

	public void OpenMenu(GameObject p_menu) {
		OpenMenu(p_menu, false);
	}

	public void OpenMenu(GameObject p_menu, bool p_pause) {
		if(p_pause) {
			if(m_currentMenu) m_currentMenu.SetActive(false);
			if(m_previousMenu == p_menu) { m_openedMenus.Remove(m_currentMenu); m_previousMenu = null; }
			else m_previousMenu = m_currentMenu;

			m_currentMenu = p_menu;
			m_currentMenu.SetActive(true);

			if(m_previousMenu) m_openedMenus.Remove(m_previousMenu);
			m_openedMenus.Add(p_menu);
		} else {
			if(m_openedMenus.Contains(p_menu)) { 
				p_menu.SetActive(false);
				m_openedMenus.Remove(p_menu);
			} else {
				p_menu.SetActive(true);
				m_openedMenus.Add(p_menu);
			}
		}

		m_onMenuChangedEvent.Raise();
	}

	public void ClearMenu() {
		if(m_currentMenu) m_currentMenu.SetActive(false);
		if(m_openedMenus.Count > 0)
			foreach(GameObject menu in m_openedMenus)
				menu.SetActive(false);

		m_openedMenus.Clear();
		m_currentMenu = null;
		m_previousMenu = null;
	}

	public void ChangeScenes(string scene) {
		SceneManager.LoadScene(scene);
	}

	public void Pause() {
		Time.timeScale = 0f;
	}

	public void Resume() { 
		Time.timeScale = 1f;
	}

	void OnSceneLoad(Scene p_scene, LoadSceneMode p_mode) { 
		if(m_resumeEvent) m_resumeEvent.Raise();
	}

	public void Quit() { 
		Application.Quit();
	}
}
