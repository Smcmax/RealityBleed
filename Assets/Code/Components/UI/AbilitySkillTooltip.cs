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
		Ability ability = p_wrapper.Ability != null ? p_wrapper.Ability.Ability : null;
		Skill skill = p_wrapper.Skill != null ? p_wrapper.Skill.Skill : null;

		Text name = m_modifiableInfo.Find(ti => ti.m_name == "AbilitySkill Name Text").Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset);
		name.text = Get(p_wrapper.GetName());
		name.color = ability != null ? ability.m_domain.m_nameColor.Value : skill.m_nameColor.Value;

		if(ability != null) {
			Text domain = m_modifiableInfo.Find(ti => ti.m_name == "Domain Text").GetAligned<Text>(ref m_tooltipInfoOffset);
			domain.text = Get(ability.m_domain.m_name);
			domain.color = ability.m_domain.m_nameColor.Value;

			Text active = m_modifiableInfo.Find(ti => ti.m_name == "Active Text").Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset);
			active.text = Get(ability.m_isPassive ? "Passive" : "Active");
		}

		ShowSeparator(1);

        string prefixColorTag = "<color=#" + ColorUtility.ToHtmlStringRGBA(Constants.YELLOW) + ">";
        string suffixColorTag = "</color>";

		Text trainingLevel = m_modifiableInfo.Find(ti => ti.m_name == "Training Level").Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset);
		trainingLevel.text = Game.m_languages.FormatTexts(Get("Training Level: {0}"), prefixColorTag + p_wrapper.GetTrainingLevel().ToString() + suffixColorTag);

		if(p_wrapper.GetTrainingLevel() < p_wrapper.GetMaxTrainingLevel()) {
			Text expToNextLevel = m_modifiableInfo.Find(ti => ti.m_name == "Training Exp Next Level").Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset);
			expToNextLevel.text = Game.m_languages.FormatTexts(Get("EXP to next level: {0}"), 
									prefixColorTag + p_wrapper.GetTrainingExpCost(p_wrapper.GetTrainingLevel() + 1).ToString() + suffixColorTag);
		}

		if(ability != null && p_wrapper.Learned()) {
			Text cd = m_modifiableInfo.Find(ti => ti.m_name == "Cooldown").Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset);
			cd.text = Game.m_languages.FormatTexts(Get("{0}s cooldown"), 
						prefixColorTag + p_wrapper.GetCooldown(p_wrapper.GetTrainingLevel()).ToString() + suffixColorTag);
		}

		ShowSeparator(2);

		Text sellPrice = m_modifiableInfo.Find(ti => ti.m_name == "Sell Price").Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset);
		sellPrice.text = Game.m_languages.FormatTexts(Get("Sell Price: {0}g"), prefixColorTag + p_wrapper.GetSellPrice() + suffixColorTag);
		sellPrice.color = Constants.WHITE;

		TooltipInfo descInfo = m_modifiableInfo.Find(ti => ti.m_name == "Item Description Text");
		Text description = descInfo.Get<Text>();

		description.text = p_wrapper.GetDescription(p_wrapper.GetTrainingLevel(), true);
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

    private string Get(string p_key) { return Game.m_languages.GetLine(p_key); }
}

public class AbilitySkillWrapper {
	public AbilityWrapper Ability;
	public SkillWrapper Skill;

	public AbilitySkillWrapper(AbilityWrapper p_ability) { Ability = p_ability; }
	public AbilitySkillWrapper(SkillWrapper p_skill) { Skill = p_skill; }

	public bool IsEmpty() { return !(Ability != null || Skill != null); }
	public string GetName() { return IsEmpty() ? "" : (Ability != null ? Ability.Ability.m_name : Skill.Skill.m_name); }
	public bool Learned() { return IsEmpty() ? false : (Ability != null ? Ability.Learned : Skill.Learned); }
	public int GetTrainingLevel() { return IsEmpty() ? 0 : (Ability != null ? Ability.TrainingLevel : Skill.TrainingLevel); }
	public int GetSellPrice() { return IsEmpty() ? 0 : (Ability != null ? Ability.Ability.m_sellPrice : Skill.Skill.m_sellPrice); }
	public int GetManaCost(int p_trainingLevel) {
		return IsEmpty() ? 0 :
			(Ability != null ?
			Ability.Ability.m_manaCosts.Find(m => m.TrainingLevel == p_trainingLevel).Value :
			0);
	}

	public float GetCooldown(int p_trainingLevel) {
		return IsEmpty() ? 0 :
			(Ability != null ?
			Ability.Ability.m_cooldowns.Find(m => m.TrainingLevel == p_trainingLevel).Value :
			0);
	}

	public int GetMaxTrainingLevel() {
		return IsEmpty() ? 0 :
			(Ability != null ?
			Ability.Ability.m_trainingExpCosts[Ability.Ability.m_trainingExpCosts.Count - 1].TrainingLevel :
			Skill.Skill.m_trainingExpCosts[Skill.Skill.m_trainingExpCosts.Count - 1].TrainingLevel);
	}

	public int GetTrainingExpCost(int p_trainingLevel) {
		return IsEmpty() ? 0 :
			(Ability != null ?
			Ability.Ability.m_trainingExpCosts.Find(t => t.TrainingLevel == p_trainingLevel).Value :
			Skill.Skill.m_trainingExpCosts.Find(t => t.TrainingLevel == p_trainingLevel).Value);
	}

	public string GetDescription(int p_trainingLevel, bool p_translate) {
		return IsEmpty() ? "" :
			(Ability != null ?
			Ability.Ability.GetDescription(p_trainingLevel, p_translate) :
			Skill.Skill.GetDescription(p_trainingLevel, p_translate));
	}
}