using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class ItemTooltip : MonoBehaviour {

	[Tooltip("The canvas used by this tooltip. It moves with the mouse, so be careful!")]
	public Canvas m_canvas;

	[Tooltip("The panel's border size")]
	public float m_tooltipBorderSize;

	[Tooltip("A dictionary assigning a name to each text field, make sure the name matches in the code")]
	public List<TooltipInfo> m_modifiableInfo;

	[Tooltip("The percentage of the screen's worth of offset applied to the tooltip's position. 0-100")]
	public Vector2 m_offsetPercentage;

	private RectTransform m_rectTransform;
	private RectTransform m_canvasRect;
	private float m_panelHeight;
	private float m_tooltipInfoOffset;

	void Awake() { 
		m_rectTransform = GetComponent<RectTransform>();
		m_canvasRect = m_canvas.GetComponent<RectTransform>();
	}

	void Update() {
		Vector2 mouse = Input.mousePosition;
		Vector2 adjustedMouse = new Vector2((mouse.x / (float) Screen.width) * m_canvasRect.sizeDelta.x, 
													  (mouse.y / (float) Screen.height) * m_canvasRect.sizeDelta.y);
		float tooltipWorldWidth = (m_rectTransform.sizeDelta.x / m_canvasRect.sizeDelta.x) * Screen.width;
		float tooltipWorldHeight = (m_rectTransform.sizeDelta.y / m_canvasRect.sizeDelta.y) * Screen.height;
		float offsetPercentX = m_offsetPercentage.x / 100f;
		float offsetPercentY = m_offsetPercentage.y / 100f;

		// if the tooltip is too far to the right, shift it to cursor's left side
		if(mouse.x + (Screen.width * offsetPercentX) + tooltipWorldWidth / 2 > Screen.width) 
			offsetPercentX = -offsetPercentX;

		// just don't change the canvas' reference res please
		float calcX = adjustedMouse.x + m_canvasRect.sizeDelta.x * offsetPercentX * (1065f / m_canvasRect.sizeDelta.x);
		float calcY = adjustedMouse.y + m_canvasRect.sizeDelta.y * offsetPercentY;

		// if the tooltip is too far down, cap it to the bottom of the screen
		if(mouse.y + (Screen.height * offsetPercentY) - tooltipWorldHeight / 2 < 0)
			calcY = m_rectTransform.sizeDelta.y / 2;

		// if the tooltip is too far up, cap it to the top of the screen
		if(mouse.y + (Screen.height * offsetPercentY) + tooltipWorldHeight / 2 > Screen.height)
			calcY = m_canvasRect.sizeDelta.y - m_rectTransform.sizeDelta.y / 2;

		m_rectTransform.anchoredPosition = new Vector3(calcX, calcY);
	}

	public void Show() {
		m_canvas.gameObject.SetActive(true);
		gameObject.SetActive(true);

		m_rectTransform.sizeDelta = new Vector2(m_rectTransform.sizeDelta.x, m_panelHeight);
	}

	public void Hide() {
		gameObject.SetActive(false);
		m_canvas.gameObject.SetActive(false);

		if(m_rectTransform) 
			m_rectTransform.anchoredPosition = new Vector3(-5000, -5000, 0); // throw it out of the screen to avoid flashing...
	}

	private void FillModifiableInfo() { 
		foreach(Transform child in GetComponentsInChildren<Transform>())
			if(m_modifiableInfo.Find(ti => ti.m_name == child.name) == null)
				m_modifiableInfo.Add(new TooltipInfo(child.name, child.gameObject, child.GetComponent<RectTransform>()));
	}

	public void SetItem(Item p_item) {
		Entity holder = p_item.m_holder ? p_item.m_holder : p_item.m_inventory.m_interactor;
		if(m_modifiableInfo.Count == 0) FillModifiableInfo();

		foreach(TooltipInfo info in m_modifiableInfo)
			info.m_info.SetActive(false);

		m_panelHeight = m_tooltipBorderSize * 2 + 12;
		m_tooltipInfoOffset = -(m_panelHeight / 2);
		BaseItem item = p_item.m_item;

		Text name = m_modifiableInfo.Find(ti => ti.m_name == "Item Name Text").Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset);
		name.text = item.m_name;
		name.color = item.m_nameColor;

		if(item is Armor || item is Weapon) {
			Text type = m_modifiableInfo.Find(ti => ti.m_name == "Item Type Text").GetAligned<Text>(ref m_tooltipInfoOffset);
			string itemType = "";

			if(item is Armor) itemType = (item as Armor).m_armorType.ToString();
			if(item is Weapon) itemType = (item as Weapon).m_weaponType.ToString();

			type.text = itemType;
		}

		Text slot = m_modifiableInfo.Find(ti => ti.m_name == "Slot Info Text").Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset);
		slot.text = item.GetSlotInfoText();
		
		ShowSeparator(1);

		if(item is Weapon) { 
			Weapon weapon = item as Weapon;

			ShowShotPattern(1, weapon.m_leftClickPattern, ref m_panelHeight);
			ShowSeparator(2);

			if(weapon.m_rightClickPattern) {
				ShowShotPattern(2, weapon.m_rightClickPattern, ref m_panelHeight);
				ShowSeparator(3);
			}
		}

		if(item is Weapon || item is Armor) { 
			int[] statGains = item is Weapon ? (item as Weapon).m_statGainValues : (item as Armor).m_statGainValues;

			if(statGains.Length == UnitStats.STAT_AMOUNT) {
				Text statGainText = m_modifiableInfo.Find(ti => ti.m_name == "Stat Gain Text").Get<Text>();
				Text statComparisonOneText = m_modifiableInfo.Find(ti => ti.m_name == "Stat Comparison 1").Get<Text>();
				Text statComparisonTwoText = m_modifiableInfo.Find(ti => ti.m_name == "Stat Comparison 2").Get<Text>();
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

					Text statGain = m_modifiableInfo.Find(ti => ti.m_name == ((Stats) i).ToString() + " Gain Text").Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset);
					if(!m_modifiableInfo.Exists(ti => ti.m_name == ((Stats) i).ToString() + " Comparison 1"))
						InstantiateStatText(((Stats) i).ToString() + " Comparison 1", statComparisonOneText, statGain.transform);
					if(!m_modifiableInfo.Exists(ti => ti.m_name == ((Stats) i).ToString() + " Comparison 2"))
						InstantiateStatText(((Stats) i).ToString() + " Comparison 2", statComparisonTwoText, statGain.transform);

					Text statComparisonOne = m_modifiableInfo.Find(ti => ti.m_name == ((Stats) i).ToString() + " Comparison 1").Get<Text>();
					Text statComparisonTwo = m_modifiableInfo.Find(ti => ti.m_name == ((Stats) i).ToString() + " Comparison 2").Get<Text>();

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

		if(item is Weapon || item is Armor) {
			m_modifiableInfo.Find(ti => ti.m_name == "Durability Label").Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset);
			Text durability = m_modifiableInfo.Find(ti => ti.m_name == "Current Durability").Get<Text>();
			durability.text = p_item.m_durability + "%";
			durability.color = Constants.YELLOW;
		}

		m_modifiableInfo.Find(ti => ti.m_name == "Sell Price Label").Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset);
		Text sellPrice = m_modifiableInfo.Find(ti => ti.m_name == "Sell Price").Get<Text>();
		sellPrice.text = item.m_sellPrice + "g";
		sellPrice.color = Constants.YELLOW;

		TooltipInfo descInfo = m_modifiableInfo.Find(ti => ti.m_name == "Item Description Text");
		Text description = descInfo.Get<Text>();

		description.text = item.m_description;
		description.color = Constants.YELLOW;
		m_tooltipInfoOffset += description.rectTransform.rect.y;

		Show(); // activating the description to allow the preferred height to be fetched

		float descPrefHeight = LayoutUtility.GetPreferredHeight(description.rectTransform);
		m_tooltipInfoOffset += descPrefHeight / 2;
		
		description = descInfo.Get<Text>(ref m_panelHeight, ref m_tooltipInfoOffset, descPrefHeight);

		Show(); // resizing the panel again to fit
	}

	private void InstantiateStatText(string p_name, Text p_original, Transform p_parent) {
		Text text = Instantiate(p_original, p_parent);

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
		m_modifiableInfo.Find(ti => ti.m_name == shot + " CD Label").GetAligned<Text>(ref m_tooltipInfoOffset);
		Text cd = m_modifiableInfo.Find(ti => ti.m_name == shot + " CD").Get<Text>();
		cd.text = p_pattern.m_patternCooldown + "s";
		cd.color = Constants.YELLOW;

		m_modifiableInfo.Find(ti => ti.m_name == shot + " Label").Get<Text>(ref p_panelHeight, ref m_tooltipInfoOffset).text = "Shot 1 (+" + p_pattern.m_projectileInfo.m_statApplied.ToString() + ")";

		if(p_pattern.m_projectileInfo.m_piercing) {
			m_modifiableInfo.Find(ti => ti.m_name == shot + " Piercing Text").GetAligned<Text>(ref m_tooltipInfoOffset).color = Constants.PURPLE;
		}

		m_modifiableInfo.Find(ti => ti.m_name == shot + " Background").GetAligned<Image>(ref m_tooltipInfoOffset);
		Image shotSprite = m_modifiableInfo.Find(ti => ti.m_name == shot + " Sprite").Get<Image>();
		shotSprite.sprite = p_pattern.m_projectile.GetComponent<SpriteRenderer>().sprite;

		m_modifiableInfo.Find(ti => ti.m_name == shot + " Shots Label").Get<Text>(ref p_panelHeight, ref m_tooltipInfoOffset);
		Text shots = m_modifiableInfo.Find(ti => ti.m_name == shot + " Shots").Get<Text>();
		shots.text = p_pattern.m_shots.ToString();
		shots.color = Constants.YELLOW;

		if(p_pattern.m_projectileInfo.m_armorPiercing) {
			m_modifiableInfo.Find(ti => ti.m_name == shot + " Armor Piercing Text").GetAligned<Text>(ref m_tooltipInfoOffset).color = Constants.RED;
		}

		m_modifiableInfo.Find(ti => ti.m_name == shot + " Damage Label").Get<Text>(ref p_panelHeight, ref m_tooltipInfoOffset);
		Text damage = m_modifiableInfo.Find(ti => ti.m_name == shot + " Damage").Get<Text>();
		damage.text = p_pattern.m_projectileInfo.m_damage.ToString();
		damage.color = Constants.YELLOW;

		if(p_pattern.m_projectileInfo.m_damageType.m_icon) {
			m_modifiableInfo.Find(ti => ti.m_name == shot + " Damage").m_rect.anchoredPosition = new Vector2(48, 0);

			Image damageType = m_modifiableInfo.Find(ti => ti.m_name == shot + " DamageType").Get<Image>();
			damageType.sprite = p_pattern.m_projectileInfo.m_damageType.m_icon;
		} else m_modifiableInfo.Find(ti => ti.m_name == shot + " Damage").m_rect.anchoredPosition = new Vector2(36, 0);

		m_modifiableInfo.Find(ti => ti.m_name == shot + " Mana Label").GetAligned<Text>(ref m_tooltipInfoOffset);
		Text mana = m_modifiableInfo.Find(ti => ti.m_name == shot + " Mana").Get<Text>();
		mana.text = p_pattern.m_manaPerStep.ToString();
		mana.color = Constants.MANA_BLUE;

		m_modifiableInfo.Find(ti => ti.m_name == shot + " Range Label").Get<Text>(ref p_panelHeight, ref m_tooltipInfoOffset);
		Text range = m_modifiableInfo.Find(ti => ti.m_name == shot + " Range").Get<Text>();
		range.text = p_pattern.m_projectileInfo.m_range.ToString();
		range.color = Constants.YELLOW;

		Text extra = m_modifiableInfo.Find(ti => ti.m_name == shot + " Extra Text").Get<Text>(ref p_panelHeight, ref m_tooltipInfoOffset);
		extra.text = p_pattern.m_extraTooltipInfo;
		extra.color = Constants.YELLOW;
	}
}

