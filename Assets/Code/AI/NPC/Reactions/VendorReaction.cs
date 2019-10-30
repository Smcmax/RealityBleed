public class VendorReaction : Reaction {

    public VendorReaction() : base("shop") { }

    public override void React(DialogController p_controller, string[] p_args) {
        p_controller.DisplayShop();
    }
}