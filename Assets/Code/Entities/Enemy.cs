using System.Collections.Generic;

public class Enemy : NPC {

    public void Init() {
        Init(new List<NPCType>());
    }

    public override void Interact(Entity p_entity) {
        if(m_dialog && p_entity is Player && !m_entity.m_ai.m_target) 
            m_dialog.Interact((Player) p_entity);
    }

    public void ForceCloseDialog() {
        if(m_dialog) m_dialog.ChangeToStartingDialog();
    }
}
