using UnityEngine;

[CreateAssetMenu(menuName = "AI/NPC/Reactions/Dialogue")]
public class DialogueReaction : Reaction {

    [Tooltip("The dialogue to open when this reaction occurs")]
    public Dialogue m_dialogue;

    public override void React(DialogueController p_controller) {
        p_controller.ChangeDialogue(m_dialogue, true);
    }
}