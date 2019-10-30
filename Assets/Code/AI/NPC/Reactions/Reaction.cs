using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class Reaction {

	private static List<Reaction> m_reactions = new List<Reaction>();

	[Tooltip("The word used to trigger this reaction (ex: accept)")]
	public string m_word;

	public Reaction(string p_word) {
		m_word = p_word;

		m_reactions.Add(this);
	}

	public abstract void React(DialogController p_controller, string[] p_args);

	public static void React(DialogController p_controller, string p_reaction) {
		if(m_reactions.Count == 0) LoadReactions();

		string[] split = p_reaction.Split(' ');
		Reaction found = m_reactions.Find(r => r.m_word == split[0]);

		if(found != null) 
			found.React(p_controller, p_reaction.Replace(split[0] + " ", "").Split(' '));
	}

	private static void LoadReactions() {
		m_reactions = new List<Reaction>();

		new AcceptQuestReaction();
		new AdvanceQuestReaction();
		new CloseReaction();
		new DialogReaction();
        new VendorReaction();
	}
}
