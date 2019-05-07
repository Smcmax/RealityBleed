using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC))]
public class DialogueController : MonoBehaviour {

	[Tooltip("The dialogue currently showing")]
	public Dialogue m_currentDialogue;

	[Tooltip("The template used to display the dialogue on screen")]
	public GameObject m_dialogueTemplate;

	[HideInInspector] public NPC m_npc;
    [HideInInspector] public Entity m_interactor;

	private DialogueWindow m_dialogueWindow;
    private int m_currentDialogueLine = 0;
	private bool m_selectingChoice = false;
	private float m_lastReset = 0;
	private Dialogue m_startingDialogue;

	void Awake() {
        m_startingDialogue = m_currentDialogue;
		m_npc = GetComponent<NPC>();
		m_npc.m_dialogue = this;
	}

	public void Interact() {
		Interact(m_interactor);
	}

	public void Interact(Entity p_entity) {
		if(Time.unscaledTime - m_lastReset < 0.25f) return;
		if(m_interactor && m_interactor != p_entity) return;
		else if(!m_interactor) m_interactor = p_entity;

		MenuHandler.Instance.ClearMenu();

		if(m_currentDialogue.m_lines.Count > m_currentDialogueLine) {
			Display(GetFormattedLine(m_currentDialogue.m_lines[m_currentDialogueLine]));
			m_currentDialogueLine++;
		} else if(!m_selectingChoice) {
			m_dialogueWindow.DisplayChoices(m_currentDialogue.m_choices);
			m_selectingChoice = true;
		} else ChangeToStartingDialogue();
	}

    public string GetFormattedLine(string p_line) {
        return Game.m_languages.GetLine(p_line).Replace("{entity}", m_interactor.gameObject.name);
    }

	// TODO: add gradual display + eventual disappearance
	public void Display(string p_text) {
		if(!m_dialogueWindow) {
            m_dialogueWindow = Instantiate(m_dialogueTemplate, m_dialogueTemplate.transform.parent).GetComponent<DialogueWindow>();
			m_dialogueWindow.Setup(this, gameObject.name); // TODO: Change to NPC name lol
		}

		m_dialogueWindow.Display(p_text);
	}

	private void CleanDisplay(bool p_removeDialogueWindow) {
		if(!m_dialogueWindow) return;

		if(p_removeDialogueWindow) {
			DialogueWindow.m_openedWindows.Remove(m_dialogueWindow);

			Destroy(m_dialogueWindow.gameObject);
			m_dialogueWindow = null;
			m_interactor = null;
		} else m_dialogueWindow.Clear();
	}

	public void ChangeToStartingDialogue() {
        m_lastReset = Time.unscaledTime;
		ChangeDialogue(m_startingDialogue, false);
	}

	public void ChangeDialogue(Dialogue p_dialogue, bool p_display) {
		m_currentDialogue = p_dialogue;
		m_currentDialogueLine = 0;
		m_selectingChoice = false;

		CleanDisplay(!p_display);

		if(p_display) Interact(m_interactor);
	}
}
