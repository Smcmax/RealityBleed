using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour {

	[Tooltip("The currently opened menu. If null, the game is not currently paused")]
	public GameObject m_currentMenu;

	[Tooltip("The game event raised when the game is paused")]
	public GameEvent m_pauseEvent;

	[Tooltip("The game event raised when the game is resumed")]
	public GameEvent m_resumeEvent;

	[Tooltip("The game event raised when the menu changes")]
	public GameEvent m_onMenuChangedEvent;
	
	private GameObject m_previousMenu; // The previously opened menu.

	void Update() { 
		bool isPaused = m_currentMenu;

		if(Input.GetButtonDown("Cancel")) { 
			if(isPaused) {
				if(m_previousMenu) ChangeMenu(m_previousMenu);
				else m_resumeEvent.Raise();
			} else m_pauseEvent.Raise();
		}
	}

	public void ChangeMenu(GameObject p_menu) {
		if(m_currentMenu) m_currentMenu.SetActive(false);
		if(m_previousMenu == p_menu) m_previousMenu = null;
		else m_previousMenu = m_currentMenu;

		m_currentMenu = p_menu;
		m_currentMenu.SetActive(true);
		m_onMenuChangedEvent.Raise();
	}

	public void ClearMenu() {
		if(m_currentMenu) m_currentMenu.SetActive(false);

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

	public void Quit() { 
		Application.Quit();
	}
}