// unity doesn't show dictionaries in the inspector, so this works as such
[Serializable]
public class TooltipInfo { 
	public string m_name;
	public GameObject m_info;
	public RectTransform m_rect;

	public TooltipInfo(string p_name, GameObject p_info, RectTransform p_rect) { 
		m_name = p_name;
		m_info = p_info;
		m_rect = p_rect;
	}

	public T Get<T>() {
		m_info.SetActive(true);

		return m_info.GetComponent<T>();
	}

	public T GetAligned<T>(ref float p_tooltipInfoOffset) {
		m_info.SetActive(true);
		m_rect.anchoredPosition = new Vector2(m_rect.anchoredPosition.x, p_tooltipInfoOffset - m_rect.rect.height / 2);

		return m_info.GetComponent<T>();
	}

	// only use this if adding the object's height into the panel size
	public T Get<T>(ref float p_totalHeight, ref float p_tooltipInfoOffset) { 
		return Get<T>(ref p_totalHeight, ref p_tooltipInfoOffset, m_rect.rect.height);
	}

	public T Get<T>(ref float p_totalHeight, ref float p_tooltipInfoOffset, float p_rectHeight) {
		float newY = p_tooltipInfoOffset - p_rectHeight / 2;

		if(m_info.name.Contains("Separator")) newY -= 2;

		m_rect.anchoredPosition = new Vector2(m_rect.anchoredPosition.x, newY);
		p_totalHeight += p_rectHeight;
		p_tooltipInfoOffset -= p_rectHeight;
		m_info.SetActive(true);

		return m_info.GetComponent<T>();
	}
}