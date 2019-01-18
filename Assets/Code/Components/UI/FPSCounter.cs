using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {

	[Tooltip("The text to update with the current FPS")]
	public Text m_text;

	private float m_startTime;
	private int m_frameCount;

	void Start() { 
		m_startTime = Time.time * 1000;
	}

	void Update() { 
		if(Time.timeScale == 0f) return;

		float timeDiff = Time.time * 1000 - m_startTime;

		if(timeDiff >= 500) {
			m_text.text = "FPS: " + m_frameCount * 2;
			m_frameCount = 0;
			m_startTime = Time.time * 1000;
		}
		
		m_frameCount++;
	}
}