﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityViewer : MonoBehaviour {

	[Tooltip("The ability's icon")]
	public Image m_icon;

	[Tooltip("The ability's background icon")]
	public Image m_backgroundIcon;

	[Tooltip("The text to update with the hotkey info")]
	public Text m_hotkeyText;

	[Tooltip("The entity which has the ability to view")]
	public Entity m_entity;

	[Tooltip("Hotkey number linked to this viewer, will show the ability linked to this hotkey")]
	public int m_hotkey;

	private AbilityWrapper m_wrapper;

	public void Start() {
		StartCoroutine(UpdateView());
		StartCoroutine(UpdateCooldown());
	}

	public IEnumerator UpdateView() {
		while(true) {
			AbilityWrapper wrapper = m_entity.m_abilities.Find(a => a.HotkeySlot == m_hotkey);

			if(wrapper != null && (m_wrapper == null || wrapper.Ability != m_wrapper.Ability)) {
				m_wrapper = wrapper;

				m_icon.enabled = true;
				m_backgroundIcon.enabled = true;
				m_icon.sprite = m_wrapper.Ability.m_icon;
				m_backgroundIcon.sprite = m_wrapper.Ability.m_icon;
			} else if(wrapper == null) {
				m_wrapper = null;

				m_icon.enabled = false;
				m_backgroundIcon.enabled = false;
			}

			KeyCode hotkeyCode = Game.m_keybinds.GetKeybinds().Find(k => k.m_axis.Key == "Hotkey " + m_hotkey).m_positiveKey;

			if(KeyCodeToShortString.Mapping.ContainsKey(hotkeyCode))
				m_hotkeyText.text = KeyCodeToShortString.Mapping[hotkeyCode].ToUpper();
			else m_hotkeyText.text = "???";

			yield return new WaitForSeconds(Constants.ABILITY_VIEW_REFRESH);
		}
	}

	public IEnumerator UpdateCooldown() { 
		while(true) {
			if(m_wrapper != null) {
				float lastUse = m_wrapper.GetLastUseTime();

				if(lastUse > 0) {
					float cooldown = m_wrapper.Ability.m_cooldowns.Find(c => c.TrainingLevel == m_wrapper.TrainingLevel).Value * 1000;
					float time = Time.time * 1000;

					if(time >= lastUse + cooldown && m_icon.fillAmount != 1) m_icon.fillAmount = 1;
					else if(time < lastUse + cooldown)
						m_icon.fillAmount = 1 - (lastUse + cooldown - time) / cooldown;
				} else m_icon.fillAmount = 1;
			}

			yield return new WaitForSeconds(Constants.ABILITY_COOLDOWN_REFRESH);
		}
	}
}