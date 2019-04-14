using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoSelectObject : MonoBehaviour {

	void OnEnable() {
		Select();
	}

	public void Select() {
		if(EventSystem.current) {
			EventSystem.current.SetSelectedGameObject(gameObject);
			GetComponent<Selectable>().OnSelect(null);
		}
	}
}
