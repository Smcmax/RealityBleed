public class AdvanceQuestReaction : Reaction {

	public AdvanceQuestReaction() : base("advance") { }

	public override void React(DialogController p_controller, string[] p_args) {
		Quest quest = p_controller.m_interactor.m_currentQuests.Find(q => q.m_name == p_args[0]);

        if(quest != null) {
            foreach(QuestStep step in quest.GetCurrentSteps()) {
                Goal goal = step.GetGoal();

                if(goal.m_handInNPC == p_controller.m_npc && !goal.m_completed) {
                    if(goal is TalkGoal) goal.m_completed = true;
                    else goal.CheckRequirements(p_controller.m_interactor);
                }
            }

            quest.Advance();
        }
	}
}