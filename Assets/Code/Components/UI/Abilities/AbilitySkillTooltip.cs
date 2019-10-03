using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
		Ability ability = p_wrapper.AbilityWrapper != null ? p_wrapper.AbilityWrapper.GetAbility() : null;
		Skill skill = p_wrapper.SkillWrapper != null ? p_wrapper.SkillWrapper.GetSkill() : null;
		DamageType type = ability != null ? DamageType.Get(ability.m_domain) : null;

		TextMeshProUGUI name = m_modifiableInfo.Find(ti => ti.m_name == "AbilitySkill Name Text")
											   .Get<TextMeshProUGUI>(ref m_panelHeight, ref m_tooltipInfoOffset);
		name.text = Get(p_wrapper.GetDisplayName());
		name.color = ability != null ? type.m_nameColor.Value : skill.m_nameColor.Value;

		if(ability != null) {
            TextMeshProUGUI domain = m_modifiableInfo.Find(ti => ti.m_name == "Domain Text").GetAligned<TextMeshProUGUI>(ref m_tooltipInfoOffset);
			domain.text = Get(ability.m_domain);
			domain.color = type.m_nameColor.Value;

            TextMeshProUGUI active = m_modifiableInfo.Find(ti => ti.m_name == "Active Text")
													 .Get<TextMeshProUGUI>(ref m_panelHeight, ref m_tooltipInfoOffset);
			active.text = Get(ability.m_isPassive ? "Passive" : "Active");
		}

		ShowSeparator(1);

        string prefixColorTag = "<color=#" + ColorUtility.ToHtmlStringRGBA(Constants.YELLOW) + ">";
        string suffixColorTag = "</color>";

        TextMeshProUGUI trainingLevel = m_modifiableInfo.Find(ti => ti.m_name == "Training Level")
														.Get<TextMeshProUGUI>(ref m_panelHeight, ref m_tooltipInfoOffset);
		trainingLevel.text = Game.m_languages.FormatTexts(Get("Training Level: {0}"), prefixColorTag + p_wrapper.GetTrainingLevel().ToString() + suffixColorTag);

		if(p_wrapper.GetTrainingLevel() < p_wrapper.GetMaxTrainingLevel()) {
            TextMeshProUGUI expToNextLevel = m_modifiableInfo.Find(ti => ti.m_name == "Training Exp Next Level")
															 .Get<TextMeshProUGUI>(ref m_panelHeight, ref m_tooltipInfoOffset);
			expToNextLevel.text = Game.m_languages.FormatTexts(Get("EXP to next level: {0}"), 
									prefixColorTag + p_wrapper.GetTrainingExpCost(p_wrapper.GetTrainingLevel() + 1).ToString() + suffixColorTag);
		}

		if(ability != null && p_wrapper.Learned()) {
            TextMeshProUGUI cd = m_modifiableInfo.Find(ti => ti.m_name == "Cooldown").Get<TextMeshProUGUI>(ref m_panelHeight, ref m_tooltipInfoOffset);
			cd.text = Game.m_languages.FormatTexts(Get("{0}s cooldown"), 
						prefixColorTag + p_wrapper.GetCooldown(p_wrapper.GetTrainingLevel()).ToString() + suffixColorTag);
		}

		ShowSeparator(2);

        TextMeshProUGUI sellPrice = m_modifiableInfo.Find(ti => ti.m_name == "Sell Price").Get<TextMeshProUGUI>(ref m_panelHeight, ref m_tooltipInfoOffset);
		sellPrice.text = Game.m_languages.FormatTexts(Get("Sell Price: {0}g"), prefixColorTag + p_wrapper.GetSellPrice() + suffixColorTag);
		sellPrice.color = Constants.WHITE;

		TooltipInfo descInfo = m_modifiableInfo.Find(ti => ti.m_name == "Item Description Text");
        TextMeshProUGUI description = descInfo.Get<TextMeshProUGUI>();

		description.text = p_wrapper.GetDescription(p_wrapper.GetTrainingLevel(), true);
		description.color = Constants.YELLOW;
		m_tooltipInfoOffset += description.rectTransform.rect.y;

		Show(m_panelHeight, true); // activating the description to allow the preferred height to be fetched

		float descPrefHeight = LayoutUtility.GetPreferredHeight(description.rectTransform);
		m_tooltipInfoOffset += descPrefHeight / 2;
		
		description = descInfo.Get<TextMeshProUGUI>(ref m_panelHeight, ref m_tooltipInfoOffset, descPrefHeight);

		Show(m_panelHeight, false); // resizing the panel again to fit
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
	public AbilityWrapper AbilityWrapper;
	public SkillWrapper SkillWrapper;

	public AbilitySkillWrapper(AbilityWrapper p_wrapper) { AbilityWrapper = p_wrapper; }
	public AbilitySkillWrapper(SkillWrapper p_wrapper) { SkillWrapper = p_wrapper; }

	public bool IsEmpty() { return !(AbilityWrapper != null || SkillWrapper != null); }

	public string GetName() { 
        return IsEmpty() ? "" : 
               (AbilityWrapper != null ? AbilityWrapper.AbilityName : 
                                         SkillWrapper.SkillName); 
    }

	public string GetDisplayName() { 
        return IsEmpty() ? "" : 
               (AbilityWrapper != null ? AbilityWrapper.GetAbility().GetDisplayName() :
                                         SkillWrapper.GetSkill().GetDisplayName()); 
    }

    public bool Learned() { 
        return IsEmpty() ? false : 
               (AbilityWrapper != null ? AbilityWrapper.Learned : 
                                         SkillWrapper.Learned); 
    }
	
    public int GetTrainingLevel() { 
        return IsEmpty() ? 0 : 
               (AbilityWrapper != null ? AbilityWrapper.TrainingLevel : 
                                         SkillWrapper.TrainingLevel);
    }
	
    public int GetSellPrice() { 
        return IsEmpty() ? 0 : 
               (AbilityWrapper != null ? AbilityWrapper.GetAbility().m_sellPrice : 
                                         SkillWrapper.GetSkill().m_sellPrice); 
    }
	
    public int GetManaCost(int p_trainingLevel) {
		return IsEmpty() ? 0 :
			(AbilityWrapper != null ?
			AbilityWrapper.GetAbility().m_manaCosts.Find(m => m.TrainingLevel == p_trainingLevel).Value :
			0);
	}

	public float GetCooldown(int p_trainingLevel) {
		return IsEmpty() ? 0 :
			(AbilityWrapper != null ?
			AbilityWrapper.GetAbility().m_cooldowns.Find(m => m.TrainingLevel == p_trainingLevel).Value :
			0);
	}

	public int GetMaxTrainingLevel() {
		return IsEmpty() ? 0 :
			(AbilityWrapper != null ?
			AbilityWrapper.GetAbility().m_trainingExpCosts[AbilityWrapper.GetAbility().m_trainingExpCosts.Count - 1].TrainingLevel :
			SkillWrapper.GetSkill().m_trainingExpCosts[SkillWrapper.GetSkill().m_trainingExpCosts.Count - 1].TrainingLevel);
	}

	public int GetTrainingExpCost(int p_trainingLevel) {
		return IsEmpty() ? 0 :
			(AbilityWrapper != null ?
			AbilityWrapper.GetAbility().m_trainingExpCosts.Find(t => t.TrainingLevel == p_trainingLevel).Value :
			SkillWrapper.GetSkill().m_trainingExpCosts.Find(t => t.TrainingLevel == p_trainingLevel).Value);
	}

	public string GetDescription(int p_trainingLevel, bool p_translate) {
		return IsEmpty() ? "" :
			(AbilityWrapper != null ?
			AbilityWrapper.GetAbility().GetDescription(p_trainingLevel, p_translate) :
			SkillWrapper.GetSkill().GetDescription(p_trainingLevel, p_translate));
	}
}