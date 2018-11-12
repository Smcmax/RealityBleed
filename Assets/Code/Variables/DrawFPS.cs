using UnityEngine;
using UnityEngine.UI;

public class DrawFPS : MonoBehaviour {

	public Text m_text;
	
	// Update is called once per frame
	void Update () {
		m_text.text = "FPS: " + (int) (1.0f / Time.deltaTime);
	}
}
