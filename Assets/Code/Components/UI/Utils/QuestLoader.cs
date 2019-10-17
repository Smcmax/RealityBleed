using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestLoader : MonoBehaviour {

    [Tooltip("The panel displaying quest steps")]
    public GameObject m_stepPanel;

    [Tooltip("The prefab used for each quest")]
    public GameObject m_questPrefab;

    [Tooltip("The prefab used for each quest step")]
    public GameObject m_questStepPrefab;

    [Tooltip("The object used as a parent for all quest prefabs")]
    public Transform m_questParent;

    [Tooltip("The object used as a parent for all quest step prefabs")]
    public Transform m_stepParent;

    [Tooltip("Should this loader display active or completed quests?")]
    public bool m_displayActive;

    private Dictionary<Quest, GameObject> m_loadedQuests;
    private List<GameObject> m_loadedSteps;

    public void OnEnable() {
        m_loadedQuests = new Dictionary<Quest, GameObject>();
        m_loadedSteps = new List<GameObject>();

        List<Quest> quests = new List<Quest>();
        Player player = Player.GetPlayerFromId(MenuHandler.Instance.m_handlingPlayer.id);

        if(m_displayActive) quests.AddRange(player.m_currentQuests);
        else quests.AddRange(player.m_completedQuests);

        foreach(Quest quest in quests) {
            GameObject questObj = Instantiate(m_questPrefab, m_questParent);

            questObj.transform.Find("Background").Find("Quest Name").GetComponent<TMP_Text>().text = quest.m_name;
            
            QuestSelectable selectable = questObj.GetComponent<QuestSelectable>();
            selectable.m_loader = this;
            selectable.m_quest = quest;

            m_loadedQuests.Add(quest, questObj);
        }

        if(m_loadedQuests.Count > 0) GetComponentInChildren<SelectableSelectionController>().Init();
    }

    public void LoadQuestSteps(Quest p_quest) {
        DestroyQuestSteps();

        foreach(QuestStep step in p_quest.m_steps) {
            if(step.GetGoal() == null) continue;

            GameObject stepObj = Instantiate(m_questStepPrefab, m_stepParent);

            stepObj.transform.Find("Status Icon").gameObject.SetActive(step.GetGoal().m_completed);
            stepObj.transform.Find("Step Name").GetComponent<TMP_Text>().text = step.GetGoal().GetDisplayName();

            m_loadedSteps.Add(stepObj);
        }
    }

    private void DestroyQuestSteps() {
        if(m_loadedSteps.Count > 0)
            foreach(GameObject loadedSteps in new List<GameObject>(m_loadedSteps))
                Destroy(loadedSteps);

        m_loadedSteps.Clear();
    }

    public void OnDisable() {
        if(m_loadedQuests.Count > 0) {
            foreach(Quest loaded in new Dictionary<Quest, GameObject>(m_loadedQuests).Keys)
                Destroy(m_loadedQuests[loaded]);

            DestroyQuestSteps();
        }

        m_loadedQuests.Clear();
    }
}
