using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentSetManager : MonoBehaviour {

    [Tooltip("All stored persistent sets, note that these do not persist past game closes")]
    public Dictionary<string, IList> m_sets;

    void Awake() {
        m_sets = new Dictionary<string, IList>();
    }

    public IList Get(string p_name) {
        if(m_sets.ContainsKey(p_name)) return m_sets[p_name];

        return new List<object>();
    }

    public bool Contains(string p_name) {
        return m_sets.ContainsKey(p_name);
    }

    public void Set(string p_name, IList p_list) {
        m_sets[p_name] = p_list;
    }
}
