using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class Dialog {

	public static List<Dialog> m_loadedDialogs = new List<Dialog>();

	[Tooltip("This dialogue's name")]
	public string m_name;

	[Tooltip("The lines of dialogue to deliver before triggering the next choice prompt, {player} fills in the player's name")]
	public List<string> m_lines;

	[Tooltip("The interactor's available choices after all lines have been delivered")]
	public List<Choice> m_choices;

	public static Dialog Get(string p_name) {
		string name = p_name.Replace("-External", "");
		Dialog found = m_loadedDialogs.Find(d => d.m_name == name);

		if(found != null) return found;

		Dialog dialog = null;

		if(p_name.EndsWith("-External")) {
			StreamReader reader = new StreamReader(Application.dataPath + "/Data/Dialogs/" + name + ".json");

			dialog = JsonUtility.FromJson<Dialog>(reader.ReadToEnd());
			reader.Close();
		} else dialog = JsonUtility.FromJson<Dialog>(Resources.Load<TextAsset>("Dialogs/" + p_name).text);

		if(dialog != null) m_loadedDialogs.Add(dialog);
		
		return dialog;
	}
}