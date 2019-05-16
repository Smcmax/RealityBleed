using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public abstract class Goal {

    private static List<Goal> m_defaultGoals = new List<Goal>(); // default goals

    [Tooltip("The type of this goal, used to find and configure a goal with a certain type")]
    public string m_type;

	[Tooltip("The quest linked to this goal")]
	public Quest m_associatedQuest;

	[Tooltip("Whether or not this goal is completed")]
	public bool m_completed;

	[Tooltip("The NPC to hand this goal in to, leave null if not necessary (if set, should be set in activate), this is just to make steps move along through npcs")]
	public NPC m_handInNPC;

    public Goal(string p_type) {
        m_type = p_type;
		m_completed = false;
    }

    public abstract void Activate(Entity p_entity, string[] p_args);

    public abstract void Deactivate(Entity p_entity);

    public static Goal Activate(Entity p_entity, Quest p_quest, string p_goal) {
        if(m_defaultGoals.Count == 0) LoadGoals();

        string[] split = p_goal.Split(' ');
        Goal found = m_defaultGoals.Find(r => r.m_type == split[0]);

        if(found != null) {
            Goal newGoal = (Goal) Activator.CreateInstance(found.GetType()); // unsure if this will even work
			newGoal.m_associatedQuest = p_quest;
            newGoal.Activate(p_entity, p_goal.Replace(split[0] + " ", "").Split(' '));

            return newGoal;
        }

        return null;
    }

    private static void LoadGoals() {
        m_defaultGoals = new List<Goal>();
        
        m_defaultGoals.Add(new KillGoal());
		m_defaultGoals.Add(new TalkGoal());
    }
}