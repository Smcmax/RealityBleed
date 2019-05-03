using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextualSliderText : MonoBehaviour {

	[Tooltip("The slider who's value will populate the text")]
	public Slider m_slider;

	[Tooltip("The text to update with the slider's value")]
	public TextMeshProUGUI m_text;

	[Tooltip("The strings to display for each possible value of the slider")]
	public List<ValueTextPair> m_valueTextPairs;

	[Tooltip("The event called whenever the value changes")]
	public UnityIntEvent m_valueChangedEvent;

	[HideInInspector] public int m_value;

	void Start() {
		ValueTextPair pair = FindCurrentPair();
		string currentText = pair.m_text;

		m_slider.value = m_value;
		m_text.text = Game.m_languages.GetLine(currentText);
		m_text.color = Constants.WHITE;
		m_text.fontSize = pair.m_textSize;

		m_slider.onValueChanged.AddListener((value) => {
			m_valueChangedEvent.Invoke((int)value);
			m_value = (int) value;

			ValueTextPair vtp = FindCurrentPair();
			m_text.text = Game.m_languages.GetLine(vtp.m_text);
			m_text.fontSize = vtp.m_textSize;
		});
	}

	public ValueTextPair FindCurrentPair() { 
		if(m_valueTextPairs.Count == 0) return new ValueTextPair(0, "None", 24);
		
		ValueTextPair pair = m_valueTextPairs.Find(vtp => vtp.m_value == m_value);
		
		if(pair == default(ValueTextPair)) return new ValueTextPair(0, "None", 24);
		
		return pair;
	}
}

[Serializable]
public class ValueTextPair { 
	public int m_value;
	public string m_text;
	public int m_textSize;

	public ValueTextPair(int p_value, string p_text, int p_textSize) { 
		m_value = p_value;
		m_text = p_text;
		m_textSize = p_textSize;
	}
}