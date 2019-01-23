using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class GameEventListener : MonoBehaviour {

	[Tooltip("Event to register to")]
	public GameEvent m_event;

	[Tooltip("How long to wait before invoking the response")]
	[Range(0, 2.5f)] public float m_responseDelay;

	public UnityEvent m_response;

	protected virtual void OnEnable() { 
		m_event.RegisterListener(this);
	}

	protected virtual void OnDisable() { 
		m_event.UnregisterListener(this);
	}

	public void OnEventRaised() { 
		if(m_responseDelay == 0f) m_response.Invoke();
		else StartCoroutine(InvokeLater(m_responseDelay));
	}

	IEnumerator InvokeLater(float p_wait){
		yield return new WaitForSeconds(p_wait);
		m_response.Invoke();
	}
}
