public class AcceptQuestReaction : Reaction {

	public AcceptQuestReaction() : base("accept") { }

	public override void React(DialogController p_controller, string[] p_args) {
		Quest quest = Quest.Get(p_args[0], false);

		if(p_controller.m_interactor.IsEligible(quest))
			quest.Accept(p_controller.m_npc, p_controller.m_interactor);
	}
}