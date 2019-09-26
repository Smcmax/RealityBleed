using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class NPCGenerator : MonoBehaviour {

    [Tooltip("The template to use for every NPC in the game")]
    public GameObject m_npcTemplate;

	[Tooltip("Are we using external data to generate npcs?")]
	public bool m_useExternalTypes;

    [Tooltip("All included possible types of npcs in the game")]
    [HideInInspector] public List<NPCType> m_types = new List<NPCType>();

	[Tooltip("All externally loaded types of npcs (includes existing types from m_types if they contain modifications)")]
	[HideInInspector] public List<NPCType> m_externalTypes = new List<NPCType>();

	[Tooltip("Internal and external types merged together")]
	[HideInInspector] public List<NPCType> m_combinedTypes = new List<NPCType>();

	void Awake() {
		LoadTypes(false);
	}

	public void LoadTypes(bool p_clearCaches) {
		m_types.Clear();
		m_externalTypes.Clear();
		m_combinedTypes.Clear();

		if(p_clearCaches) { 
			Dialog.m_loadedDialogs.Clear();
			Quest.m_loadedQuests.Clear();

			CleanUp();
		}

		TextAsset[] types = Resources.LoadAll<TextAsset>("NPCTypes");

        foreach(TextAsset typeText in types) {
            NPCType type = JsonUtility.FromJson<NPCType>(typeText.text);

            if(type != null) m_types.Add(type);
        }

		string[] files = Directory.GetFiles(Application.dataPath + "/Data/NPCTypes/");

		if(files.Length > 0)
			foreach(string file in files) { 
				if(file.ToLower().EndsWith(".json")) {
					StreamReader reader = new StreamReader(file);
                    NPCType type = JsonUtility.FromJson<NPCType>(reader.ReadToEnd());

                    if(type != null) m_externalTypes.Add(type);
					reader.Close();
				}
			}

		foreach(NPCType type in m_types) { 
			NPCType external = m_externalTypes.Find(nt => nt.m_type == type.m_type);

			if(external != null) { 
				NPCType combined = type.Clone();
				
				combined.Combine(external, true);
				m_combinedTypes.Add(combined);
			} else m_combinedTypes.Add(type);
		}

		if(m_externalTypes.Count > 0)
			foreach(NPCType external in m_externalTypes)
				if(!m_types.Exists(nt => nt.m_type == external.m_type)) {
					external.AppendDataMarker();
					m_combinedTypes.Add(external);
				}
	}

	public void CleanUp() { 
		Resources.UnloadUnusedAssets();
	}

    public void GenerateRandom(int p_typeAmount) {
        List<NPCType> selected = new List<NPCType>();
        List<NPCType> leftover = new List<NPCType>(m_useExternalTypes ? m_combinedTypes : m_types);
		bool addedPriorityType = false;

        while(selected.Count < p_typeAmount && leftover.Count > 0) {
            NPCType type = leftover[Random.Range(0, leftover.Count)];

            leftover.Remove(type);

            if(selected.Find(t => t.m_incompatibleTypes.Contains(type.m_type)) == null && (!addedPriorityType || !type.m_priorityType)) {
                selected.Add(type);

				if(type.m_priorityType) addedPriorityType = true;
			}
        }

        Generate(selected);
    }

    public void GenerateRandomWithExclusions(int p_typeAmount, List<NPCType> p_exclusions) {
        List<NPCType> selected = new List<NPCType>();
        List<NPCType> leftover = new List<NPCType>(m_useExternalTypes ? m_combinedTypes : m_types);
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

        Generate(selected);
    }

    public void Generate(List<NPCType> p_specificTypes) {
        GameObject npcObject = Instantiate(m_npcTemplate);
		SpriteRenderer renderer = npcObject.GetComponent<SpriteRenderer>();
        NPC npc = npcObject.GetComponent<NPC>();

		List<NPCType> allTypes = new List<NPCType>();
        List<string> possibleGreetings = new List<string>();
        List<string> quests = new List<string>();
		List<string> names = new List<string>();
		List<SerializableSprite> sprites = new List<SerializableSprite>();
		List<int> minStats = new List<int>();
		List<int> maxStats = new List<int>();
		bool male = Random.Range(0, 100) >= 50;
		bool priorityTypeProcessed = false;

        foreach(NPCType specific in p_specificTypes) {
			NPCType type = specific;

			if(m_useExternalTypes) type = m_combinedTypes.Find(nt => nt.m_type == specific.m_type);
			allTypes.Add(type);

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

		npc.Init(allTypes);

		npcObject.name = names[Random.Range(0, names.Count)];

		SerializableSprite sprite = sprites[Random.Range(0, sprites.Count)];
		sprite.m_name = (male ? "Male/" : "Female/") + sprite.m_name;
		renderer.sprite = sprite.Sprite;

        npc.m_questsAvailable = quests;
		npc.m_dialog.m_dialogTemplate = GameObject.Find("UI").transform.Find("Dialogue Canvas").Find("Speech Bubble").gameObject;
        npc.m_dialog.m_currentDialog = Dialog.Get(possibleGreetings[Random.Range(0, possibleGreetings.Count)]);
        npc.m_dialog.Init();

		npc.transform.position = Player.GetPlayerFromId(0).transform.position;
		npc.m_entity.m_feedbackTemplate = GameObject.Find("UI").transform.Find("Feedback Canvas").Find("Feedback").gameObject;

		StartCoroutine(SetStats(npc, minStats, maxStats));
    }

	private IEnumerator SetStats(NPC p_npc, List<int> p_minStats, List<int> p_maxStats) {
		yield return new WaitForSeconds(0.05f);

		for(int i = 0; i < UnitStats.STAT_AMOUNT; i++) {
			bool defaultStat = p_minStats.Count <= i || p_maxStats.Count <= i;
			p_npc.m_entity.m_stats.SetBaseStat((Stats) i, defaultStat ? 5 : Random.Range(p_minStats[i], p_maxStats[i]));
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