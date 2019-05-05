using UnityEngine;
using System.Collections.Generic;

public abstract class Ability : ScriptableObject {

	[Header("Generic Attributes")]
	[Tooltip("The ability's name")]
	public string m_name;

	[Tooltip("The ability's displayed icon")]
	public Sprite m_icon;
	
	[Tooltip("The ability's associated domain, an ability MUST have a domain")]
	public DamageType m_domain;

	[Tooltip("Whether or not the effect is activated by the game or by the player")]
	public bool m_isPassive;

	[Tooltip("How much this ability sells for in shops (base price)")]
	public int m_sellPrice;

	[Space]
	[Tooltip("The cooldown for this ability per training level")]
	public List<TrainingLevelFloatWrapper> m_cooldowns;

	[Space]
	[Tooltip("The mana cost to use this ability per training level")]
	public List<TrainingLevelIntWrapper> m_manaCosts;

	[Space]
	[Tooltip("How expensive this ability is to train per training level")]
	public List<TrainingLevelIntWrapper> m_trainingExpCosts;

	[Space]
	[Tooltip("The list containing the appropriate description for each training level")]
	public List<DescriptionLevelWrapper> m_descriptions;

	public abstract string GetDescription(int p_trainingLevel, bool p_translate);
	public abstract void Use(Entity p_entity, int p_trainingLevel);
}

[System.Serializable]
public struct DescriptionLevelWrapper { 
	public int TrainingLevel;

	[Tooltip("Description, some variables may auto-fill depending on the type described (look at the notes)")]
	[Multiline] public string Description;
}

[System.Serializable]
public struct TrainingLevelIntWrapper { 
	public int TrainingLevel;
	public int Value;
}

public class AbilityWrapper {
	public Ability Ability;
	public bool Learned;
	public int TrainingLevel;
	public int HotkeySlot; // 0 - 6
	public List<Ability> ChainedAbilities;

	private float LastUse;
	
	public float GetLastUseTime() { return LastUse; }

	public bool Use() {
		if(LastUse == 0 || Time.time * 1000 >= LastUse + Ability.m_cooldowns.Find(c => c.TrainingLevel == TrainingLevel).Value * 1000) {
			LastUse = Time.time * 1000;

			return true;
		}

		return false;
	}
}