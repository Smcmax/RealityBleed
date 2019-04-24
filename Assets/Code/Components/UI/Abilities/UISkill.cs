using UnityEngine;
using UnityEngine.UI;

public class UISkill : ClickHandler {

	[Tooltip("The skill represented by the UI")]
	public SkillWrapper m_skill;

	[HideInInspector] public SkillLoader m_loader;

	void OnDisable() { 
		if(m_loader.m_tooltip.gameObject.activeSelf)
			HideTooltip();
	}

	public void ShowTooltip() {
		m_loader.m_tooltip.SetAbilitySkill(new AbilitySkillWrapper(m_skill));
	}

	public void HideTooltip() {
		m_loader.m_tooltip.Hide();
	}
}