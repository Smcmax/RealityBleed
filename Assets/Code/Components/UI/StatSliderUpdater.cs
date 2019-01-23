using UnityEngine;
using UnityEngine.UI;

public class StatSliderUpdater : MonoBehaviour {

	[Tooltip("The slider to update using the values given below")]
	public Slider m_slider;

	[Tooltip("Which entity's stat are we filling in here?")]
	public Entity m_entity;

	[Tooltip("The stat to update in this slider")]
	public Stats m_stat;

	public void Awake() {
		UpdateSlider();
	}

	public void UpdateSlider() {
		if(!m_entity) return;

		m_slider.minValue = 0;

		// HP/MP work in reduction of the base stat, so the modifiers bring it closer and closer to 0
		m_slider.maxValue = m_entity.m_stats.GetBaseStatWithGear(m_stat);
		m_slider.value = m_entity.m_stats.GetStat(m_stat);
	}
}