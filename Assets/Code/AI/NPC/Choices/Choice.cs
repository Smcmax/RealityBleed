using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Choice {

	[Tooltip("Text categorizing the choice for the interactor, what it sees in the prompt")]
	public string m_line;

	[Tooltip("Reactions to the choice made by the interactor")]
	public List<string> m_reactions;

	public void React(DialogController p_controller) {
		foreach(string reaction in m_reactions)
			Reaction.React(p_controller, reaction);
	}
}
