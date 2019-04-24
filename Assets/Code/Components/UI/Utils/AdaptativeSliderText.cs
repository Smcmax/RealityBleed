using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class AdaptativeSliderText : MonoBehaviour {

	[Tooltip("The slider who's value will populate the text")]
	public Slider m_slider;

	[Tooltip("The text to update with the slider's value")]
	public Text m_text;

	[Tooltip("Text shown before the value")]
	public string m_prefixText;

	[Tooltip("Text shown after the value")]
	public string m_suffixText;

	[Tooltip("Should the maximum value be unlimited?")]
	public bool m_unlimitedValueAllowed;

	[Tooltip("Thresholds of the slider's value under which the slider will change color")]
	public List<ColorThreshold> m_colorThresholds;

	[Tooltip("The event called whenever the value changes")]
	public UnityIntEvent m_valueChangedEvent;

	[HideInInspector] public int m_value;
	[HideInInspector] public int m_unlimited;

	void Start() {
		Color currentColor = FindCurrentColorThreshold().m_color;

		m_slider.value = m_value == m_unlimited && m_unlimitedValueAllowed ? m_slider.maxValue : m_value;

		if(m_unlimitedValueAllowed && m_value == m_unlimited) 
			m_text.text = Get("Unlimited" + m_suffixText);
		else m_text.text = Game.m_languages.FormatTexts(Get(m_prefixText + "{0}" + m_suffixText), m_value.ToString());

		m_text.color = currentColor;

		m_slider.onValueChanged.AddListener((value) => {
			UpdateSlider(value);
		});
	}

	public void UpdateSlider() { UpdateSlider(m_value); }

	public void UpdateSlider(float p_value) {
        m_value = (int) p_value == (int) m_slider.maxValue && m_unlimitedValueAllowed ? m_unlimited : (int) p_value;

        if(m_unlimitedValueAllowed && m_value == m_unlimited)
            m_text.text = Get("Unlimited" + m_suffixText);
        else m_text.text = Game.m_languages.FormatTexts(Get(m_prefixText + "{0}" + m_suffixText), m_value.ToString());

        m_text.color = FindCurrentColorThreshold().m_color;
        m_valueChangedEvent.Invoke(m_value);
	}

	public ColorThreshold FindCurrentColorThreshold() {
		if(m_colorThresholds.Count == 0) return new ColorThreshold(1000, Constants.WHITE);

		int value = m_value == m_unlimited ? m_colorThresholds[m_colorThresholds.Count - 1].m_threshold : m_value;

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

	private string Get(string p_string) {
        return Game.m_languages.GetLine(p_string);
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