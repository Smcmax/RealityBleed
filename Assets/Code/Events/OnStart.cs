using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class OnStart : MonoBehaviour {

	public UnityEvent m_response;

	void Start() {
		m_response.Invoke();
	}
}
