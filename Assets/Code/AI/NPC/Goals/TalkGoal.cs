public class TalkGoal : Goal {

	public TalkGoal() : base("talk") { }

	// p_args[0] is either an npc type or a number from the quest's npc history (if npc history is invalid, getting random npc)
	public override void Activate(Entity p_entity, string[] p_args) {
		int npcHistory = -1;
		
		if(int.TryParse(p_args[0], out npcHistory)) { 
			if(m_associatedQuest.m_npcHistory.Count > npcHistory && npcHistory >= 0)
				m_handInNPC = m_associatedQuest.m_npcHistory[npcHistory];
			else {
				m_handInNPC = NPC.FindRandom();

				while(m_handInNPC == m_associatedQuest.m_currentNPC)
					m_handInNPC = NPC.FindRandom();
			}
		} else {
			m_handInNPC = NPC.FindRandomFromType(p_args[0]);

			while(m_handInNPC == m_associatedQuest.m_currentNPC)
				m_handInNPC = NPC.FindRandomFromType(p_args[0]);
		}

		m_completed = true;
	}

	public override void Deactivate(Entity p_entity) { }
}