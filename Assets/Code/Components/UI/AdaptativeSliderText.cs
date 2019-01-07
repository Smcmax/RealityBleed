using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class AdaptativeSliderText : MonoBehaviour {

	[Tooltip("The slider who's value will populate the text")]
	public Slider m_slider;

	[Tooltip("The text to update with the slider's value")]
	public Text m_text;

	[Tooltip("Thresholds of the slider's value under which the slider will change color")]
	public List<ColorThreshold> m_colorThresholds;

	[Tooltip("The event called whenever the value changes")]
	public UnityFloatEvent m_valueChangedEvent;

	[HideInInspector] public float m_value;
	[HideInInspector] public float m_unlimited;

	void Start() {
		string baseText = m_text.text;
		Color currentColor = FindCurrentColorThreshold().m_color;

		// TODO: optimize
		m_slider.value = m_value != m_unlimited ? m_value : m_slider.maxValue;
		m_text.text = (m_value != m_unlimited ? m_value.ToString() : "Unlimited") + baseText;
		m_text.color = currentColor;

		m_slider.onValueChanged.AddListener((value) => {
			m_valueChangedEvent.Invoke(value);

			m_value = value != m_slider.maxValue ? value : m_unlimited;
			m_text.text = (value != m_slider.maxValue ? value.ToString() : "Unlimited") + baseText;
			m_text.color = FindCurrentColorThreshold().m_color;
		});
	}

	public ColorThreshold FindCurrentColorThreshold() {
		if(m_colorThresholds.Count == 0) return new ColorThreshold(1000, Constants.WHITE);

		float value = m_value == m_unlimited ? m_colorThresholds[m_colorThresholds.Count - 1].m_threshold : m_value;

		ColorThreshold closest = null;

		foreach(ColorThreshold ct in m_colorThresholds) {
			if(ct.m_threshold >= value) 
				if(closest == null || ct.m_threshold < closest.m_threshold) 
					closest = ct;
		}

		if(closest == null) { // over all thresholds
			m_value = m_unlimited;
			return m_colorThresholds[m_colorThresholds.Count - 1];
		}
		
		return closest;
	}
}

[Serializable]
public class ColorThreshold { 
	public int m_threshold;
	public Color m_color;

	public ColorThreshold(int p_threshold, Color p_color) { 
		m_threshold = p_threshold;
		m_color = p_color;
	}
}