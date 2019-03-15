using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIAbility : ClickHandler {

	[Tooltip("The ability represented by the UI")]
	public AbilityWrapper m_ability;

	[HideInInspector] public AbilityLoader m_loader;
	[HideInInspector] public Image m_selectionBorder;
	[HideInInspector] public Image m_highlightBorder;

	void Awake() { 
		m_selectionBorder = transform.Find("SelectionBorder").GetComponent<Image>();
		m_highlightBorder = transform.Find("HighlightBorder").GetComponent<Image>();
	}

	void OnDisable() { 
		if(m_loader.m_tooltip.gameObject.activeSelf)
			HideTooltip();
	}

	public void Init() {
		if(m_ability.HotkeySlot > 0)
			m_selectionBorder.gameObject.SetActive(true);
	}

	public void ShowTooltip() {
		m_loader.m_tooltip.SetAbilitySkill(new AbilitySkillWrapper(m_ability));
	}

	public void HideTooltip() {
		m_loader.m_tooltip.Hide();
	}

	protected override void OnAnyClick(GameObject p_clicked, Player p_clicker) {
		if(p_clicker != m_loader.m_entity) return;

		if(m_ability.Learned && !m_loader.m_contextualAbilityMenuObject.activeSelf) {
			m_loader.m_contextualAbilityMenuScript.m_selectedAbility = this;
			MenuHandler.Instance.OpenMenu(m_loader.m_contextualAbilityMenu);
		} else if(m_ability.Learned && m_loader.m_contextualAbilityMenuScript.IsChaining()) { 
			bool added = false;

			if(!m_loader.m_contextualAbilityMenuScript.m_chainedAbilities.Contains(this)) {
				m_loader.m_contextualAbilityMenuScript.m_chainedAbilities.Add(this);
				added = true;
			} else m_loader.m_contextualAbilityMenuScript.m_chainedAbilities.Remove(this);

			m_highlightBorder.gameObject.SetActive(added);
		}
	}
}