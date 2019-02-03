using UnityEngine;
using System.Collections.Generic;

public abstract class Skill : ScriptableObject {

	[Header("Generic Attributes")]
	[Tooltip("The skill's name")]
	public string m_name;

	[Tooltip("The skill's displayed icon")]
	public Sprite m_icon;

	[Tooltip("Whether or not the effect is activated by the game or by the player")]
	public bool m_isPassive;

	[Tooltip("How much this skill costs to train (to hire someone to train the player)")]
	public int m_sellPrice;

	[Space]
	[Tooltip("How expensive this skill is to train at first")]
	public List<TrainingLevelIntWrapper> m_trainingExpCosts;

	[Space]
	[Tooltip("The list containing the appropriate description for each training level")]
	public List<DescriptionLevelWrapper> m_descriptions;

	public abstract string GetDescription(int p_trainingLevel);
	public abstract void Use(Entity p_entity, int p_trainingLevel);
}

[System.Serializable]
public struct TrainingLevelFloatWrapper {
	public int TrainingLevel;
	public float Value;
}