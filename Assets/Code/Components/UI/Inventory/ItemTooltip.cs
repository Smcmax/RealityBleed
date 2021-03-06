﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ItemTooltip : Tooltip {

	[Tooltip("The panel's border size")]
	public float m_tooltipBorderSize;

	private float m_panelHeight;
	private float m_tooltipInfoOffset;

	public void SetItem(Item p_item) {
		Entity holder = p_item.m_holder ? p_item.m_holder : p_item.m_inventory.m_interactor;
		if(m_modifiableInfo.Count == 0) FillModifiableInfo();

		foreach(TooltipInfo info in m_modifiableInfo)
			info.m_info.SetActive(false);

		m_panelHeight = m_tooltipBorderSize * 2 + 12;
		m_tooltipInfoOffset = -(m_panelHeight / 2);
		BaseItem item = p_item.m_item;

        Show(m_panelHeight, true); // activating the tooltip (out of sight) to allow preferred heights to be fetched

        TextMeshProUGUI name = m_modifiableInfo.Find(ti => ti.m_name == "Item Name Text").Get<TextMeshProUGUI>(ref m_panelHeight, ref m_tooltipInfoOffset);
		name.text = Get(item.GetDisplayName());
		name.color = item.m_nameColor.Value;

		if(item is Armor || item is Weapon) {
            TextMeshProUGUI type = m_modifiableInfo.Find(ti => ti.m_name == "Item Type Text").GetAligned<TextMeshProUGUI>(ref m_tooltipInfoOffset);
			string itemType = "";

			if(item is Armor) itemType = (item as Armor).GetArmorType().ToString();
			if(item is Weapon) itemType = (item as Weapon).GetWeaponType().ToString();

			type.text = Get(itemType);
		}

        TextMeshProUGUI slot = m_modifiableInfo.Find(ti => ti.m_name == "Slot Info Text").Get<TextMeshProUGUI>(ref m_panelHeight, ref m_tooltipInfoOffset);
		slot.text = Get(item.GetSlotInfoText());
		
		ShowSeparator(1);

		bool leftShotPattern = false;
		bool rightShotPattern = false;

		if(item is Weapon) { 
			Weapon weapon = item as Weapon;

			leftShotPattern = !String.IsNullOrEmpty(weapon.m_leftClickPattern) && ShotPattern.Get(weapon.m_leftClickPattern, true) != null;

			if(leftShotPattern) {
				ShowShotPattern(1, ShotPattern.Get(weapon.m_leftClickPattern, true), ref m_panelHeight);
				ShowSeparator(2);

				rightShotPattern = !String.IsNullOrEmpty(weapon.m_rightClickPattern) && ShotPattern.Get(weapon.m_rightClickPattern, true) != null;

				if(rightShotPattern) {
					ShowShotPattern(2, ShotPattern.Get(weapon.m_rightClickPattern, true), ref m_panelHeight);
					ShowSeparator(3);
				}
			}
		}

		if(item is Weapon || item is Armor) { 
			int[] statGains = item is Weapon ? (item as Weapon).m_statGainValues : (item as Armor).m_statGainValues;

			if(statGains.Length == UnitStats.STAT_AMOUNT) {
                TextMeshProUGUI statGainText = m_modifiableInfo.Find(ti => ti.m_name == "Stat Gain Text").Get<TextMeshProUGUI>();
                TextMeshProUGUI statComparisonOneText = m_modifiableInfo.Find(ti => ti.m_name == "Stat Comparison 1").Get<TextMeshProUGUI>();
                TextMeshProUGUI statComparisonTwoText = m_modifiableInfo.Find(ti => ti.m_name == "Stat Comparison 2").Get<TextMeshProUGUI>();
				int[] comparisonOne = item.m_equipmentSlots.Count >= 1 ?
										  BaseItem.GetStatGainDifferences(item, holder.m_equipment, item.m_equipmentSlots[0]) : new int[UnitStats.STAT_AMOUNT];
				int[] comparisonTwo = item.m_equipmentSlots.Count == 2 ? 
										  BaseItem.GetStatGainDifferences(item, holder.m_equipment, item.m_equipmentSlots[1]) : new int[UnitStats.STAT_AMOUNT];
				
				if(p_item.m_inventory == holder.m_equipment) { 
					comparisonOne = new int[UnitStats.STAT_AMOUNT];
					comparisonTwo = new int[UnitStats.STAT_AMOUNT];
				}

				for(int i = 0; i < statGains.Length; ++i) { 
					if(statGains[i] == 0 && comparisonOne[i] == 0 && comparisonTwo[i] == 0) continue;
					if(!m_modifiableInfo.Exists(ti => ti.m_name == ((Stats) i).ToString() + " Gain Text"))
						InstantiateStatText(((Stats) i).ToString() + " Gain Text", statGainText, transform);

                    TextMeshProUGUI statGain = m_modifiableInfo.Find(ti => ti.m_name == ((Stats) i).ToString() + " Gain Text")
															   .Get<TextMeshProUGUI>(ref m_panelHeight, ref m_tooltipInfoOffset);
					if(!m_modifiableInfo.Exists(ti => ti.m_name == ((Stats) i).ToString() + " Comparison 1"))
						InstantiateStatText(((Stats) i).ToString() + " Comparison 1", statComparisonOneText, statGain.transform);
					if(!m_modifiableInfo.Exists(ti => ti.m_name == ((Stats) i).ToString() + " Comparison 2"))
						InstantiateStatText(((Stats) i).ToString() + " Comparison 2", statComparisonTwoText, statGain.transform);

                    TextMeshProUGUI statComparisonOne = m_modifiableInfo.Find(ti => ti.m_name == ((Stats) i).ToString() + " Comparison 1")
																		.Get<TextMeshProUGUI>();
                    TextMeshProUGUI statComparisonTwo = m_modifiableInfo.Find(ti => ti.m_name == ((Stats) i).ToString() + " Comparison 2")
																		.Get<TextMeshProUGUI>();

					statGain.color = statGains[i] > 0 ? Constants.GREEN : (statGains[i] == 0 ? Constants.YELLOW : Constants.RED);
					statGain.text = (statGains[i] > 0 ? "+" : "") + statGains[i] + " " + ((Stats) i).ToString();

					if(comparisonOne[i] != 0) {
						statComparisonOne.color = comparisonOne[i] > 0 ? Constants.GREEN : Constants.RED;
						statComparisonOne.text = "(" + (comparisonOne[i] > 0 ? "+" : "") + comparisonOne[i] + ")";
					} else statComparisonOne.text = "";

					if(comparisonTwo[i] != 0) {
						statComparisonTwo.color = comparisonTwo[i] > 0 ? Constants.GREEN : Constants.RED;
						statComparisonTwo.text = "(" + (comparisonTwo[i] > 0 ? "+" : "") + comparisonTwo[i] + ")";
					} else statComparisonTwo.text = "";
				}
			}
		}

        string prefixColorTag = "<color=#" + ColorUtility.ToHtmlStringRGBA(Constants.YELLOW) + ">";
        string suffixColorTag = "</color>";

		if(item is Weapon || item is Armor) {
            TextMeshProUGUI durability = m_modifiableInfo.Find(ti => ti.m_name == "Durability")
														 .Get<TextMeshProUGUI>(ref m_panelHeight, ref m_tooltipInfoOffset);
			durability.text = Game.m_languages.FormatTexts(Get("Durability: {0}%"), prefixColorTag + p_item.m_durability + suffixColorTag);
			durability.color = Constants.WHITE;
		}

        TextMeshProUGUI sellPrice = m_modifiableInfo.Find(ti => ti.m_name == "Sell Price")
													.Get<TextMeshProUGUI>(ref m_panelHeight, ref m_tooltipInfoOffset);
		sellPrice.text = Game.m_languages.FormatTexts(Get("Sell Price: {0}g"), prefixColorTag + item.m_sellPrice + suffixColorTag);
		sellPrice.color = Constants.WHITE;

		TooltipInfo descInfo = m_modifiableInfo.Find(ti => ti.m_name == "Item Description Text");
        TextMeshProUGUI description = descInfo.Get<TextMeshProUGUI>();

        description.text = "";

        float basePrefHeight = LayoutUtility.GetPreferredHeight(description.rectTransform);

		description.text = Get(item.m_description);
		description.color = Constants.YELLOW;
		m_tooltipInfoOffset += description.rectTransform.rect.y;

        float descPrefHeight = LayoutUtility.GetPreferredHeight(description.rectTransform);
        if(basePrefHeight != descPrefHeight) { // multiline
            m_tooltipInfoOffset += descPrefHeight / 2f;

            description = descInfo.Get<TextMeshProUGUI>(ref m_panelHeight, ref m_tooltipInfoOffset, descPrefHeight);
        } else { 
			description = descInfo.Get<TextMeshProUGUI>(ref m_panelHeight, ref m_tooltipInfoOffset);
			m_panelHeight += basePrefHeight;
		}

		if(leftShotPattern) PositionDamageType(1);
		if(rightShotPattern) PositionDamageType(2);

		Show(m_panelHeight, false); // resizing the panel again to fit and actually showing it
	}

	private void PositionDamageType(int p_shotNumber) {
        TextMeshProUGUI damage = m_modifiableInfo.Find(ti => ti.m_name == "Shot " + p_shotNumber + " Damage").Get<TextMeshProUGUI>();
        Image damageType = m_modifiableInfo.Find(ti => ti.m_name == "Shot " + p_shotNumber + " DamageType").Get<Image>();
        float width = LayoutUtility.GetPreferredWidth(damage.rectTransform);

        damageType.rectTransform.anchoredPosition = new Vector2(53 - (damage.rectTransform.sizeDelta.x - width), 1);
	}

	private void InstantiateStatText(string p_name, TextMeshProUGUI p_original, Transform p_parent) {
        TextMeshProUGUI text = Instantiate(p_original, p_parent);

		text.name = p_name;
		m_modifiableInfo.Add(new TooltipInfo(text.name, text.gameObject, text.GetComponent<RectTransform>()));
	}

	private void ShowSeparator(float p_separatorNumber) {
		m_modifiableInfo.Find(ti => ti.m_name == "Separator " + p_separatorNumber).Get<Image>(ref m_panelHeight, ref m_tooltipInfoOffset);
		m_modifiableInfo.Find(ti => ti.m_name == "Left Stopper " + p_separatorNumber).Get<Image>();
		m_modifiableInfo.Find(ti => ti.m_name == "Right Stopper " + p_separatorNumber).Get<Image>();

		m_panelHeight += 5;
		m_tooltipInfoOffset -= 5;
	}

	private void ShowShotPattern(float p_shotNumber, ShotPattern p_pattern, ref float p_panelHeight) {
		string shot = "Shot " + p_shotNumber;
		string prefixColorTag = "<color=#" + ColorUtility.ToHtmlStringRGBA(Constants.YELLOW) + ">";
		string suffixColorTag = "</color>";

        TextMeshProUGUI cd = m_modifiableInfo.Find(ti => ti.m_name == shot + " CD").GetAligned<TextMeshProUGUI>(ref m_tooltipInfoOffset);
		cd.text = Game.m_languages.FormatTexts(Get("{0}s cooldown"), prefixColorTag + p_pattern.m_patternCooldown.ToString() + suffixColorTag);
		cd.color = Constants.WHITE;
		
		string statColor = ColorUtility.ToHtmlStringRGBA(((Stats) Enum.Parse(typeof(Stats), p_pattern.m_projectileInfo.m_statApplied)).GetColor());
		m_modifiableInfo.Find(ti => ti.m_name == shot + " Label").Get<TextMeshProUGUI>(ref p_panelHeight, ref m_tooltipInfoOffset).text = 
																  Game.m_languages.FormatTexts(Get("Shot {0}"), p_shotNumber.ToString()) + 
																  " (+<color=#" + statColor + ">" + 
																  p_pattern.m_projectileInfo.m_statApplied.ToString() + "</color>)";

		if(p_pattern.m_projectileInfo.m_piercing) {
			m_modifiableInfo.Find(ti => ti.m_name == shot + " Piercing Text").GetAligned<TextMeshProUGUI>(ref m_tooltipInfoOffset).color = Constants.PURPLE;
		}

		m_modifiableInfo.Find(ti => ti.m_name == shot + " Background").GetAligned<Image>(ref m_tooltipInfoOffset);
		Image shotSprite = m_modifiableInfo.Find(ti => ti.m_name == shot + " Sprite").Get<Image>();
		shotSprite.sprite = Projectile.Get(p_pattern.m_projectile).GetComponent<SpriteRenderer>().sprite;

        TextMeshProUGUI shots = m_modifiableInfo.Find(ti => ti.m_name == shot + " Shots").Get<TextMeshProUGUI>(ref p_panelHeight, ref m_tooltipInfoOffset);
		shots.text = Game.m_languages.FormatTexts(Get("Shots: {0}"), prefixColorTag + p_pattern.m_shots.ToString() + suffixColorTag);
		shots.color = Constants.WHITE;

		if(p_pattern.m_projectileInfo.m_armorPiercing) {
			m_modifiableInfo.Find(ti => ti.m_name == shot + " Armor Piercing Text")
							.GetAligned<TextMeshProUGUI>(ref m_tooltipInfoOffset).color = Constants.RED;
		}

        TextMeshProUGUI damage = m_modifiableInfo.Find(ti => ti.m_name == shot + " Damage").Get<TextMeshProUGUI>(ref p_panelHeight, ref m_tooltipInfoOffset);
		damage.text = Game.m_languages.FormatTexts(Get("Damage: {0}"), prefixColorTag + p_pattern.m_projectileInfo.m_damage.ToString() + suffixColorTag);
		damage.color = Constants.WHITE;

		Sprite damageTypeIcon = DamageType.Get(p_pattern.m_projectileInfo.m_damageType).m_icon;

		if(damageTypeIcon) {
			Image damageType = m_modifiableInfo.Find(ti => ti.m_name == shot + " DamageType").Get<Image>();
			damageType.sprite = damageTypeIcon;
		}

        TextMeshProUGUI mana = m_modifiableInfo.Find(ti => ti.m_name == shot + " Mana").GetAligned<TextMeshProUGUI>(ref m_tooltipInfoOffset);
		mana.text = Game.m_languages.FormatTexts(Get("{0} Mana"), p_pattern.m_manaPerStep.ToString());
		mana.color = Constants.MANA_BLUE;

        TextMeshProUGUI range = m_modifiableInfo.Find(ti => ti.m_name == shot + " Range").Get<TextMeshProUGUI>(ref p_panelHeight, ref m_tooltipInfoOffset);
		range.text = Game.m_languages.FormatTexts(Get("Range: {0}"), prefixColorTag + p_pattern.m_projectileInfo.m_range.ToString() + suffixColorTag);
		range.color = Constants.WHITE;

		if(p_pattern.m_extraTooltipInfo.Length > 0) {
			TooltipInfo extraInfo = m_modifiableInfo.Find(ti => ti.m_name == shot + " Extra Text");
            TextMeshProUGUI extra = extraInfo.Get<TextMeshProUGUI>();
			extra.text = "";

            float basePrefHeight = LayoutUtility.GetPreferredHeight(extra.rectTransform);

            extra.text = Get(p_pattern.m_extraTooltipInfo);
            extra.color = Constants.YELLOW;

            float extraPrefHeight = LayoutUtility.GetPreferredHeight(extra.rectTransform);
            if(basePrefHeight != extraPrefHeight) { // multiline
                m_tooltipInfoOffset += extra.rectTransform.rect.y;
                m_tooltipInfoOffset += extraPrefHeight / 1.15f;

				// original 1.7f (pre-tmp)
                extra = extraInfo.Get<TextMeshProUGUI>(ref m_panelHeight, ref m_tooltipInfoOffset, extraPrefHeight * 1.6f);
                m_panelHeight -= extraPrefHeight * 1.6f - extraPrefHeight;
				m_tooltipInfoOffset -= extra.rectTransform.rect.y / 2;
			} else
                extra = extraInfo.Get<TextMeshProUGUI>(ref m_panelHeight, ref m_tooltipInfoOffset);
		}
	}

	private string Get(string p_key) { return Game.m_languages.GetLine(p_key); }
}