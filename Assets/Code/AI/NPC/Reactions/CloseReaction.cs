using UnityEngine;

public class CloseReaction : Reaction {

    public CloseReaction() : base("close") { }

    public override void React(DialogController p_controller, string[] p_args) {
        p_controller.ChangeToStartingDialog();
    }
}