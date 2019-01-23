using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class KeybindRebinder : MonoBehaviour {

	[Tooltip("Background to change")]
	public Image m_background;

	[Tooltip("Name of the axis to rebind with this control")]
	public string m_keybindName;

	[Tooltip("True if you're rebinding the positive key, false if negative key")]
	public bool m_positiveKeyRebind;

	private Text m_label;
	private Text m_text;
	private Keybind m_keybind;
	private bool m_rebinding;

	public void Init() { 
		m_label = transform.parent.GetComponentInChildren<Text>();
		m_text = GetComponentInChildren<Text>();
		UpdateKeybind();
	}

	public void UpdateKeybind() {
		m_keybind = Game.m_keybinds.GetKeybinds().Find(k => k.m_axis.Key == m_keybindName);
		UpdateText();
	}

	void Update() { 
		if(m_rebinding && m_keybind != null) { 
			foreach(KeyCode key in Enum.GetValues(typeof(KeyCode))) { 
				if(Input.GetKeyDown(key)) { 
					if(m_positiveKeyRebind) {
						// if both keys are unbound, bind the main key and call it a day
						if(m_keybind.m_positiveKey == KeyCode.None && m_keybind.m_altPositiveKey == KeyCode.None)
							m_keybind.m_positiveKey = key;
						else if(m_keybind.m_positiveKey != key) { 
							if(m_keybind.m_altPositiveKey == key) // if we pressed the alt key, wipe it
								m_keybind.m_altPositiveKey = KeyCode.None;
							else if(m_keybind.m_altPositiveKey != KeyCode.None) { // if both keys are bound, swap primary keypress
								m_keybind.m_positiveKey = key;
							} else m_keybind.m_altPositiveKey = key; // adding an alternate keypress
						} else { 
							if(m_keybind.m_altPositiveKey != KeyCode.None) { // if we're unbinding the main key, but there's an alt key, swap
								m_keybind.m_positiveKey = m_keybind.m_altPositiveKey;
								m_keybind.m_altPositiveKey = KeyCode.None;
							} else m_keybind.m_positiveKey = KeyCode.None; // otherwise just unset all
						}
					} else { // same logic as above but with negative keys
						if(m_keybind.m_negativeKey == KeyCode.None && m_keybind.m_altNegativeKey == KeyCode.None)
							m_keybind.m_negativeKey = key;
						else if(m_keybind.m_negativeKey != key) {
							if(m_keybind.m_altNegativeKey == key)
								m_keybind.m_altNegativeKey = KeyCode.None;
							else if(m_keybind.m_altNegativeKey != KeyCode.None) {
								m_keybind.m_negativeKey = key;
							} else m_keybind.m_altNegativeKey = key;
						} else {
							if(m_keybind.m_altNegativeKey != KeyCode.None) {
								m_keybind.m_negativeKey = m_keybind.m_altNegativeKey;
								m_keybind.m_altNegativeKey = KeyCode.None;
							} else m_keybind.m_negativeKey = KeyCode.None;
						}
					}

					Game.m_keybinds.SaveKeybind(m_keybind);
					StopRebinding();
					break;
				}
			}
		}
	}

	private void UpdateText() {
		string keys = "";

		if(m_positiveKeyRebind) {
			if(m_keybind.m_positiveKey != KeyCode.None) {
				keys += m_keybind.m_positiveKey.ToString();

				if(m_keybind.m_altPositiveKey != KeyCode.None) 
					keys += "," + m_keybind.m_altPositiveKey.ToString();
			} 
		} else {
			if(m_keybind.m_negativeKey != KeyCode.None) {
				keys += m_keybind.m_negativeKey.ToString();

				if(m_keybind.m_altNegativeKey != KeyCode.None)
					keys += "," + m_keybind.m_altNegativeKey.ToString();
			} 
		}

		if(keys == "") keys = "Unset";

		m_text.text = string.Concat(keys.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' '); ;
		m_label.text = m_positiveKeyRebind ? m_keybind.m_positiveDisplayName : m_keybind.m_negativeDisplayName;
	}

	public void Rebind() {
		if(m_rebinding) { StopRebinding(); return; }

		m_rebinding = true; 
		m_background.color = new Color(1, 1, 1, 1);
		Game.m_keybinds.m_blockAllKeybinds = true;
	}

	public void StopRebinding() {
		Game.m_keybinds.m_blockAllKeybinds = false;
		m_rebinding = false;
		m_background.color = new Color(1, 1, 1, 100f / 255f);
		UpdateText();
		EventSystem.current.SetSelectedGameObject(null);
	}
}
