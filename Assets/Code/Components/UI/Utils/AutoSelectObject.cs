using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class AutoSelectObject : MonoBehaviour {

	void OnEnable() {
        StartSelectCoroutine();
	}

    public void StartSelectCoroutine() {
        StartCoroutine(Select());
    }

	public IEnumerator Select() {
		yield return new WaitForSecondsRealtime(0.01f);

		if(EventSystem.current) {
			EventSystem.current.SetSelectedGameObject(gameObject);
			GetComponent<Selectable>().OnSelect(null);
		}
	}
}
