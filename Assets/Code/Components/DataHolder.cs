using System.Collections.Generic;
using System;

[Serializable]
public class DataHolder {
	private Dictionary<string, object> m_data;

	public DataHolder() {
		m_data = new Dictionary<string, object>();
	}

	public object Get(string p_key) {
		object value;

		return m_data.TryGetValue(p_key, out value) ? value : null;
	}

	public bool Has(string p_key) {
		return m_data.ContainsKey(p_key);
	}

	public void Clear() {
		m_data.Clear();
	}

	public void Set(string p_key, object p_value) {
		m_data.Remove(p_key);
		m_data.Add(p_key, p_value);
	}
}
