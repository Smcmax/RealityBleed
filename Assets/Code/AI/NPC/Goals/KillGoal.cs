using UnityEngine;

public class KillGoal : Goal {

    public KillGoal() : base("kill") { }

    public override void Activate(Entity p_entity, string[] p_args) {
        // listen to killing events and find if entity is killer?
    }

    public override void CheckRequirements(Entity p_entity) {
        m_completed = true;
    }

    public override void Deactivate(Entity p_entity) {

    }

    public override string GetDisplayName() {
        return "Kill x";
    }
}