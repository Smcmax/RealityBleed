using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCGenerator : MonoBehaviour {

    [Tooltip("The template to use for every NPC in the game")]
    public GameObject m_npcTemplate;

    [Tooltip("All loaded possible types of npcs in the game")]
    [HideInInspector] public List<NPCType> m_types;

    void Awake() {
		TextAsset[] types = Resources.LoadAll<TextAsset>("NPCTypes");

		foreach(TextAsset type in types)
			m_types.Add(JsonUtility.FromJson<NPCType>(type.text));
	}

	public void GenerateRandom() { 
		GenerateRandom(1);
	}

    public NPC GenerateRandom(int p_typeAmount) {
        List<NPCType> selected = new List<NPCType>();
        List<NPCType> leftover = new List<NPCType>(m_types);
		bool addedPriorityType = false;

        while(selected.Count < p_typeAmount && leftover.Count > 0) {
            NPCType type = leftover[Random.Range(0, leftover.Count)];

            leftover.Remove(type);

            if(selected.Find(t => t.m_incompatibleTypes.Contains(type.m_type)) == null && (!addedPriorityType || !type.m_priorityType)) {
                selected.Add(type);

				if(type.m_priorityType) addedPriorityType = true;
			}
        }

        return Generate(selected);
    }

    public NPC GenerateRandomWithExclusions(int p_typeAmount, List<NPCType> p_exclusions) {
        List<NPCType> selected = new List<NPCType>();
        List<NPCType> leftover = new List<NPCType>(m_types);
		bool addedPriorityType = false;

		foreach(NPCType type in p_exclusions)
            leftover.Remove(type);

        while(selected.Count < p_typeAmount && leftover.Count > 0) {
            NPCType type = leftover[Random.Range(0, leftover.Count)];

            leftover.Remove(type);

			if(selected.Find(t => t.m_incompatibleTypes.Contains(type.m_type)) == null && (!addedPriorityType || !type.m_priorityType)) {
				selected.Add(type);

				if(type.m_priorityType) addedPriorityType = true;
			}
		}

        return Generate(selected);
    }

    public NPC Generate(List<NPCType> p_specificTypes) {
        GameObject npcObject = Instantiate(m_npcTemplate);
		SpriteRenderer renderer = npcObject.GetComponent<SpriteRenderer>();
        NPC npc = npcObject.GetComponent<NPC>();

        npc.Init(p_specificTypes);

        List<string> possibleGreetings = new List<string>();
        List<string> quests = new List<string>();
		List<string> names = new List<string>();
		List<string> sprites = new List<string>();
		List<int> minStats = new List<int>();
		List<int> maxStats = new List<int>();
		bool male = Random.Range(0, 100) >= 50;
		bool priorityTypeProcessed = false;

        foreach(NPCType type in p_specificTypes) {
			if(type.m_priorityType && !priorityTypeProcessed) { 
				names.Clear();
				sprites.Clear();

				priorityTypeProcessed = true;
			}

			if(type.m_priorityType || !priorityTypeProcessed) {
				if(male) {
					if(type.m_maleNames.Count > 0)
						names.AddRange(type.m_maleNames);

					if(type.m_maleSprites.Count > 0)
						sprites.AddRange(type.m_maleSprites);
				} else {
					if(type.m_femaleNames.Count > 0)
						names.AddRange(type.m_femaleNames);

					if(type.m_femaleSprites.Count > 0)
						sprites.AddRange(type.m_femaleSprites);
				}

				if(type.m_minimumStats != null && type.m_minimumStats.Count > 0 && 
					type.m_maximumStats != null && type.m_maximumStats.Count > 0) {
					minStats = type.m_minimumStats;
					maxStats = type.m_maximumStats;
				}
			}

            if(type.m_greetings.Count > 0)
                possibleGreetings.AddRange(type.m_greetings);

            if(type.m_quests.Count > 0) {
                int questCount = 0;
                float random = Random.Range(0f, 100f);

                while(random <= type.m_questPercentage && (questCount <= type.m_quests.Count || questCount <= 3)) {
                    questCount++;
                    random = Random.Range(0f, 100f);
                }

                if(questCount > 0) {
                    List<string> availableQuests = new List<string>(type.m_quests);

                    for(int i = 0; i < questCount; i++) {
						if(availableQuests.Count == 0) break;
						string sQuest = availableQuests[Random.Range(0, availableQuests.Count)];

						List<string> currentQuests = new List<string>(quests);
                        List<string> topLevelQuests = new List<string>();
                        GetTopLevelQuests(sQuest, ref currentQuests, ref topLevelQuests);

                        foreach(string top in topLevelQuests)
                            if(availableQuests.Contains(top)) {
                                availableQuests.Remove(top);
								quests.Add(top);
							}

                        i += topLevelQuests.Count;
                    }
                }
            }
        }

		npcObject.name = names[Random.Range(0, names.Count)];

		string spriteName = sprites[Random.Range(0, sprites.Count)];
		renderer.sprite = SpriteUtils.LoadNPCSprite(male ? "Male/" + spriteName : "Female/" + spriteName);

        npc.m_questsAvailable = quests;
		npc.m_dialog.m_dialogTemplate = GameObject.Find("UI").transform.Find("Dialogue Canvas").Find("Speech Bubble").gameObject;
        npc.m_dialog.m_currentDialog = Dialog.Get(possibleGreetings[Random.Range(0, possibleGreetings.Count)]);
        npc.m_dialog.Init();

		npc.transform.position = Player.GetPlayerFromId(0).transform.position;
		npc.m_entity.m_feedbackTemplate = GameObject.Find("UI").transform.Find("Feedback Canvas").Find("Feedback").gameObject;

		StartCoroutine(SetStats(npc, minStats, maxStats));

        return npc;
    }

	private IEnumerator SetStats(NPC p_npc, List<int> p_minStats, List<int> p_maxStats) {
		yield return new WaitForSeconds(0.05f);

		for(int i = 0; i < UnitStats.STAT_AMOUNT; i++) {
			bool defaultStat = p_minStats.Count <= i || p_maxStats.Count <= i;
			p_npc.m_entity.m_stats.SetBaseStat((Stats)i, defaultStat ? 5 : Random.Range(p_minStats[i], p_maxStats[i]));
		}
	}

    private void GetTopLevelQuests(string p_quest, ref List<string> p_list, ref List<string> p_topLevelQuests) {
        if(!p_list.Contains(p_quest)) p_list.Add(p_quest);
        else return;

        Quest quest = Quest.Get(p_quest, true);

        if(quest.m_prerequisites != null && quest.m_prerequisites.Count > 0) {
            foreach(string prev in quest.m_prerequisites)
                if(!p_list.Contains(prev))
					GetTopLevelQuests(prev, ref p_list, ref p_topLevelQuests);
        } else p_topLevelQuests.Add(p_quest);

        if(quest.m_nextQuests != null && quest.m_nextQuests.Count > 0)
            foreach(string next in quest.m_nextQuests)
                if(!p_list.Contains(next))
					GetTopLevelQuests(next, ref p_list, ref p_topLevelQuests);
    }
}