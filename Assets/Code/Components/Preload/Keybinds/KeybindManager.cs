using UnityEngine;
using System.Collections.Generic;

// one manager per player if we ever have local coop?
public class KeybindManager : MonoBehaviour {

	public List<Keybind> m_mouseAndKeyboard;
	public List<Keybind> m_controllerOne;

	[HideInInspector] public string m_currentProfile;
	[HideInInspector] public bool m_blockAllKeybinds;

	void OnEnable() {
		LoadKeybinds();

		foreach(Keybind keybind in GetAllKeybinds())
			keybind.m_axis.StartTracking();		

		SimpleInput.OnUpdate += OnKeybindUpdate;
	}

	void OnDisable() {
		SaveKeybinds();

		foreach(Keybind keybind in GetAllKeybinds())
			keybind.m_axis.StopTracking();

		SimpleInput.OnUpdate -= OnKeybindUpdate;
	}

	public List<Keybind> GetKeybinds() { 
		switch(m_currentProfile) {
			case "mouseAndKB": return m_mouseAndKeyboard;
			case "controller1": return m_controllerOne;
			default: return new List<Keybind>();
		}
	}

	public List<Keybind> GetAllKeybinds() { 
		List<Keybind> list = new List<Keybind>();

		list.AddRange(m_mouseAndKeyboard);
		list.AddRange(m_controllerOne);

		return list;
	}

	private void LoadKeybinds() { 
		m_currentProfile = PlayerPrefs.GetString("Keybind-CurrentProfile", "mouseAndKB");

		foreach(Keybind keybind in GetAllKeybinds()) { 
			string name = "Keybind-" + keybind.m_axis.Key.Replace(" ", "_");

			if(PlayerPrefs.HasKey(name + "Positive"))
				keybind.m_positiveKey = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(name + "Positive"));

			if(PlayerPrefs.HasKey(name + "AltPositive"))
				keybind.m_altPositiveKey = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(name + "AltPositive"));

			if(PlayerPrefs.HasKey(name + "Negative"))
				keybind.m_negativeKey = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(name + "Negative"));

			if(PlayerPrefs.HasKey(name + "AltNegative"))
				keybind.m_altNegativeKey = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(name + "AltNegative"));
		}
	}

	private void SaveKeybinds() {
		SaveCurrentProfile();

		foreach(Keybind keybind in GetAllKeybinds()) {
			SaveKeybind(keybind);
		}	
	}

	public void SaveCurrentProfile() {
		PlayerPrefs.SetString("Keybind-CurrentProfile", m_currentProfile);
	}

	public void SaveKeybind(Keybind p_keybind) {
		string name = "Keybind-" + p_keybind.m_axis.Key.Replace(" ", "_");

		PlayerPrefs.SetString(name + "Positive", p_keybind.m_positiveKey.ToString());
		PlayerPrefs.SetString(name + "AltPositive", p_keybind.m_altPositiveKey.ToString());
		PlayerPrefs.SetString(name + "Negative", p_keybind.m_negativeKey.ToString());
		PlayerPrefs.SetString(name + "AltNegative", p_keybind.m_altNegativeKey.ToString());
	}

	void Update() { 
		foreach(Keybind keybind in GetKeybinds()) { 
			// update runs right after OnKeybindUpdate, so we let it stay pressed/released until next frame
			if(keybind.m_pressedLastFrame) { 
				keybind.m_pressedThisFrame = false;
				keybind.m_pressedLastFrame = false;
			} else if(keybind.m_pressedThisFrame) keybind.m_pressedLastFrame = true;			
			
			if(keybind.m_releasedLastFrame) { 
				keybind.m_releasedThisFrame = false;
				keybind.m_releasedLastFrame = false;
			} else if(keybind.m_releasedThisFrame) keybind.m_releasedLastFrame = true;
		}
	}

	void OnKeybindUpdate() { 
		if(m_blockAllKeybinds) return;

		foreach(Keybind keybind in GetKeybinds()) {
			if(Input.GetKey(keybind.m_negativeKey) || Input.GetKey(keybind.m_altNegativeKey))
				keybind.m_axis.value = -1f;
			else if(Input.GetKey(keybind.m_positiveKey) || Input.GetKey(keybind.m_altPositiveKey))
				keybind.m_axis.value = 1f;
			else keybind.m_axis.value = 0f;

			if(Input.GetKeyDown(keybind.m_negativeKey) || Input.GetKeyDown(keybind.m_altNegativeKey) || 
				Input.GetKeyDown(keybind.m_positiveKey) || Input.GetKeyDown(keybind.m_altPositiveKey))
				keybind.m_pressedThisFrame = true;

			if (Input.GetKeyUp(keybind.m_negativeKey) || Input.GetKeyUp(keybind.m_altNegativeKey) ||
				Input.GetKeyUp(keybind.m_positiveKey) || Input.GetKeyUp(keybind.m_altPositiveKey))
				keybind.m_releasedThisFrame = true;
		}
	}

	public float GetAxis(string p_keybind) { 
		if(GetKeybinds().Find(k => k.m_axis.Key == p_keybind).m_useRawValues)
			return SimpleInput.GetAxisRaw(p_keybind);
		else return SimpleInput.GetAxis(p_keybind);
	}

	public bool GetButton(string p_keybind) { 
		return GetKeybinds().Find(k => k.m_axis.Key == p_keybind).m_axis.value == 1f;
	}

	public bool GetButtonDown(string p_keybind) { 
		return GetKeybinds().Find(k => k.m_axis.Key == p_keybind).m_pressedThisFrame;
	}	
	
	public bool GetButtonUp(string p_keybind) { 
		return GetKeybinds().Find(k => k.m_axis.Key == p_keybind).m_releasedThisFrame;
	}
}
