using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIAbility : ClickHandler {

	[Tooltip("The ability or skill represented by the UI")]
	public AbilitySkillWrapper m_abilitySkill;

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
		if(m_abilitySkill.Ability.Ability && m_abilitySkill.Ability.HotkeySlot > 0)
			m_selectionBorder.gameObject.SetActive(true);
	}

	public void ShowTooltip() {
		if(m_abilitySkill.IsEmpty()) return;

		m_loader.m_tooltip.SetAbilitySkill(m_abilitySkill);
	}

	public void HideTooltip() {
		if(m_abilitySkill.IsEmpty()) return;

		m_loader.m_tooltip.Hide();
	}

	protected override void OnAnyClick(GameObject p_clicked) { 
		if(m_abilitySkill.Ability.Ability && m_abilitySkill.Learned() && !m_loader.m_contextualAbilityMenu.gameObject.activeSelf) {
			m_loader.m_contextualAbilityMenu.m_selectedAbility = this;
			m_loader.m_contextualAbilityMenu.gameObject.SetActive(true);
			m_loader.m_contextualAbilityMenu.LoadHotkeys();
		} else if(m_abilitySkill.Ability.Ability && m_abilitySkill.Learned() && m_loader.m_contextualAbilityMenu.IsChaining()) { 
			bool added = false;

			if(!m_loader.m_contextualAbilityMenu.m_chainedAbilities.Contains(this)) {
				m_loader.m_contextualAbilityMenu.m_chainedAbilities.Add(this);
				added = true;
			} else m_loader.m_contextualAbilityMenu.m_chainedAbilities.Remove(this);

			m_highlightBorder.gameObject.SetActive(added);
		}
	}
}

public class AbilityWrapper {
	public Ability Ability;
	public bool Learned;
	public int TrainingLevel;
	public int HotkeySlot; // 1 - 6
	public List<Ability> ChainedAbilities;

	private float LastUse;
	private Entity Owner;

	public Entity GetOwner() { return Owner; }
	public void SetOwner(Entity p_entity) { Owner = p_entity; }
	public float GetLastUseTime() { return LastUse; }

	public bool Use() { 
		if(LastUse == 0 || Time.time * 1000 >= LastUse + Ability.m_cooldowns.Find(c => c.TrainingLevel == TrainingLevel).Value * 1000) {
			LastUse = Time.time * 1000;

			return true;
		}

		return false;
	}
}

[System.Serializable]
public class SkillWrapper {
	public Skill Skill;
	public bool Learned;
	public int TrainingLevel;

	private Entity Owner;

	public Entity GetOwner() { return Owner; }
	public void SetOwner(Entity p_entity) { Owner = p_entity; }
}

public class AbilitySkillWrapper { 
	public AbilityWrapper Ability;
	public SkillWrapper Skill;

	public bool IsEmpty() { return !(Ability.Ability || Skill.Skill); }
	public string GetName() { return IsEmpty() ? "" : (Ability.Ability ? Ability.Ability.m_name : Skill.Skill.m_name); }
	public bool Learned() { return IsEmpty() ? false : (Ability.Ability ? Ability.Learned : Skill.Learned); }
	public int GetTrainingLevel() { return IsEmpty() ? 0 : (Ability.Ability ? Ability.TrainingLevel : Skill.TrainingLevel); }
	public int GetSellPrice() { return IsEmpty() ? 0 : (Ability.Ability ? Ability.Ability.m_sellPrice : Skill.Skill.m_sellPrice); }
	public int GetManaCost(int p_trainingLevel) { return IsEmpty() ? 0 : 
			(Ability.Ability ? 
			Ability.Ability.m_manaCosts.Find(m => m.TrainingLevel == p_trainingLevel).Value : 
			0); 
	}

	public float GetCooldown(int p_trainingLevel) { return IsEmpty() ? 0 :
			(Ability.Ability ?
			Ability.Ability.m_cooldowns.Find(m => m.TrainingLevel == p_trainingLevel).Value :
			0);
	}

	public int GetMaxTrainingLevel() { return IsEmpty() ? 0 : 
			(Ability.Ability ? 
			Ability.Ability.m_trainingExpCosts[Ability.Ability.m_trainingExpCosts.Count - 1].TrainingLevel
			: Skill.Skill.m_trainingExpCosts[Skill.Skill.m_trainingExpCosts.Count - 1].TrainingLevel); 
	}

	public int GetTrainingExpCost(int p_trainingLevel) { return IsEmpty() ? 0 :
			(Ability.Ability ?
			Ability.Ability.m_trainingExpCosts.Find(t => t.TrainingLevel == p_trainingLevel).Value :
			Skill.Skill.m_trainingExpCosts.Find(t => t.TrainingLevel == p_trainingLevel).Value);
	}

	public string GetDescription(int p_trainingLevel) { return IsEmpty() ? "" : 
			(Ability.Ability ? 
			Ability.Ability.GetDescription(p_trainingLevel) : 
			Skill.Skill.GetDescription(p_trainingLevel)); 
	}
}