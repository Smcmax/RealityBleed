using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Rewired.UI.ControlMapper;
using UnityEngine.EventSystems;

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

    [Tooltip("The game event raised when the quest log is brought up")]
    public GameEvent m_onQuestLogEvent;

    public List<Menu> m_openedMenus;
	[HideInInspector] public Rewired.Player m_handlingPlayer; // The player handling the menu

	private bool m_paused;
	private Menu m_previousControlMapperMenu = null;
	private GameObject m_lastSelectedGameObject;

	private MenuHandler() { }

	public static MenuHandler Instance { get; private set; }

	void Start() { Instance = this; }

	void OnEnable() {
		if(Time.timeScale == 0f) m_paused = true;
		else m_paused = false;

		m_openedMenus = new List<Menu>();
		m_handlingPlayer = null;
		m_lastSelectedGameObject = null;
		SceneManager.sceneLoaded += OnSceneLoad;
	}

	void OnDisable() {
		m_handlingPlayer = null;
		m_lastSelectedGameObject = null;
		SceneManager.sceneLoaded -= OnSceneLoad;
	}

	void Update() {
		UpdateHandlingPlayerAllInputs();
		
		if(GetButtonDown("Pause")) Escape();

		if(DialogWindow.m_openedWindows.Count == 0) {
			if(GetButtonDown("UIInteract2")) GoBack();

            string openedMenuButton = GetButtonRelatedToSingleOpenMenu();

			if(openedMenuButton == "" && GetButtonDown("Inventory") && !m_paused && m_onInventoryEvent) 
                m_onInventoryEvent.Raise();

			if(openedMenuButton == "" && GetButtonDown("Character") && !m_paused && m_onCharacterEvent) 
                m_onCharacterEvent.Raise();

			if((openedMenuButton == "" || openedMenuButton == "Skillbook") && 
                GetButtonDown("Skillbook") && !m_paused && m_onSkillbookEvent) 
                m_onSkillbookEvent.Raise();

			if((openedMenuButton == "" || openedMenuButton == "Map") && 
                GetButtonDown("Map") && !m_paused && m_onMapEvent) 
                m_onMapEvent.Raise();

			if((openedMenuButton == "" || openedMenuButton == "QuestLog") && 
                GetButtonDown("QuestLog") && !m_paused && m_onQuestLogEvent) 
                m_onQuestLogEvent.Raise();
		}

		GameObject selected = EventSystem.current.currentSelectedGameObject;

		// if the player moves the UI with a selectable way of navigation (d-pad), set the selectable properly (it's unset when moving cursor)
		if(m_handlingPlayer != null && !selected && (m_handlingPlayer.GetAxisRaw("UIMoveX") != 0 || m_handlingPlayer.GetAxisRaw("UIMoveY") != 0)) { 
			if(m_lastSelectedGameObject != null && m_lastSelectedGameObject.activeSelf)
				EventSystem.current.SetSelectedGameObject(m_lastSelectedGameObject);
		} else if(m_handlingPlayer != null && selected && m_lastSelectedGameObject != selected) { // save the last selected game object
			m_lastSelectedGameObject = selected;
		}
    }

	private bool GetButtonDown(string p_button) { 
		if(m_listeningToAllInputs || m_handlingPlayer == null) {
			foreach(Rewired.Player player in Rewired.ReInput.players.Players)
				if(player.GetButtonDown(p_button)) {
					if(m_handlingPlayer == null) {
						m_handlingPlayer = player;
						Game.m_options.UpdateUIControls();
					}
					
					return true;
				}
		} else return m_handlingPlayer.GetButtonDown(p_button);

		return false;
	}

	private void UpdateHandlingPlayerAllInputs() {
		bool changed = false;

		if(m_listeningToAllInputs) {
            foreach(Rewired.Player player in Rewired.ReInput.players.Players) {
				if(player.GetAnyButton() || player.GetAxis("UIAimX") != 0 || player.GetAxis("UIAimY") != 0) {
                    m_handlingPlayer = player;
                    changed = true;
				}
			}
		}

		if(changed) Game.m_options.UpdateUIControls();
	}

	public void Escape() {
		if(!gameObject.activeSelf) { Instance.Escape(); return; }

		if(m_openedMenus.Count == 0) m_pauseEvent.Raise();
		else GoBack();
	}

	public void GoBack() {
		if(!gameObject.activeSelf) { Instance.GoBack(); return; }

		if(m_openedMenus.Count > 0) {
			if(m_previousControlMapperMenu) CloseControlMapper();
			else if(m_openedMenus.Count == 1 && m_openedMenus[0].m_previousMenu) OpenMenu(m_openedMenus[0].m_previousMenu);
			else if(m_paused) m_resumeEvent.Raise();
			else if(!m_openedMenus.Exists(m => !m.m_closeable)) ClearMenu();
		}
	}

	// it does its own stuff so it needs its own special snowflake functions to integrate into this handler properly
	public void OpenControlMapper() {
		if(!gameObject.activeSelf) { Instance.OpenControlMapper(); return; }

        Game.m_controlMapper.Open();
		OpenMenu(Game.m_controlMapperMenu);

		foreach(Menu opened in new List<Menu>(m_openedMenus))
			if(opened != null && opened.gameObject.name == Game.m_controlMapperMenu.m_previousMenu.gameObject.name) {
				// we have to keep a reference to the previous menu here cause the one set in control mapper is a prefab, which isn't openable
				m_previousControlMapperMenu = opened;
				CloseMenu(opened);

				break;
			}
	}

	public void CloseControlMapper() {
		if(!gameObject.activeSelf) { Instance.CloseControlMapper(); return; }

		Game.m_controlMapperMenu.gameObject.transform.parent.GetComponent<ControlMapper>().Close(true); // save settings

		if(m_previousControlMapperMenu is TabMenu) OpenTabMenu((TabMenu) m_previousControlMapperMenu);
		else OpenMenu(m_previousControlMapperMenu);

		CloseMenu(Game.m_controlMapperMenu); // close it internally, it's already closed though
		m_previousControlMapperMenu = null;
	}

	public void OpenMenu(Menu p_menu) {
		if(!gameObject.activeSelf) { Instance.OpenMenu(p_menu); return; }

		if(m_openedMenus.Contains(p_menu)) {
			CloseMenu(p_menu);
			return;
		}

        if(p_menu.m_singleOpenedMenu && m_openedMenus.Count > 0)
            foreach(Menu opened in new List<Menu>(m_openedMenus)) {
                m_openedMenus.Remove(opened);
                opened.gameObject.SetActive(false);
            }

		p_menu.gameObject.SetActive(true);

		foreach(Menu opened in new List<Menu>(m_openedMenus)) {
			if(p_menu == opened.m_previousMenu || p_menu.m_previousMenu == opened) {
				m_openedMenus.Remove(opened);
				opened.gameObject.SetActive(false);
			}
		}

		if(p_menu is TabMenu) ((TabMenu) p_menu).ResetTab();

		m_openedMenus.Add(p_menu);
		m_onMenuChangedEvent.Raise();

		if(m_handlingPlayer != null && Player.m_players.Count > 0) 
			Player.GetPlayerFromId(m_handlingPlayer.id).m_mouse.ChangeMode(CursorModes.CURSOR, false);
	}

	public void OpenTabMenu(TabMenu p_menu) { 
		OpenTabMenu(p_menu, p_menu.m_currentTab);
	}

	public void OpenTabMenu(TabMenu p_menu, string p_tabName) { 
		OpenMenu(p_menu);

		p_menu.Select(p_tabName);
	}

	public void CloseMenu(Menu p_menu) {
		if(!gameObject.activeSelf) { MenuHandler.Instance.CloseMenu(p_menu); return; }

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
			foreach(Menu menu in new List<Menu>(m_openedMenus))
				menu.gameObject.SetActive(false);

		if(m_openedMenus.Count > 0) m_onMenuChangedEvent.Raise();

		if(m_handlingPlayer != null && Player.m_players.Count > 0) 
			Player.GetPlayerFromId(m_handlingPlayer.id).m_mouse.ChangeMode(CursorModes.LINE, false);

		m_openedMenus.Clear();
		m_handlingPlayer = null;
		m_lastSelectedGameObject = null;
	}

    private bool IsSingleOpenMenuOpen() {
        foreach(Menu opened in m_openedMenus)
            if(opened.m_singleOpenedMenu) return true;

        return false;
    }

    private string GetButtonRelatedToSingleOpenMenu() {
        foreach(Menu opened in m_openedMenus)
            if(opened.m_singleOpenedMenu) return opened.m_menuButtonName;

        return "";
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
		UIItem.GhostCanvas = GameObject.Find("Mouse Canvas").transform;
		if(m_resumeEvent) m_resumeEvent.Raise();
	}

	public void Quit() {
		if(!Application.isEditor) System.Diagnostics.Process.GetCurrentProcess().Kill();
		else Application.Quit();
	}

	public void ReloadConfigs() { 
		ShotPattern.LoadAll();
		BaseItem.LoadAll();
		Game.m_npcGenerator.LoadTypes(true);
        Game.m_enemyGenerator.LoadTypes();
	}
}
