using UnityEngine;
using UnityEngine.UI;

public class SliderUpdater : MonoBehaviour {

	[Tooltip("The slider to update using the values given below")]
	public Slider m_slider;

	[Tooltip("The main value's reference")]
	public FloatVariable m_value;

	[Tooltip("The maximum value possible")]
	public FloatVariable m_maxValue;

	public void OnEnable() {
		UpdateSlider();
	}

	public void UpdateSlider() {
		m_slider.minValue = 0;
		m_slider.maxValue = m_maxValue.Value;
		m_slider.value = m_value.Value;
	}
}