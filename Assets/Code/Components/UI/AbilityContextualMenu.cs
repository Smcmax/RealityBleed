using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AbilityContextualMenu : MonoBehaviour {

	[Tooltip("The event to raise when an hotkey is set")]
	public GameEvent m_hotkeySetEvent;

	[HideInInspector] public UIAbility m_selectedAbility;
	[HideInInspector] public List<UIAbility> m_chainedAbilities;

	private Dropdown m_dropdown;
	private Text m_chainAbilitiesText;
	private bool m_chaining;

	public void LoadHotkeys() {
		if(!m_dropdown) m_dropdown = transform.Find("Hotkey Selection").Find("Dropdown").GetComponent<Dropdown>();

		m_dropdown.ClearOptions();
		m_dropdown.options.Add(new Dropdown.OptionData("Not Bound"));

		for(int i = 1; i <= 6; i++) {
			KeyCode hotkeyCode = Game.m_keybinds.GetKeybinds().Find(k => k.m_axis.Key == "Hotkey " + i).m_positiveKey;
			string keyName = "???";

			if(KeyCodeToShortString.Mapping.ContainsKey(hotkeyCode))
				keyName = KeyCodeToShortString.Mapping[hotkeyCode];

			Dropdown.OptionData option = new Dropdown.OptionData(keyName);
			m_dropdown.options.Add(option);
		}

		m_dropdown.value = m_selectedAbility.m_abilitySkill.Ability.HotkeySlot;
		m_dropdown.RefreshShownValue();
	}

	public void SetHotkey() {
		AbilityWrapper existingBind = m_selectedAbility.m_loader.m_entity.m_abilities.Find(a => a.HotkeySlot == m_dropdown.value);

		if(existingBind != null) {
			existingBind.HotkeySlot = 0;
			m_selectedAbility.m_loader.m_loadedAbilities.Find(a => a.m_abilitySkill.Ability == existingBind).m_selectionBorder.gameObject.SetActive(false);
		}

		m_selectedAbility.m_abilitySkill.Ability.HotkeySlot = m_dropdown.value;
		m_selectedAbility.m_selectionBorder.gameObject.SetActive(m_dropdown.value > 0);

		m_hotkeySetEvent.Raise();
	}

	public bool IsChaining() { return m_chaining; }

	public void StartChaining() { 
		if(!m_chainAbilitiesText) m_chainAbilitiesText = transform.Find("ChainAbilities").GetComponentInChildren<Text>();

		m_chainedAbilities = new List<UIAbility>();
		m_chaining = true;
		m_chainAbilitiesText.text = "Save Chain";

		m_selectedAbility.m_highlightBorder.gameObject.SetActive(true);
	}

	public void ToggleChaining(bool p_saveChain) { 
		if(m_chaining) StopChaining(p_saveChain);
		else StartChaining();
	}

	public void StopChaining(bool p_saveChain) { 
		m_chaining = false;
		m_chainAbilitiesText.text = "Chain Abilities";

		m_selectedAbility.m_highlightBorder.gameObject.SetActive(false);

		if(p_saveChain) {
			List<Ability> chained = new List<Ability>();

			foreach(UIAbility ability in m_chainedAbilities) { 
				ability.m_highlightBorder.gameObject.SetActive(false);
				chained.Add(ability.m_abilitySkill.Ability.Ability);
			}

			m_selectedAbility.m_abilitySkill.Ability.ChainedAbilities = chained;
		}
		
		m_chainedAbilities.Clear();
	}
}