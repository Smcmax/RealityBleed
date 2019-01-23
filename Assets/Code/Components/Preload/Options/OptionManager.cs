using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class OptionManager : MonoBehaviour {

	[Tooltip("Wipes all saved data on start")]
	public bool m_wipeAllDataOnStart;

	[HideInInspector] public List<OptionValue> m_options;

	void Awake() { 
		if(m_wipeAllDataOnStart) PlayerPrefs.DeleteAll();

		m_options = new List<OptionValue>();

		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public OptionValue Get(string p_key) { 
		return m_options.Find(ov => ov.m_key == p_key);
	}

	public OptionValue LoadOption(string p_key, string p_dataType) { 
		OptionValue val = new OptionValue(p_key, p_dataType);

		val.Load();
		m_options.Add(val);

		return val;
	}	
	
	public OptionValue LoadOptionInt(string p_key, int p_default) { 
		OptionValue val = new OptionValue(p_key, "int");

		val.Load(p_default);
		m_options.Add(val);

		return val;
	}	

	public OptionValue LoadOptionBool(string p_key, bool p_default) { 
		OptionValue val = new OptionValue(p_key, "bool");

		val.Load(p_default);
		m_options.Add(val);

		return val;
	}	

	public OptionValue LoadOptionFloat(string p_key, float p_default) { 
		OptionValue val = new OptionValue(p_key, "float");

		val.Load(p_default);
		m_options.Add(val);

		return val;
	}

	public OptionValue LoadOptionString(string p_key, string p_default) { 
		OptionValue val = new OptionValue(p_key, "string");

		val.Load(p_default);
		m_options.Add(val);

		return val;
	}

	void OnSceneLoaded(Scene p_scene, LoadSceneMode p_mode) {
		HideUIOnMouseover.ObjectsHiddenOnMouseover.Clear();
	}
}

public class OptionValue { 
	public string m_key;

	private int m_int;
	private float m_float;
	private string m_string;

	private string m_dataType;

	public OptionValue(string p_key, string p_dataType) { 
		m_key = p_key;
		m_dataType = p_dataType;
	}

	public void Load() { 
		switch(m_dataType) {
			case "int": Load(0); break;
			case "bool": Load(false); break;
			case "float": Load(0f); break;
			case "string": Load(""); break;
			default: break;
		}
	}

	public void Load(int p_default) {
		m_int = PlayerPrefs.GetInt(m_key, p_default);
		m_float = m_int;
		m_string = m_int.ToString();
	}

	public void Load(bool p_default) { 
		Load(p_default ? 1 : 0);
	}

	public void Load(float p_default) { 
		m_float = PlayerPrefs.GetFloat(m_key, p_default);
		m_int = (int) m_float;
		m_string = m_float.ToString();
	}

	public void Load(string p_default) { 
		m_string = PlayerPrefs.GetString(m_key, p_default);
		m_int = 0;
		m_float = 0;
	}

	public int GetInt() { 
		return m_int;
	}

	public bool GetBool() { 
		return m_int == 1;
	}

	public float GetFloat() { 
		return m_float;
	}

	public string GetString() { 
		return m_string;
	}

	public void Save(int p_value) { 
		m_int = p_value;
		m_float = m_int;
		m_string = m_int.ToString();

		PlayerPrefs.SetInt(m_key, m_int);
		PlayerPrefs.Save();
	}

	public void Save(bool p_value) { 
		Save(p_value ? 1 : 0);
	}

	public void Save(float p_value) { 
		m_float = p_value;
		m_int = (int) p_value;
		m_string = p_value.ToString();

		PlayerPrefs.SetFloat(m_key, m_float);
		PlayerPrefs.Save();
	}

	public void Save(string p_value) { 
		m_string = p_value;

		PlayerPrefs.SetString(m_key, p_value);
		PlayerPrefs.Save();
	}
}