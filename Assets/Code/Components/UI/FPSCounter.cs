using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {

	[Tooltip("The text to update with the current FPS")]
	public Text m_text;

	private float m_count;
	
	IEnumerator Start() { 
		while(true) { 
			if(Time.timeScale == 1f) { 
				m_count = 1 / Time.deltaTime;
				m_text.text = "FPS: " + Mathf.Round(m_count);
			}

			yield return new WaitForSeconds(0.5f);
		}
	}
}
