using UnityEngine;
using UnityEngine.EventSystems;

public class AutoSelectObject : MonoBehaviour {

	void OnEnable() {
		Select();
	}

	public void Select() {
		EventSystem.current.SetSelectedGameObject(gameObject);
	}
}
