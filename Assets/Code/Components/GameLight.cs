using UnityEngine;
using System;

public class GameLight : MonoBehaviour {

	[Tooltip("The light used when the quality settings want quality shadows")]
	public Light m_quality;

	[Tooltip("Whether or not to allow this light to switch to a more performant lighting type")]
	public bool m_enablePerformanceType;

	[Tooltip("The light used when the quality settings want performing shadows")]
	[ConditionalField("m_enablePerformanceType", "true")] public Light m_performance;

	void Start() {
		if(!m_quality) m_quality = CreateLight("QualityLight");
		if(!m_performance && m_enablePerformanceType) 
			m_performance = CreateLight("PerformanceLight");

		bool quality = true;

		if(m_enablePerformanceType && Game.m_options.Get("Shadows").GetInt() <= 1) 
			quality = false;

		m_quality.gameObject.SetActive(quality);
		if(m_performance) m_performance.gameObject.SetActive(!quality);

		OptionsMenuHandler.Instance.AddGameLight(this);
	}

	public void SwitchQualityModes(bool p_quality) {
		if(m_enablePerformanceType) {
			m_quality.gameObject.SetActive(p_quality);
			if(m_performance) m_performance.gameObject.SetActive(!p_quality);
		}
	}

	private Light CreateLight(string p_name) {
		GameObject obj = new GameObject(p_name);

		obj.transform.SetParent(transform);
		obj.transform.position = transform.position;
		obj.transform.position += new Vector3(0, 0, -1);
		obj.layer = gameObject.layer;

		return obj.AddComponent<Light>();
	}
}
