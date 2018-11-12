using System.Collections.Generic;
using System;

[Serializable]
public class BehaviourData {
	private Dictionary<string, float> m_data;

	public BehaviourData() {
		m_data = new Dictionary<string, float>();
	}

	public float Get(string p_key) {
		float value = 0f;

		return m_data.TryGetValue(p_key, out value) ? value : 0f;
	}

	public bool Has(string p_key) {
		return m_data.ContainsKey(p_key);
	}

	public void Clear() {
		m_data.Clear();
	}

	public void Set(string p_key, float p_value) {
		m_data.Remove(p_key);
		m_data.Add(p_key, p_value);
	}
}
