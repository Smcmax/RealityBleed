using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System;

public class Option : MonoBehaviour {

	[Tooltip("The component controlling the option's value (slider, toggle, etc.), null = no updating")]
	public Component m_control;

	[Tooltip("The key used to save and retrieve the option")]
	public string m_saveKey;

	[Tooltip("int, float, string or bool")]
	public string m_type;

	[Tooltip("This option's default value")]
	[ConditionalField("m_type", "int")] public int m_intDefault;
	
	[Tooltip("This option's default value")]
	[ConditionalField("m_type", "bool")] public bool m_boolDefault;	

	[Tooltip("This option's default value")]
	[ConditionalField("m_type", "float")] public float m_floatDefault;

	[Tooltip("This option's default value")]
	[ConditionalField("m_type", "string")] public string m_stringDefault;

	[Tooltip("Time in seconds before the post-startup event is called")]
	[Range(0, 2.5f)] public float m_postStartupDelay;

	[Header("Startup/Update Events")]

	public UnityIntEvent m_intEvent;
	public UnityFloatEvent m_floatEvent;
	public UnityStringEvent m_stringEvent;
	public UnityBoolEvent m_boolEvent;

	public UnityEvent m_postStartupEvent;

	void Start() {
		OptionValue value = null;

		switch(m_type.ToLower()) {
			case "int": case "integer": 
				value = Game.m_options.LoadOptionInt(m_saveKey, m_intDefault); break;
			case "float": case "real":
				value = Game.m_options.LoadOptionFloat(m_saveKey, m_floatDefault); break;
			case "string": case "text":
				value = Game.m_options.LoadOptionString(m_saveKey, m_stringDefault); break;
			case "bool": case "boolean":
				value = Game.m_options.LoadOptionBool(m_saveKey, m_boolDefault); break;
			default: break;
		}

		if(m_control) { 
			if(m_control is Slider)
				((Slider) m_control).value = value.GetFloat();
			else if(m_control is AdaptativeSliderText)
				((AdaptativeSliderText) m_control).m_value = value.GetInt();
			else if(m_control is TextualSliderText) 
				((TextualSliderText) m_control).m_value = value.GetInt();
			else if(m_control is Toggle)
				((Toggle) m_control).isOn = value.GetBool();
		}

		CallEvents();
		StartCoroutine(LateStart(m_postStartupDelay));
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
		return Game.m_options.Get(m_saveKey).GetInt();
	}

	public float GetFloatValue() { 
		return Game.m_options.Get(m_saveKey).GetFloat();
	}

	public string GetStringValue() { 
		return Game.m_options.Get(m_saveKey).GetString();
	}

	public bool GetBoolValue() { 
		return Game.m_options.Get(m_saveKey).GetBool();
	}

	public bool HasKey() { return PlayerPrefs.HasKey(m_saveKey); }

	public void Save(int p_value) { 
		Game.m_options.Get(m_saveKey).Save(p_value);

		CallEvents();
	}

	public void Save(float p_value) {
		Game.m_options.Get(m_saveKey).Save(p_value);

		CallEvents();
	}

	public void Save(string p_value) {
		Game.m_options.Get(m_saveKey).Save(p_value);

		CallEvents();
	}

	public void Save(bool p_value) {
		Game.m_options.Get(m_saveKey).Save(p_value);

		CallEvents();
	}
}
