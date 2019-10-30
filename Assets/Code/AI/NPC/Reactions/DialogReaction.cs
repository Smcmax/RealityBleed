using UnityEngine;

public class DialogReaction : Reaction {

    public DialogReaction() : base("show") { }

    public override void React(DialogController p_controller, string[] p_args) {
        p_controller.ChangeDialog(Dialog.Get(p_args[0]), true, true);
    }
}