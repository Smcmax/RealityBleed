using UnityEngine;

public class Menu : MonoBehaviour {

	[Tooltip("The menu to go back to from the current menu, null if none")]
	public Menu m_previousMenu;

	[Tooltip("Can this menu be closed?")]
	public bool m_closeable;

	public void SetPrevious(Menu p_menu) { 
		m_previousMenu = p_menu;
	}

	public void CloseControlMapper() { // needed cause otherwise you don't get a handler...
		MenuHandler.Instance.CloseControlMapper();
	}
}
