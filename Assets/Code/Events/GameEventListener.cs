using UnityEngine;
using System.Collections;

public class GameEventListener : MonoBehaviour {

	[Tooltip("Event to register to")]
	public GameEvent m_event;

	[Tooltip("How long to wait before invoking the response")]
	[Range(0, 2.5f)] public float m_responseDelay;

	public UnityDataHolderEvent m_response;

	protected virtual void OnEnable() { 
		m_event.RegisterListener(this);
	}

	protected virtual void OnDisable() { 
		m_event.UnregisterListener(this);
	}

	public void OnEventRaised() { 
		OnEventRaised(null);
	}

	public void OnEventRaised(DataHolder p_data) { 
		if(m_responseDelay == 0f) m_response.Invoke(p_data);
		else StartCoroutine(InvokeLater(m_responseDelay, p_data));
	}

	IEnumerator InvokeLater(float p_wait, DataHolder p_data){
		yield return new WaitForSeconds(p_wait);
		m_response.Invoke(p_data);
	}
}
