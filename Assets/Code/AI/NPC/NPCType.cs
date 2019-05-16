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
}