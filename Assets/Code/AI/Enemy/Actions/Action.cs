using UnityEngine;
using System;

[Serializable]
public class Action {

    [Tooltip("The action's type, ex: Chase, FindTarget, Patrol, etc.")]
    public string m_type;

    public static Action Load(string p_json) {
        Action action = JsonUtility.FromJson<Action>(p_json);
        Type type = null;

        if(!action) return null;

        switch(action.m_type.ToLower()) {
            case "chase": type = typeof(ChaseAction); break;
            case "findtarget": type = typeof(FindTargetAction); break;
            case "patrol": type = typeof(PatrolAction); break;
            case "scan": type = typeof(ScanAction); break;
            case "shoot": type = typeof(ShootAction); break;
        }

        if(type == null) return null;

        return (Action) JsonUtility.FromJson(p_json, type);
    }

    public virtual void Execute(StateController p_controller) { }
	public virtual void OnTransition(StateController p_controller) { }

    public static implicit operator bool(Action p_instance) {
        return p_instance != null;
    }
}
