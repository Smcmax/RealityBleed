using UnityEngine;
using UnityEngine.UI;

public class FadeAwayText : MonoBehaviour {

	[Tooltip("The speed at which the text floats upwards")]
	[Range(0, 2)] public float m_speed;

	[Tooltip("The length of time over which this text fades away")]
	[Range(0, 5)] public float m_maxTime;

	private float m_startTime;

	void Awake() {
		m_startTime = Time.time * 1000;
	}

	void LateUpdate() {
		if(m_maxTime * 1000 + m_startTime < Time.time * 1000) Destroy(gameObject);

		transform.Translate(new Vector3(0, m_speed / 100f));

		float alphaPercentage = ((Time.time * 1000 - m_startTime) / (m_maxTime * 1000));
		Text text = GetComponent<Text>();
		text.color = new Color(text.color.r, text.color.g, text.color.b, 1 - alphaPercentage);
	}
}
