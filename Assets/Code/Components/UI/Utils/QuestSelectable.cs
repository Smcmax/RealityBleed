using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestSelectable : Selectable {

    [HideInInspector] public Quest m_quest;
    [HideInInspector] public QuestLoader m_loader;

    public override void OnSelect(BaseEventData eventData) {
        base.OnSelect(eventData);

        m_loader.LoadQuestSteps(m_quest);
    }
}
