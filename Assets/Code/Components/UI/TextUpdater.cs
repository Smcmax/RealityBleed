using UnityEngine;
using UnityEngine.UI;

public class TextUpdater : MonoBehaviour {

	[Tooltip("The text to update using the values given below")]
	public Text m_text;

	[Tooltip("The main value's reference")]
	public FloatVariable m_value;

	[Tooltip("The maximum value possible, if null, only the value will be shown")]
	public FloatVariable m_maxValue;

	public void OnEnable() {
		UpdateText();
	}

	public void UpdateText() {
		m_text.text = m_value.Value + (m_maxValue ? "/" + m_maxValue.Value : "");
	}
}