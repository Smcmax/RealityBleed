using UnityEngine;
using UnityEngine.UI;

public class FadeAwayText : MonoBehaviour {

	[Tooltip("The speed at which the text floats upwards")]
	[Range(0, 2)] public float m_speed;

	[Tooltip("The length of time over which this text fades away")]
	[Range(0, 5)] public float m_maxTime;

	private float m_startTime;
	private UIWorldSpaceFollower m_follower;
	private Text m_text;

	void Awake() {
		m_startTime = Time.time * 1000;
		m_follower = GetComponent<UIWorldSpaceFollower>();
		m_text = GetComponent<Text>();

		if(m_follower) m_follower.m_freezePosition = m_follower.m_parent.position;
	}

	void LateUpdate() {
		if(Time.timeScale == 0f) return;
		if(m_maxTime * 1000 + m_startTime < Time.time * 1000) Destroy(gameObject);

		Vector3 move = new Vector3(0, m_speed / 100f);

		if(m_follower) m_follower.m_freezePosition += move;
		else transform.Translate(move);

		float alphaPercentage = ((Time.time * 1000 - m_startTime) / (m_maxTime * 1000));
		m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, 1 - alphaPercentage);
	}
}
