using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class NPCType {

    [Tooltip("The name of this type")]
    public string m_type;

    [Tooltip("Percentage chance to give a quest from associated quests")]
    public float m_questPercentage;

	[Tooltip("Only one priority type may be assigned to an npc, which will restrict allowed types automatically and force names/sprites to be taken from this type")]
	public bool m_priorityType;

	[Tooltip("Types which cannot be associated with this npc type")]
    public List<string> m_incompatibleTypes;

	[Tooltip("The lower stat bounds for this npc type, leave blank if using default stats")]
	public List<int> m_minimumStats;

	[Tooltip("The upper stat bounds for this npc type, leave blank if using default stats")]
	public List<int> m_maximumStats;

	[Tooltip("All available male names for npcs")]
	public List<string> m_maleNames;

	[Tooltip("All available female names for npcs")]
	public List<string> m_femaleNames;

	[Tooltip("All available male sprites for npcs based on their file names")]
	public List<string> m_maleSprites;

	[Tooltip("All available female sprites for npcs based on their file names")]
	public List<string> m_femaleSprites;

	[Tooltip("All dialogues associated to this npc type, determines which dialogues are allowed to be used for this type")]
    public List<string> m_greetings;

    [Tooltip("All starting quests associated to this npc type (no follow-up quests in here), determines which quests are allowed to be generated for this type")]
    public List<string> m_quests;

	public NPCType Clone() {
		NPCType newType = new NPCType();

		newType.m_type = m_type;
		newType.m_questPercentage = m_questPercentage;
		newType.m_priorityType = m_priorityType;
		newType.m_incompatibleTypes = new List<string>(m_incompatibleTypes);
		newType.m_minimumStats = new List<int>(m_minimumStats);
		newType.m_maximumStats = new List<int>(m_maximumStats);
		newType.m_maleNames = new List<string>(m_maleNames);
		newType.m_femaleNames = new List<string>(m_femaleNames);
		newType.m_maleSprites = new List<string>(m_maleSprites);
		newType.m_femaleSprites = new List<string>(m_femaleSprites);
		newType.m_greetings = new List<string>(m_greetings);
		newType.m_quests = new List<string>(m_quests);

		return newType;
	}

	public void Combine(NPCType type, bool p_appendDataMarker) { 
		m_questPercentage = type.m_questPercentage;
		m_priorityType = type.m_priorityType;

		if(type.m_incompatibleTypes.Count > 0)
			m_incompatibleTypes.AddRange(type.m_incompatibleTypes);

		if(type.m_minimumStats.Count > 0) {
			m_minimumStats.Clear();
			m_minimumStats.AddRange(type.m_minimumStats);
		}

		if(type.m_maximumStats.Count > 0) {
			m_maximumStats.Clear();
			m_maximumStats.AddRange(type.m_maximumStats);
		}

		if(type.m_maleNames.Count > 0)
			m_maleNames.AddRange(type.m_maleNames);

		if(type.m_femaleNames.Count > 0)
			m_femaleNames.AddRange(type.m_femaleNames);

		if(type.m_maleSprites.Count > 0)
			foreach(string sprite in type.m_maleSprites)
				m_maleSprites.Add(sprite + (p_appendDataMarker ? "-External" : ""));

		if(type.m_femaleSprites.Count > 0)
			foreach(string sprite in type.m_femaleSprites)
				m_femaleSprites.Add(sprite + (p_appendDataMarker ? "-External" : ""));

		if(type.m_greetings.Count > 0)
			foreach(string greeting in type.m_greetings)
				m_greetings.Add(greeting + (p_appendDataMarker ? "-External" : ""));

		if(type.m_quests.Count > 0)
			foreach(string quest in type.m_quests)
				m_quests.Add(quest + (p_appendDataMarker ? "-External" : ""));
	}
}