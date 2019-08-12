using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject {

	private readonly List<GameEventListener> m_listeners = new List<GameEventListener>();

	public void Raise() { 
		Raise(new DataHolder());
	}

	public void Raise(DataHolder p_data) {
		Debug.Log(this.name);
		// counting down to avoid list removal errors
		for(int i = m_listeners.Count - 1; i >= 0; i--)
			m_listeners[i].OnEventRaised(p_data);
	}

	public void RegisterListener(GameEventListener p_listener) {
		if(!m_listeners.Contains(p_listener))
			m_listeners.Add(p_listener);
	}

	public void UnregisterListener(GameEventListener p_listener) {
		if(m_listeners.Contains(p_listener))
			m_listeners.Remove(p_listener);
	}
}