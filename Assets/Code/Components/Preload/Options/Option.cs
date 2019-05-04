using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using System;

public class Option : MonoBehaviour {

	public static List<Option> m_loadedOptions = new List<Option>();

	[Tooltip("The component controlling the option's value (slider, toggle, etc.), null = no updating")]
	public Component m_control;

	[Tooltip("The key used to save and retrieve the option")]
	public string m_saveKey;

	[Tooltip("If the option should be set per-player and not globally")]
	public bool m_playerSpecific;

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
	public UnityOptionEvent m_optionEvent;

	public UnityEvent m_postStartupEvent;

	void Start() {
		if(m_playerSpecific)
			for(int i = 0; i < Constants.MAX_PLAYER_COUNT; i++)
				LoadOption(m_saveKey + "_" + i);
		else LoadOption(m_saveKey);

		UpdateUIControl();
		CallEvents();
		StartCoroutine(LateStart(m_postStartupDelay));
	}

	private void LoadOption(string p_key) {
		switch(m_type.ToLower()) {
			case "int": case "integer": 
				Game.m_options.LoadOptionInt(p_key, m_intDefault); break;
			case "float": case "real":
				Game.m_options.LoadOptionFloat(p_key, m_floatDefault); break;
			case "string": case "text":
				Game.m_options.LoadOptionString(p_key, m_stringDefault); break;
			case "bool": case "boolean":
				Game.m_options.LoadOptionBool(p_key, m_boolDefault); break;
			default: break;
		}
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

		m_optionEvent.Invoke(this);
	}

	public void UpdateUIControl() {
		OptionValue value = Game.m_options.Get(GetCurrentKey());

		if(m_control) { 
			if(m_control is Slider)
				((Slider) m_control).value = value.GetFloat();
			else if(m_control is AdaptativeSliderText)
				((AdaptativeSliderText) m_control).m_value = value.GetInt();
			else if(m_control is TextualSliderText) 
				((TextualSliderText) m_control).m_value = value.GetInt();
			else if(m_control is Toggle)
				((Toggle) m_control).isOn = value.GetBool();
			else if(m_control is CursorSelector)
				((CursorSelector) m_control).Load();
		}
	}

	public string GetCurrentKey() {
		string key = m_saveKey;

		if(m_playerSpecific) {
			if(MenuHandler.Instance && MenuHandler.Instance.m_handlingPlayer != null)
				key += "_" + MenuHandler.Instance.m_handlingPlayer.id;
			else key += "_0";
		}

		return key;
	}

	public int GetIntValue() { 
		return Game.m_options.Get(GetCurrentKey()).GetInt();
	}
	
	public int GetIntValue(int p_playerId) {
		return Game.m_options.Get(m_saveKey, p_playerId).GetInt();
	}

	public float GetFloatValue() { 
		return Game.m_options.Get(GetCurrentKey()).GetFloat();
	}

	public float GetFloatValue(int p_playerId) {
		return Game.m_options.Get(m_saveKey, p_playerId).GetFloat();
	}

	public string GetStringValue() { 
		return Game.m_options.Get(GetCurrentKey()).GetString();
	}

	public string GetStringValue(int p_playerId) {
		return Game.m_options.Get(m_saveKey, p_playerId).GetString();
	}

	public bool GetBoolValue() { 
		return Game.m_options.Get(GetCurrentKey()).GetBool();
	}

	public bool GetBoolValue(int p_playerId) {
		return Game.m_options.Get(m_saveKey, p_playerId).GetBool();
	}

	public bool HasKey() { return PlayerPrefs.HasKey(GetCurrentKey()); }

	public bool HasKey(int p_playerId) { return PlayerPrefs.HasKey(m_saveKey + "_" + p_playerId); }

	public void Save(int p_value) { 
		Game.m_options.Get(GetCurrentKey()).Save(p_value);

		CallEvents();
	}

    public void Save(int p_playerId, int p_value) {
        Game.m_options.Get(m_saveKey, p_playerId).Save(p_value);

        CallEvents();
    }

	public void Save(float p_value) {
		Game.m_options.Get(GetCurrentKey()).Save(p_value);

		CallEvents();
	}

    public void Save(int p_playerId, float p_value) {
        Game.m_options.Get(m_saveKey, p_playerId).Save(p_value);

        CallEvents();
    }

	public void Save(string p_value) {
		Game.m_options.Get(GetCurrentKey()).Save(p_value);

		CallEvents();
	}

    public void Save(int p_playerId, string p_value) {
        Game.m_options.Get(m_saveKey, p_playerId).Save(p_value);

        CallEvents();
    }

	public void Save(bool p_value) {
		Game.m_options.Get(GetCurrentKey()).Save(p_value);

		CallEvents();
	}

    public void Save(int p_playerId, bool p_value) {
        Game.m_options.Get(m_saveKey, p_playerId).Save(p_value);

        CallEvents();
    }
}
