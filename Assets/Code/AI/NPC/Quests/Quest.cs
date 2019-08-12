using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

[Serializable]
public class Quest {

    public static List<Quest> m_loadedQuests = new List<Quest>();

    [Tooltip("The name of this quest")]
    public string m_name;

	[Tooltip("The choice leading into the lead in dialog, empty if none")]
	public string m_leadInChoiceLine;

	[Tooltip("The dialog chain leading into this quest, empty if none")]
	public string m_leadInDialog;

	[Tooltip("The dialog chain after the quest, empty if none")]
	public string m_leadOutDialog;

    [Tooltip("The prerequisite quests to this one")]
    public List<string> m_prerequisites;

    [Tooltip("The following quests to this one")]
    public List<string> m_nextQuests;

    [Tooltip("All steps pertaining to this quest")]
    public List<QuestStep> m_steps;

    [Tooltip("The reward drop table for this quest")]
    public DropTable m_rewardTable;

	[HideInInspector] public List<NPC> m_npcHistory = new List<NPC>();
	[HideInInspector] public NPC m_currentNPC;

	private Player m_player;
    private int m_currentStepNumber = -1;

	public void Accept(NPC p_questGiver, Player p_player) {
		m_npcHistory.Add(p_questGiver);
		m_currentNPC = p_questGiver;
		m_player = p_player;
		m_player.m_currentQuests.Add(this);
        m_currentStepNumber = -1;

        Advance();
    }

    public List<QuestStep> GetPreviousSteps() {
        return m_steps.FindAll(qs => qs.m_stepNumber < m_currentStepNumber);
    }

    public List<QuestStep> GetCurrentSteps() {
        return m_steps.FindAll(qs => qs.m_stepNumber == m_currentStepNumber);
    }

	public string GetNextChoiceLine() {
        List<QuestStep> nextSteps = m_steps.FindAll(qs => qs.m_stepNumber == m_currentStepNumber + 1);

        foreach(QuestStep step in nextSteps)
            if(!String.IsNullOrEmpty(step.m_choiceLine))
                return step.m_choiceLine;

        return "";
    }

	public string GetHandInChoiceLine() {
		List<QuestStep> steps = GetCurrentSteps();

		foreach(QuestStep step in steps)
			if(!String.IsNullOrEmpty(step.m_handInChoiceLine))
				return step.m_handInChoiceLine;

		return "";
	}

	public string GetNextChoicePostDialog() {
        List<QuestStep> nextSteps = m_steps.FindAll(qs => qs.m_stepNumber == m_currentStepNumber + 1);

        foreach(QuestStep step in nextSteps)
			if(!String.IsNullOrEmpty(step.m_postDialog))
				return step.m_postDialog;

        return "";
    }

	public bool IsLastStep() { 
		return !m_steps.Exists(qs => qs.m_stepNumber == m_currentStepNumber + 1);
	}

    public void Advance() {
		if(m_currentStepNumber >= 0) {
			foreach(QuestStep step in GetCurrentSteps())
				if(!step.GetGoal().m_completed) return;

			ActivateGoals(false);
		}

        m_currentStepNumber++;

        if(!ActivateGoals(true)) Complete();
    }

    private bool ActivateGoals(bool p_activate) {
        List<QuestStep> steps = GetCurrentSteps();

        if(steps != null && steps.Count > 0) {
            foreach(QuestStep step in steps)
                if(p_activate) {
                    step.SetGoal(Goal.Activate(m_player, this, step.m_goal));

					NPC handInNPC = step.GetGoal().m_handInNPC;

					if(handInNPC && handInNPC != null) {
						m_currentNPC = handInNPC;

						if(!m_npcHistory.Contains(m_currentNPC))
							m_npcHistory.Add(m_currentNPC);
					}
				} else step.GetGoal().Deactivate(m_player);

			return true;
        }

		return false;
    }

	public void Complete() { 
		m_rewardTable.Drop(m_player.m_inventory);

		// assigned dynamically to reduce generation times and to improve accuracy 
		if(m_nextQuests.Count > 0)
			foreach(string next in m_nextQuests)
				if(!m_currentNPC.m_questsAvailable.Contains(next))
					m_currentNPC.m_questsAvailable.Add(next);
	}

    // reference means to simply return the template quest instead of making a new one
    public static Quest Get(string p_name, bool p_reference) {
		string name = p_name.Replace("-External", "");
        Quest found = m_loadedQuests.Find(d => d.m_name == name);

        if(found != null) return p_reference ? found : found.Clone();

		Quest loadedQuest = null;

		if(p_name.EndsWith("-External")) { 
			StreamReader reader = new StreamReader(Application.dataPath + "/Data/Quests/" + name + ".json");

			loadedQuest = JsonUtility.FromJson<Quest>(reader.ReadToEnd());
			reader.Close();
		} else loadedQuest = JsonUtility.FromJson<Quest>(Resources.Load<TextAsset>("Quests/" + name).text);

        if(loadedQuest != null) m_loadedQuests.Add(loadedQuest);

        return Get(p_name, p_reference);
    }

	public Quest Clone() {
		Quest newQuest = new Quest();

		newQuest.m_name = m_name;
		newQuest.m_leadInChoiceLine = m_leadInChoiceLine;
		newQuest.m_leadInDialog = m_leadInDialog;
		newQuest.m_leadOutDialog = m_leadOutDialog;
		newQuest.m_prerequisites = new List<string>(m_prerequisites);
		newQuest.m_nextQuests = new List<string>(m_nextQuests);
		newQuest.m_steps = new List<QuestStep>();
		newQuest.m_rewardTable = m_rewardTable;

		foreach(QuestStep step in m_steps) {
			QuestStep newStep = new QuestStep();

			newStep.m_stepNumber = step.m_stepNumber;
			newStep.m_choiceLine = step.m_choiceLine;
			newStep.m_postDialog = step.m_postDialog;
			newStep.m_handInChoiceLine = step.m_handInChoiceLine;
			newStep.m_goal = step.m_goal;

			newQuest.m_steps.Add(newStep);
		}

		return newQuest;
	}
}

[Serializable]
public class QuestStep {

    [Tooltip("The step's execution order (starting at 0), if same as other steps, they will all be required to advance")]
    public int m_stepNumber;

    [Tooltip("The choice to initiate this step of the quest, if empty, initiated by another step with equivalent step number or the previous hand in")]
    public string m_choiceLine;

	[Tooltip("Dialogs given by the npc upon accepting this step of the quest")]
	public string m_postDialog;

	[Tooltip("The choice to hand in this step of the quest (and move on to the next) if necessary")]
	public string m_handInChoiceLine;

    [Tooltip("The goal to accomplish during this step")]
    public string m_goal;

    private Goal m_goalObject;

    public Goal GetGoal() {
        return m_goalObject;
    }

    public void SetGoal(Goal p_goal) {
        m_goalObject = p_goal;
    }
}