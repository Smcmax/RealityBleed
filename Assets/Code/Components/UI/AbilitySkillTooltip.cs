using UnityEngine;
using UnityEngine.UI;

public class AbilitySkillTooltip : Tooltip {

	[Tooltip("The panel's border size")]
	public float m_tooltipBorderSize;

	private float m_panelHeight;
	private float m_tooltipInfoOffset;

	public void SetAbilitySkill(AbilitySkillWrapper p_wrapper) {
		if(m_modifiableInfo.Count == 0) FillModifiableInfo();

		foreach(TooltipInfo info in m_modifiableInfo)
			info.m_info.SetActive(false);

		m_panelHeight = m_tooltipBorderSize * 2 + 12;
		m_tooltipInfoOffset = -(m_panelHeight / 2);
		Ability ability = p_wrapper.Ability.Ability ? p_wrapper.Ability.Ability : null;
		//Skill skill = p_wrapper.Skill.Skill ? p_wrapper.Skill.Skill : null;

		Text name = m_modifiableInfo.Find(ti => ti.m_name == "AbilitySkill Name Text").Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset);
		name.text = p_wrapper.GetName();
		name.color = ability != null ? ability.m_domain.m_nameColor : Constants.WHITE;

		if(ability != null) {
			Text domain = m_modifiableInfo.Find(ti => ti.m_name == "Domain Text").GetAligned<Text>(ref m_tooltipInfoOffset);
			domain.text = ability.m_domain.m_name;
			domain.color = ability.m_domain.m_nameColor;

			Text active = m_modifiableInfo.Find(ti => ti.m_name == "Active Text").Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset);
			active.text = ability.m_isPassive ? "Passive" : "Active";
		}

		ShowSeparator(1);

		m_modifiableInfo.Find(ti => ti.m_name == "Training Level Label").Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset);
		Text trainingLevel = m_modifiableInfo.Find(ti => ti.m_name == "Training Level").Get<Text>();
		trainingLevel.text = p_wrapper.GetTrainingLevel().ToString();

		if(p_wrapper.GetTrainingLevel() < p_wrapper.GetMaxTrainingLevel()) {
			m_modifiableInfo.Find(ti => ti.m_name == "Training Exp Next Level").Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset);
			Text expToNextLevel = m_modifiableInfo.Find(ti => ti.m_name == "Exp Next Level").Get<Text>();
			expToNextLevel.text = p_wrapper.GetTrainingExpCost(p_wrapper.GetTrainingLevel() + 1).ToString();
		}

		if(ability != null && p_wrapper.Learned()) {
			m_modifiableInfo.Find(ti => ti.m_name == "Cooldown Label").Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset);
			Text cd = m_modifiableInfo.Find(ti => ti.m_name == "Cooldown").Get<Text>();
			cd.text = p_wrapper.GetCooldown(p_wrapper.GetTrainingLevel()).ToString() + "s";
		}

		ShowSeparator(2);

		m_modifiableInfo.Find(ti => ti.m_name == "Sell Price Label").Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset);
		Text sellPrice = m_modifiableInfo.Find(ti => ti.m_name == "Sell Price").Get<Text>();
		sellPrice.text = p_wrapper.GetSellPrice() + "g";
		sellPrice.color = Constants.YELLOW;

		TooltipInfo descInfo = m_modifiableInfo.Find(ti => ti.m_name == "Item Description Text");
		Text description = descInfo.Get<Text>();

		description.text = p_wrapper.GetDescription(p_wrapper.GetTrainingLevel());
		description.color = Constants.YELLOW;
		m_tooltipInfoOffset += description.rectTransform.rect.y;

		Show(m_panelHeight); // activating the description to allow the preferred height to be fetched

		float descPrefHeight = LayoutUtility.GetPreferredHeight(description.rectTransform);
		m_tooltipInfoOffset += descPrefHeight / 2;
		
		description = descInfo.Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset, descPrefHeight);

		Show(m_panelHeight); // resizing the panel again to fit
	}

	private void ShowSeparator(float p_separatorNumber) {
		m_modifiableInfo.Find(ti => ti.m_name == "Separator " + p_separatorNumber).Get<Image>(ref m_panelHeight, ref m_tooltipInfoOffset);
		m_modifiableInfo.Find(ti => ti.m_name == "Left Stopper " + p_separatorNumber).Get<Image>();
		m_modifiableInfo.Find(ti => ti.m_name == "Right Stopper " + p_separatorNumber).Get<Image>();

		m_panelHeight += 5;
		m_tooltipInfoOffset -= 5;
	}
}