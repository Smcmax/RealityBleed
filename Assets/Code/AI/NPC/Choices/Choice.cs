using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "AI/NPC/Choice")]
public class Choice : ScriptableObject {

	[Tooltip("Text categorizing the choice for the interactor, what it sees in the prompt")]
	public string m_line;

	[Tooltip("Reactions to the choice made by the interactor")]
	public List<Reaction> m_reactions;

	public void React(DialogueController p_controller) {
		foreach(Reaction reaction in m_reactions)
			reaction.React(p_controller);
	}
}
