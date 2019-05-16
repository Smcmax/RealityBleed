using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Dialog {

	private static List<Dialog> m_loadedDialogs = new List<Dialog>();

	[Tooltip("This dialogue's name")]
	public string m_name;

	[Tooltip("The lines of dialogue to deliver before triggering the next choice prompt, {player} fills in the player's name")]
	public List<string> m_lines;

	[Tooltip("The interactor's available choices after all lines have been delivered")]
	public List<Choice> m_choices;

	public static Dialog Get(string p_name) {
		Dialog found = m_loadedDialogs.Find(d => d.m_name == p_name);

		if(found != null) return found;

		Dialog dialog = JsonUtility.FromJson<Dialog>(Resources.Load<TextAsset>("Dialogs/" + p_name).text);
		m_loadedDialogs.Add(dialog);
		
		return dialog;
	}
}