using UnityEngine;
using System.Collections.Generic;

public abstract class Skill : ScriptableObject {

	[Header("Generic Attributes")]
	[Tooltip("The skill's name")]
	public string m_name;

	[Tooltip("The skill's displayed icon")]
	public Sprite m_icon;

	[Tooltip("Color to use in tooltips")]
	public ColorReference m_nameColor;

	[Tooltip("Whether or not the effect is activated by the game or by the player")]
	public bool m_isPassive;

	[Tooltip("How much this skill costs to train (to hire someone to train the player)")]
	public int m_sellPrice;

	[Space]
	[Tooltip("How expensive this skill is to train per training level")]
	public List<TrainingLevelIntWrapper> m_trainingExpCosts;

	[Space]
	[Tooltip("The list containing the appropriate description for each training level")]
	public List<DescriptionLevelWrapper> m_descriptions;

	public abstract string GetDescription(int p_trainingLevel);
	public abstract void Use(Entity p_entity, int p_trainingLevel);
	public abstract void Remove(Entity p_entity, int p_trainingLevel);
}

[System.Serializable]
public struct TrainingLevelFloatWrapper {
	public int TrainingLevel;
	public float Value;
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