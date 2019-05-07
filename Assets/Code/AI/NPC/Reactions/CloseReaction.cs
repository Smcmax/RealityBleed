using UnityEngine;

[CreateAssetMenu(menuName = "AI/NPC/Reactions/Close")]
public class CloseReaction : Reaction {

    public override void React(DialogueController p_controller) {
        p_controller.ChangeToStartingDialogue();
    }
}