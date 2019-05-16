public class AdvanceQuestReaction : Reaction {

	public AdvanceQuestReaction() : base("advance") { }

	public override void React(DialogController p_controller, string[] p_args) {
		Quest quest = p_controller.m_interactor.m_currentQuests.Find(q => q.m_name == p_args[0]);

		if(quest != null) quest.Advance();
	}
}