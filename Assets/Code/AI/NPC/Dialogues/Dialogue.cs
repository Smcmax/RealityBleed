using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "AI/NPC/Dialogue")]
public class Dialogue : ScriptableObject {

	[Tooltip("The lines of dialogue to deliver before triggering the next choice prompt, {entity} fills in the entity's name")]
	public List<string> m_lines;

	[Tooltip("The interactor's available choices after all lines have been delivered")]
	public List<Choice> m_choices;
}