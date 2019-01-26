using UnityEngine;
using UnityEngine.UI;

public class StatTextUpdater : MonoBehaviour {

	[Tooltip("The text to update using the stat's current color")]
	public Text m_statNameText;

	[Tooltip("The text to update using the stat's current value")]
	public Text m_statText;

	[Tooltip("The text to update using the stat's modifiers")]
	public Text m_modifierText;

	[Tooltip("Whether or not the maximal value should be shown instead of the stat's current value")]
	public bool m_showMaxValue;

	[Tooltip("Which entity's stat are we filling in here?")]
	public Entity m_entity;

	[Tooltip("The stat to update in this text box")]
	public Stats m_stat;

	public void Awake() {
		UpdateText();
	}

	public void UpdateText() {
		int stat = m_showMaxValue ? m_entity.m_stats.GetBaseStatWithGear(m_stat) : m_entity.m_stats.GetStat(m_stat);
		int modifier = (m_showMaxValue ? 0 : m_entity.m_stats.GetModifier(m_stat)) + m_entity.m_stats.GetGearModifier(m_stat);
		string sign = modifier > 0 ? "+" : "";
		Color signColor = modifier > 0 ? Constants.GREEN : (modifier == 0 ? Constants.YELLOW : Constants.RED);

		m_statNameText.color = m_stat.GetColor();
		m_statText.text = stat.ToString();
		m_modifierText.text = "(" + sign + modifier + ")";
		m_modifierText.color = signColor;
	}
}