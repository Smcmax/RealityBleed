using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

[Serializable]
public class Ability {

	[Tooltip("All included abilities in the game")]
	public static List<Ability> m_abilities = new List<Ability>();

	[Tooltip("All externally loaded abilities")]
	public static List<Ability> m_externalAbilities = new List<Ability>();

	[Tooltip("Internal and external abilities in a single list. Note that if an external and an internal ability have the same name, the external will load over it")]
	public static List<Ability> m_combinedAbilities = new List<Ability>();

	[Tooltip("Are we using external abilities on top of the internal ones?")]
	public static bool m_useExternalAbilities = true; // TODO: change to reflect modded usage instead of a hard true

	[Header("Generic Attributes")]
	[Tooltip("The ability's name")]
	public string m_name;

	[Tooltip("The ability's type (ShotPattern)")]
	public string m_type;

	[Tooltip("The ability's displayed icon")]
	public SerializableSprite m_icon;
	
	[Tooltip("The ability's associated domain, an ability MUST have a domain")]
	public string m_domain;

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

	// basically 2 abstract functions, but the class can't be abstract due to the json loading
	public virtual string GetDescription(int p_trainingLevel, bool p_translate) { return ""; }
	public virtual void Use(Entity p_entity, int p_trainingLevel) {}

	public static void LoadAll() {
		m_abilities.Clear();
		m_externalAbilities.Clear();
		m_combinedAbilities.Clear();

        Dictionary<TextAsset, string> abilities = new Dictionary<TextAsset, string>(); // string = subdir path

        List<string> subdirs = JsonUtility.FromJson<Subdirs>(Resources.Load<TextAsset>("Abilities/Subdirs").text).m_subdirs;

        foreach(string subdir in subdirs)
            foreach(TextAsset ability in Resources.LoadAll<TextAsset>("Abilities/" + subdir))
                abilities.Add(ability, subdir);

		foreach(TextAsset loadedAbility in Resources.LoadAll<TextAsset>("Abilities")) {
            if(loadedAbility.name == "Subdirs") continue;

            Ability ability = Load(loadedAbility.text);

			ability.m_icon.m_name = "Abilities/" + abilities[loadedAbility] + "/" + ability.m_icon.m_name;

			if(ability) m_abilities.Add(ability);
		}

		string[] files = Directory.GetFiles(Application.dataPath + "/Data/Abilities/");

		if(files.Length > 0)
			foreach(string file in files) {
				if(file.ToLower().EndsWith(".json")) {
					StreamReader reader = new StreamReader(file);
					Ability ability = Load(reader.ReadToEnd());

                    if(ability) {
                        ability.m_icon.m_name = "Abilities/" + ability.m_icon.m_name;
                        ability.m_icon.m_internal = false;

                        m_externalAbilities.Add(ability);
                    }

					reader.Close();
				}
			}

		foreach(Ability ability in m_abilities) { 
			Ability external = m_externalAbilities.Find(a => a.m_type == ability.m_type);

			if(external) m_combinedAbilities.Add(external);
			else m_combinedAbilities.Add(ability);
		}

		if(m_externalAbilities.Count > 0)
			foreach(Ability external in m_externalAbilities)
				if(!m_abilities.Exists(a => a.m_type == external.m_type))
					m_combinedAbilities.Add(external);
	}

	private static Ability Load(string p_json) { 
		Ability ability = JsonUtility.FromJson<Ability>(p_json);
		Type type = null;

        if(!ability) return null;

		switch(ability.m_type.ToLower()) {
			case "shotpattern": type = typeof(ShotPatternAbility); break;
		}

		if(type == null) return null;

		return (Ability) JsonUtility.FromJson(p_json, type);
	}

	public static Ability Get(string p_name) { 
		List<Ability> availableAbilities = m_useExternalAbilities ? m_combinedAbilities : m_abilities;
		Ability found = availableAbilities.Find(a => a.m_name == p_name);

		if(found) return found;
		
		return null;
	}

    public static implicit operator bool(Ability p_instance) {
        return p_instance != null;
    }
}

[Serializable]
public struct DescriptionLevelWrapper { 
	public int TrainingLevel;

	[Tooltip("Description, some variables may auto-fill depending on the type described")]
	[Multiline] public string Description;
}

[Serializable]
public struct TrainingLevelIntWrapper { 
	public int TrainingLevel;
	public int Value;
}

[Serializable]
public struct TrainingLevelFloatWrapper {
	public int TrainingLevel;
	public float Value;
}

public class AbilityWrapper {
	public string AbilityName;
	public bool Learned;
	public int TrainingLevel;
	public int HotkeySlot; // 0 - 6
	public List<string> ChainedAbilities;

	private Ability LoadedAbility;
	private float LastUse;
	
	public float GetLastUseTime() { return LastUse; }

	public Ability GetAbility() {
		if(LoadedAbility == null || LoadedAbility.m_name != AbilityName)
			LoadedAbility = Ability.Get(AbilityName);

		return LoadedAbility;
	}

	public bool Use() {
		if(LastUse == 0 || Time.time * 1000 >= LastUse + GetAbility().m_cooldowns.Find(c => c.TrainingLevel == TrainingLevel).Value * 1000) {
			LastUse = Time.time * 1000;

			return true;
		}

		return false;
	}
}