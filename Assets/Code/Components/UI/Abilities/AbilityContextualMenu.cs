using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AbilityContextualMenu : MonoBehaviour {

    [Tooltip("The object to auto-focus whenever this menu closes")]
    public AutoSelectObject m_focusOnClose;

	[Tooltip("The event to raise when an hotkey is set")]
	public GameEvent m_hotkeySetEvent;

	[HideInInspector] public UIAbility m_selectedAbility;
	[HideInInspector] public List<UIAbility> m_chainedAbilities;

	private TMP_Dropdown m_dropdown;
	private TextMeshProUGUI m_chainAbilitiesText;
	private bool m_chaining;

	void OnEnable() {
		if(!m_dropdown) m_dropdown = transform.Find("Hotkey Selection").Find("Dropdown").GetComponent<TMP_Dropdown>();

		m_dropdown.ClearOptions();
		m_dropdown.options.Add(new TMP_Dropdown.OptionData(Game.m_languages.GetLine("Unbound")));

		for(int i = 1; i <= 6; i++) {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(Game.m_languages.FormatTexts(Game.m_languages.GetLine("Hotkey {0}"), i.ToString()));
			m_dropdown.options.Add(option);
		}

		m_dropdown.value = m_selectedAbility.m_ability.HotkeySlot;
		m_dropdown.RefreshShownValue();
	}

    void OnDisable() {
        m_focusOnClose.StartSelectCoroutine();
    }

    public void SetHotkey() {
		AbilityWrapper existingBind = m_selectedAbility.m_loader.m_entity.m_abilities.Find(a => a.HotkeySlot == m_dropdown.value);

		if(existingBind != null) {
			existingBind.HotkeySlot = 0;
			m_selectedAbility.m_loader.m_loadedAbilities.Find(a => a.m_ability == existingBind).m_selectionBorder.gameObject.SetActive(false);
		}

		m_selectedAbility.m_ability.HotkeySlot = m_dropdown.value;
		m_selectedAbility.m_selectionBorder.gameObject.SetActive(m_dropdown.value > 0);

		m_hotkeySetEvent.Raise();
	}

	public bool IsChaining() { return m_chaining; }

	public void StartChaining() { 
		if(!m_chainAbilitiesText) m_chainAbilitiesText = transform.Find("ChainAbilities").GetComponentInChildren<TextMeshProUGUI>();

		m_chainedAbilities = new List<UIAbility>();
		m_chaining = true;
		m_chainAbilitiesText.text = Game.m_languages.GetLine("Save Chain");

		m_selectedAbility.m_highlightBorder.gameObject.SetActive(true);
	}

	public void ToggleChaining(bool p_saveChain) { 
		if(m_chaining) StopChaining(p_saveChain);
		else StartChaining();
	}

	public void StopChaining(bool p_saveChain) { 
		m_chaining = false;
		m_chainAbilitiesText.text = Game.m_languages.GetLine("Chain Abilities");

		m_selectedAbility.m_highlightBorder.gameObject.SetActive(false);

		if(p_saveChain) {
			List<string> chained = new List<string>();

			foreach(UIAbility ability in m_chainedAbilities) { 
				ability.m_highlightBorder.gameObject.SetActive(false);
				chained.Add(ability.m_ability.AbilityName);
			}

			m_selectedAbility.m_ability.ChainedAbilities = chained;
		}
		
		m_chainedAbilities.Clear();
	}
}