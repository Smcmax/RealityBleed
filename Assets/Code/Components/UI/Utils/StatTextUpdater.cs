using UnityEngine;
using TMPro;

public class StatTextUpdater : MonoBehaviour {

	[Tooltip("The text to update using the stat's current color")]
	public TextMeshProUGUI m_statNameText;

	[Tooltip("The text to update using the stat's current value")]
	public TextMeshProUGUI m_statText;

	[Tooltip("Whether or not the maximal value should be shown instead of the stat's current value")]
	public bool m_showMaxValue;

	[Tooltip("Whether or not the stat modifier should be appended to the value")]
	public bool m_showModifier;

	[Tooltip("Which entity's stat are we filling in here? If null, getting the menu handling player's stats")]
	public Entity m_entity;

	[Tooltip("The stat to update in this text box")]
	public Stats m_stat;

	public void OnEnable() {
		if(!m_entity && MenuHandler.Instance) m_entity = Player.GetPlayerFromId(MenuHandler.Instance.m_handlingPlayer.id);

		UpdateText();
	}

	public void UpdateText() {
		if(!m_entity || !m_entity.m_stats) return;

		int stat = m_showMaxValue ? m_entity.m_stats.GetBaseStatWithGear(m_stat) : m_entity.m_stats.GetStat(m_stat);
		int modifier = (m_showMaxValue ? 0 : m_entity.m_stats.GetModifier(m_stat)) + 
											 m_entity.m_stats.GetGearModifier(m_stat) + 
											 m_entity.m_stats.GetExternalModifiers(m_stat);
		string sign = modifier > 0 ? "+" : "";
		Color signColor = modifier > 0 ? Constants.GREEN : (modifier == 0 ? Constants.YELLOW : Constants.RED);

		m_statText.text = stat.ToString() + (m_showModifier ? 
												" <color=#" + 
													ColorUtility.ToHtmlStringRGBA(signColor) + 
													">(" + sign + modifier + ")</color>" 
												: "");

		if(m_statNameText) m_statNameText.color = m_stat.GetColor();
	}
}