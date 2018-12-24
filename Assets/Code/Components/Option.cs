using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

/*
 * Option handles both the saving/recording of the value through controls and the loading on scene launch. 
 */
public class Option : MonoBehaviour {

	[Tooltip("The component controlling the option's value (slider, toggle, etc.), null = no updating")]
	public Component m_control;

	[Tooltip("The key used to save and retrieve the option")]
	public string m_saveKey;

	[Tooltip("Int, Float, String or Bool")]
	public string m_type;

	[Header("Startup/Update Events")]

	public UnityIntEvent m_intEvent;
	public UnityFloatEvent m_floatEvent;
	public UnityStringEvent m_stringEvent;
	public UnityBoolEvent m_boolEvent;

	public UnityEvent m_postStartupEvent;

	void Start() {
		if(m_control && HasKey()) { 
			if(m_control is Slider) ((Slider) m_control).value = GetFloatValue();
			else if(m_control is Toggle) { 
				Toggle toggle = ((Toggle) m_control);

				toggle.isOn = GetBoolValue(toggle.isOn);
				if(toggle.isOn != GetBoolValue()) Save(toggle.isOn); // save if the default is used (and it's different from false)
			}
		}

		CallEvents();
		LateStart(1f);
	}

	IEnumerator LateStart(float p_wait) { 
		yield return new WaitForSeconds(p_wait);

		m_postStartupEvent.Invoke();
	}

	public void CallEvents() {
		switch(m_type.ToLower()) {
			case "int": case "integer":
				m_intEvent.Invoke(GetIntValue()); break;
			case "float": case "real":
				m_floatEvent.Invoke(GetFloatValue()); break;
			case "string": case "text":
				m_stringEvent.Invoke(GetStringValue()); break;
			case "bool": case "boolean":
				m_boolEvent.Invoke(GetBoolValue()); break;
			default: break;
		}
	}

	public int GetIntValue() { 
		return PlayerPrefs.GetInt(m_saveKey, 0);
	}

	public int GetIntValue(int p_default) { 
		return PlayerPrefs.GetInt(m_saveKey, p_default);
	}

	public float GetFloatValue() { 
		return PlayerPrefs.GetFloat(m_saveKey, 0);
	}

	public float GetFloatValue(float p_default) { 
		return PlayerPrefs.GetFloat(m_saveKey, p_default);
	}

	public string GetStringValue() { 
		return PlayerPrefs.GetString(m_saveKey, "");
	}

	public string GetStringValue(string p_default) { 
		return PlayerPrefs.GetString(m_saveKey, p_default);
	}

	public bool GetBoolValue() { 
		return GetIntValue() == 1;
	}

	public bool GetBoolValue(bool p_default) { 
		return GetIntValue(p_default ? 1 : 0) == 1;
	}

	public bool HasKey() { return PlayerPrefs.HasKey(m_saveKey); }

	public void Save(int p_value) { 
		PlayerPrefs.SetInt(m_saveKey, p_value);
		PlayerPrefs.Save();

		CallEvents();
	}

	public void Save(float p_value) {
		PlayerPrefs.SetFloat(m_saveKey, p_value);
		PlayerPrefs.Save();

		CallEvents();
	}

	public void Save(string p_value) {
		PlayerPrefs.SetString(m_saveKey, p_value);
		PlayerPrefs.Save();

		CallEvents();
	}

	public void Save(bool p_value) {
		PlayerPrefs.SetInt(m_saveKey, p_value ? 1 : 0);
		PlayerPrefs.Save();

		CallEvents();
	}
}
