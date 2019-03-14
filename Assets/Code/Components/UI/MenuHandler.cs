using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Rewired.UI.ControlMapper;

public class MenuHandler : MonoBehaviour {

	[Tooltip("If the menu can be handled by any player or only the player who opened it")]
	public bool m_listeningToAllInputs;

	[Tooltip("The menu used for containers")]
	public Menu m_containerMenu;

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

	[Tooltip("The game event raised when the skillbook screen is brought up")]
	public GameEvent m_onSkillbookEvent;
	
	[Tooltip("The game event raised when the map screen is brought up")]
	public GameEvent m_onMapEvent;

	[HideInInspector] public List<Menu> m_openedMenus;
	[HideInInspector] public Rewired.Player m_handlingPlayer; // The player handling the menu

	private bool m_paused;
	private Menu m_previousControlMapperMenu = null;

	private MenuHandler() { }

	public static MenuHandler Instance { get; private set; }

	void Start() { Instance = this; }

	void OnEnable() {
		if(Time.timeScale == 0f) m_paused = true;
		else m_paused = false;

		m_openedMenus = new List<Menu>();
		m_handlingPlayer = null;
		SceneManager.sceneLoaded += OnSceneLoad;
	}

	void OnDisable() {
		m_handlingPlayer = null;
		SceneManager.sceneLoaded -= OnSceneLoad;
	}

	void Update() {
		if(GetButtonDown("Pause")) GoBack();
		if(GetButtonDown("Inventory") && !m_paused) m_onInventoryEvent.Raise();
		if(GetButtonDown("Character") && !m_paused) m_onCharacterEvent.Raise();
		if(GetButtonDown("Skillbook") && !m_paused) m_onSkillbookEvent.Raise();
		if(GetButtonDown("Map") && !m_paused) m_onMapEvent.Raise();
	}

	private bool GetButtonDown(string p_button) { 
		if(m_listeningToAllInputs || m_handlingPlayer == null) {
			foreach(Rewired.Player player in Rewired.ReInput.players.Players)
				if(player.GetButtonDown(p_button)) {
					if(m_handlingPlayer == null) m_handlingPlayer = player;
					
					return true;
				}
		} else return m_handlingPlayer.GetButtonDown(p_button);

		return false;
	}

	public void GoBack() {
		if(m_openedMenus.Count > 0) {
			if(m_paused && m_previousControlMapperMenu) CloseControlMapper();
			else if(m_openedMenus.Count == 1 && m_openedMenus[0].m_previousMenu) OpenMenu(m_openedMenus[0].m_previousMenu);
			else if(m_paused) m_resumeEvent.Raise();
			else ClearMenu();
		} else m_pauseEvent.Raise();
	}

	// it does its own stuff so it needs its own special snowflake functions to integrate into this handler properly
	public void OpenControlMapper() {
		OpenMenu(Game.m_controlMapperMenu);

		foreach(Menu opened in new List<Menu>(m_openedMenus))
			if(opened.gameObject.name == Game.m_controlMapperMenu.m_previousMenu.gameObject.name) {
				// we have to keep a reference to the previous menu here cause the one set in control mapper is a prefab, which isn't openable
				m_previousControlMapperMenu = opened;
				CloseMenu(opened);

				break;
			}
		
	}

	public void CloseControlMapper() {
		Game.m_controlMapperMenu.gameObject.transform.parent.GetComponent<ControlMapper>().Close(true); // save settings
		OpenMenu(m_previousControlMapperMenu);
		CloseMenu(Game.m_controlMapperMenu); // close it internally, it's already closed though
		m_previousControlMapperMenu = null;
	}

	public void OpenMenu(Menu p_menu) {
		// if we've got 2 menu handlers in the scene, we can choose which one to use by disabling one (redirecting menu opening to the one left active)
		if(!gameObject.activeSelf) {
			GameObject otherMenu = GameObject.Find("MenuHandler");

			if(otherMenu != null) {
				Instance = otherMenu.GetComponent<MenuHandler>();
				Instance.OpenMenu(p_menu);
			}

			return;
		}

		if(m_openedMenus.Contains(p_menu)) {
			CloseMenu(p_menu);
			return;
		}

		p_menu.gameObject.SetActive(true);

		foreach(Menu opened in new List<Menu>(m_openedMenus)) {
			if(p_menu == opened.m_previousMenu || p_menu.m_previousMenu == opened) {
				m_openedMenus.Remove(opened);
				opened.gameObject.SetActive(false);
			}
		}

		m_openedMenus.Add(p_menu);
		m_onMenuChangedEvent.Raise();
	}

	public void CloseMenu(Menu p_menu) {
		// if we've got 2 menu handlers in the scene, we can choose which one to use by disabling one (redirecting menu opening to the one left active)
		if(!gameObject.activeSelf) {
			GameObject otherMenu = GameObject.Find("MenuHandler");

			if(otherMenu != null) {
				Instance = otherMenu.GetComponent<MenuHandler>();
				Instance.CloseMenu(p_menu);
			}

			return;
		}

		if(m_openedMenus.Contains(p_menu)) {
			if(m_openedMenus.Count == 1 && m_paused) m_resumeEvent.Raise();
			else if(m_openedMenus.Count == 1) ClearMenu();
			else {
				m_openedMenus.Remove(p_menu);
				p_menu.gameObject.SetActive(false);

				m_onMenuChangedEvent.Raise();
			}
		}
	}

	public void ClearMenu() {
		if(m_openedMenus.Count > 0)
			foreach(Menu menu in m_openedMenus)
				menu.gameObject.SetActive(false);

		if(m_openedMenus.Count > 0) m_onMenuChangedEvent.Raise();

		m_openedMenus.Clear();
		m_handlingPlayer = null;
	}

	public void ChangeScenes(string scene) {
		SceneManager.LoadScene(scene);
	}

	public void Pause() {
		Time.timeScale = 0f;
		m_paused = true;
	}

	public void Resume() { 
		Time.timeScale = 1f;
		m_paused = false;
	}

	void OnSceneLoad(Scene p_scene, LoadSceneMode p_mode) { 
		if(m_resumeEvent) m_resumeEvent.Raise();
	}

	public void Quit() { 
		Application.Quit();
	}
}
