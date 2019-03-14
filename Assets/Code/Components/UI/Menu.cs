using UnityEngine;

public class Menu : MonoBehaviour {

	[Tooltip("The menu to go back to from the current menu, null if none")]
	public Menu m_previousMenu;

	public void CloseControlMapper() { // needed cause otherwise you don't get a handler...
		MenuHandler.Instance.CloseControlMapper();
	}
}
