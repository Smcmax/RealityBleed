using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

[Serializable]
public class Skill {

	[Tooltip("All included skills in the game")]
	public static List<Skill> m_skills = new List<Skill>();

	[Tooltip("All externally loaded skills")]
	public static List<Skill> m_externalSkills = new List<Skill>();

	[Tooltip("Internal and external skills in a single list. Note that if an external and an internal skill have the same name, the external will load over it")]
	public static List<Skill> m_combinedSkills = new List<Skill>();

	[Tooltip("Are we using external skills on top of the internal ones?")]
	public static bool m_useExternalSkills = true; // TODO: change to reflect modded usage instead of a hard true

	[Header("Generic Attributes")]
	[Tooltip("The skill's name")]
	public string m_name;

	[Tooltip("The skill's type (Modifier)")]
	public string m_type;

	[Tooltip("The skill's displayed icon")]
	public SerializableSprite m_icon;

	[Tooltip("Color to use in tooltips")]
	public ColorReference m_nameColor;

	[Tooltip("How much this skill costs to train (to hire someone to train the player)")]
	public int m_sellPrice;

	[Space]
	[Tooltip("How expensive this skill is to train per training level")]
	public List<TrainingLevelIntWrapper> m_trainingExpCosts;

	[Space]
	[Tooltip("The list containing the appropriate description for each training level")]
	public List<DescriptionLevelWrapper> m_descriptions;

	// basically 3 abstract functions, but the class can't be abstract due to the json loading
	public virtual string GetDescription(int p_trainingLevel, bool p_translate) { return ""; }
	public virtual void Use(Entity p_entity, int p_trainingLevel) {}
	public virtual void Remove(Entity p_entity, int p_trainingLevel) {}

	public static void LoadAll() {
		m_skills.Clear();
		m_externalSkills.Clear();
		m_combinedSkills.Clear();

        foreach(TextAsset loadedSkill in Resources.LoadAll<TextAsset>("Skills")) {
            Skill skill = Load(loadedSkill.text);

			skill.m_icon.m_name = "Skills/" + skill.m_icon.m_name;

			if(skill) m_skills.Add(skill);
		}

        List<string> files = new List<string>();
        FileSearch.RecursiveRetrieval(Application.dataPath + "/Data/Skills/", ref files);

		if(files.Count > 0)
			foreach(string file in files) {
				if(file.ToLower().EndsWith(".json")) {
					StreamReader reader = new StreamReader(file);
					Skill skill = Load(reader.ReadToEnd());

                    if(skill) {
                        skill.m_icon.m_name = "Skills/" + skill.m_icon.m_name;
                        skill.m_icon.m_internal = false;

                        m_externalSkills.Add(skill);
                    }

					reader.Close();
				}
			}

		foreach(Skill skill in m_skills) { 
			Skill external = m_externalSkills.Find(s => s.m_name == skill.m_name);

			if(external) m_combinedSkills.Add(external);
			else m_combinedSkills.Add(skill);
		}

		if(m_externalSkills.Count > 0)
			foreach(Skill external in m_externalSkills)
				if(!m_skills.Exists(s => s.m_name == external.m_name))
					m_combinedSkills.Add(external);
	}

	private static Skill Load(string p_json) { 
		Skill skill = JsonUtility.FromJson<Skill>(p_json);
		Type type = null;

        if(!skill) return null;

		switch(skill.m_type.ToLower()) {
			case "modifier": type = typeof(ModifierSkill); break;
		}

		if(type == null) return null;

		return (Skill) JsonUtility.FromJson(p_json, type);
	}

	public static Skill Get(string p_name) { 
		List<Skill> availableSkills = m_useExternalSkills ? m_combinedSkills : m_skills;
		Skill found = availableSkills.Find(s => s.m_name == p_name);

		if(found) return found;
		
		return null;
	}

    public string GetDisplayName() {
        if(m_name.Contains("/")) {
            string[] split = m_name.Split('/');

            return split[split.Length - 1];
        }

        return m_name;
    }

    public static implicit operator bool(Skill p_instance) {
        return p_instance != null;
    }
}

[Serializable]
public class SkillWrapper {
	public string SkillName;
	public bool Learned;
	public int TrainingLevel;

	private Skill LoadedSkill;

	public Skill GetSkill() {
		if(LoadedSkill == null || LoadedSkill.m_name != SkillName)
			LoadedSkill = Skill.Get(SkillName);

		return LoadedSkill;
	}
}