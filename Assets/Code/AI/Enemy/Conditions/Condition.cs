using UnityEngine;
using System;

[Serializable]
public class Condition {

    [Tooltip("The condition's type, ex: CurrentHealth, PatrolEnded, ScanComplete, etc.")]
    public string m_type;

    public static Condition Load(string p_json) {
        Condition condition = JsonUtility.FromJson<Condition>(p_json);
        Type type = null;

        if(!condition) return null;

        switch(condition.m_type.ToLower()) {
            case "currenthealth": type = typeof(CurrentHealthCondition); break;
            case "patrolended": type = typeof(PatrolEndedCondition); break;
            case "scancomplete": type = typeof(ScanCompleteCondition); break;
            case "targetactive": type = typeof(TargetActiveCondition); break;
            case "targethealth": type = typeof(TargetHealthCondition); break;
            case "targetinrange": type = typeof(TargetInRangeCondition); break;
            case "timeelapsed": type = typeof(TimeElapsedCondition); break;
        }

        if(type == null) return null;

        return (Condition) JsonUtility.FromJson(p_json, type);
    }

    public virtual bool Test(StateController p_controller) { return false; }

    public static implicit operator bool(Condition p_instance) {
        return p_instance != null;
    }
}
