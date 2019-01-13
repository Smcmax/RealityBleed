using UnityEngine;
using System.Collections.Generic;

public class KeybindRebinderLoader : MonoBehaviour {

	[Tooltip("The prefab used for each rebindable keybind")]
	public GameObject m_keybindRebindItemPrefab;

	void Start() {
		Load();
	}

	public void Load() { 
		for(int i = 0; i < transform.childCount; i++)
			Destroy(transform.GetChild(i));

		List<Keybind> keybinds = Game.m_keybinds.GetKeybinds();

		foreach(Keybind keybind in keybinds) {
			bool negative = keybind.m_negativeKey != KeyCode.None;

			LoadItem(keybind, true);
			if(negative) LoadItem(keybind, false);
		}
	}

	public void LoadItem(Keybind p_keybind, bool p_positive) {
		GameObject item = Instantiate(m_keybindRebindItemPrefab, transform);
		KeybindRebinder rebinder = item.GetComponentInChildren<KeybindRebinder>();

		rebinder.m_keybindName = p_keybind.m_axis.Key;
		rebinder.m_positiveKeyRebind = p_positive;

		rebinder.Init();
	}
}