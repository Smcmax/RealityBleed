using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(NPC))]
public class DialogController : MonoBehaviour {

	[Tooltip("The dialog currently showing, also represents the default dialog")]
	public Dialog m_currentDialog;

	[Tooltip("The template used to display the dialog on screen")]
	public GameObject m_dialogTemplate;

    [Tooltip("The template used to display the shop on screen")]
    public GameObject m_shopTemplate;

	[HideInInspector] public NPC m_npc;
    [HideInInspector] public Player m_interactor;

	private DialogWindow m_dialogWindow;
    private ShopWindow m_shopWindow;
    private int m_currentDialogLine = 0;
	private bool m_selectingChoice = false;
	private float m_lastReset = 0;
	private Dialog m_startingDialog;
    private Choice m_shopChoice;
	private Choice m_closeChoice;
	private Quest m_introducingQuest;

	public void Init() {
        m_startingDialog = m_currentDialog;
        m_shopChoice = new Choice();
        m_closeChoice = new Choice();

        m_shopChoice.m_line = "Shop";
        m_closeChoice.m_line = "Close";
        m_shopChoice.m_reactions = new List<string>();
        m_closeChoice.m_reactions = new List<string>();
        m_shopChoice.m_reactions.Add("shop");
        m_closeChoice.m_reactions.Add("close");
	}

	public void Interact() {
		Interact(m_interactor);
	}

	public void Interact(Player p_player) {
		if(Time.unscaledTime - m_lastReset < 0.25f) return;
		if((m_interactor && m_interactor != p_player) || (!m_interactor && p_player.m_interactingWithNPC)) return;
		else if(!m_interactor) m_interactor = p_player;

		m_interactor.m_interactingWithNPC = true;

        if(m_shopWindow) { ChangeToStartingDialog(true); return; }

        MenuHandler.Instance.ClearMenu();

		if(m_currentDialog.m_lines.Count > m_currentDialogLine) {
			Display(GetFormattedLine(m_currentDialog.m_lines[m_currentDialogLine]));
			m_currentDialogLine++;
		} else if(!m_selectingChoice) {
			List<Choice> choices = new List<Choice>(m_currentDialog.m_choices);

			if(m_introducingQuest != null) {
				if(choices.Count == 0 || choices.Exists(c => c.m_reactions.Exists(r => r.StartsWith("show")))) {
					Choice questChoice = new Choice();

					questChoice.m_line = m_introducingQuest.GetNextChoiceLine();
					questChoice.m_reactions = new List<string>();
					questChoice.m_reactions.Add("accept " + m_introducingQuest.m_name);

					if(m_introducingQuest.GetNextChoicePostDialog() != "")
						questChoice.m_reactions.Add("show " + m_introducingQuest.GetNextChoicePostDialog());
					else questChoice.m_reactions.Add("close");

					choices.Add(questChoice);
					m_introducingQuest = null;
				}
			}

			if(m_currentDialog == m_startingDialog) { 
				if(m_interactor.m_currentQuests.Count > 0)
					foreach(Quest currentQuest in m_interactor.m_currentQuests)
						if(currentQuest.GetHandInChoiceLine() != "" && currentQuest.m_currentNPCs.Contains(m_npc)) { 
                            Choice advanceChoice = new Choice();

							advanceChoice.m_line = currentQuest.GetHandInChoiceLine();
							advanceChoice.m_reactions = new List<string>();
							advanceChoice.m_reactions.Add("advance " + currentQuest.m_name);

							if(currentQuest.IsLastStep() && !String.IsNullOrEmpty(currentQuest.m_leadOutDialog))
								advanceChoice.m_reactions.Add("show " + currentQuest.m_leadOutDialog);
							else if(currentQuest.GetNextChoicePostDialog() != "")
								advanceChoice.m_reactions.Add("show " + currentQuest.GetNextChoicePostDialog());
							else advanceChoice.m_reactions.Add("close");

							choices.Add(advanceChoice);
						}

				if(m_npc.m_questsAvailable.Count > 0)
					foreach(string quest in m_npc.m_questsAvailable) {
						Quest refQuest = Quest.Get(quest, true);

						if(m_interactor.IsEligible(refQuest)) {
							if(String.IsNullOrEmpty(refQuest.m_leadInDialog) && refQuest.GetNextChoiceLine() != "") {
								Choice questChoice = new Choice();

								questChoice.m_line = refQuest.GetNextChoiceLine();
								questChoice.m_reactions = new List<string>();
								questChoice.m_reactions.Add("accept " + refQuest.m_name);

								if(refQuest.GetNextChoicePostDialog() != "") 
									questChoice.m_reactions.Add("show " + refQuest.GetNextChoicePostDialog());
								else questChoice.m_reactions.Add("close");

								choices.Add(questChoice);
							} else if(!String.IsNullOrEmpty(refQuest.m_leadInDialog) && !String.IsNullOrEmpty(refQuest.m_leadInChoiceLine) && 
										refQuest.GetNextChoiceLine() != "") { 
								Choice leadInChoice = new Choice();

								leadInChoice.m_line = refQuest.m_leadInChoiceLine;
								leadInChoice.m_reactions = new List<string>();
								leadInChoice.m_reactions.Add("show " + refQuest.m_leadInDialog);

								choices.Add(leadInChoice);
								m_introducingQuest = refQuest;
							}
						}
					}
			}

            if(m_npc.m_hasShop && m_npc.m_entity.m_inventory.Count() > 0)
                choices.Add(m_shopChoice);

            if(choices.Count == 0) { ChangeToStartingDialog(true); return; }

			choices.Add(m_closeChoice);

			m_dialogWindow.DisplayChoices(choices);
			m_selectingChoice = true;
		} else {
            GameObject obj = EventSystem.current.currentSelectedGameObject;

            if(obj) {
                Button button = obj.GetComponent<Button>();

                if(button) button.onClick.Invoke();
            }
        }
	}

    public string GetFormattedLine(string p_line) {
        return Game.m_languages.GetLine(p_line).Replace("{player}", m_interactor.gameObject.name);
    }

	// TODO: add gradual display + eventual disappearance
	public void Display(string p_text) {
		if(!m_dialogWindow) {
            m_dialogWindow = Instantiate(m_dialogTemplate, m_dialogTemplate.transform.parent).GetComponent<DialogWindow>();
			m_dialogWindow.Setup(this, gameObject.name); // TODO: Change to NPC name lol
		}

		m_dialogWindow.Display(p_text);

        MenuHandler.Instance.m_handlingPlayer = m_interactor.m_rewiredPlayer;
    }

    public void DisplayShop() {
        if(m_shopWindow || m_dialogWindow) ChangeToStartingDialog(false);

        if(!m_npc.m_entity.m_inventory.m_itemTooltip)
            m_npc.m_entity.m_inventory.m_itemTooltip = m_interactor.m_inventory.m_itemTooltip;

        m_shopWindow = Instantiate(m_shopTemplate, m_shopTemplate.transform.parent).GetComponent<ShopWindow>();

        MenuHandler.Instance.m_handlingPlayer = m_interactor.m_rewiredPlayer;

        m_shopWindow.Setup(this, m_npc.m_entity.m_inventory, gameObject.name);
    }

	private void CleanDisplay(bool p_removeWindow, bool p_wipeVariables) {
		if(!m_dialogWindow && !m_shopWindow) return;

        if(p_removeWindow) {
            if(m_dialogWindow) {
                DialogWindow.m_openedWindows.Remove(m_dialogWindow);

                Destroy(m_dialogWindow.gameObject);
                m_dialogWindow = null;
            } else if(m_shopWindow) {
                ShopWindow.m_openedWindows.Remove(m_shopWindow);

                Destroy(m_shopWindow.gameObject);
                m_shopWindow = null;
            }

            if(p_wipeVariables) {
                m_interactor.m_interactingWithNPC = false;
                m_interactor = null;
                m_introducingQuest = null;
                MenuHandler.Instance.m_handlingPlayer = null;
            }
        } else if(m_dialogWindow) m_dialogWindow.Clear();
	}

	public void ChangeToStartingDialog(bool p_wipeVariables) {
        m_lastReset = Time.unscaledTime;
		ChangeDialog(m_startingDialog, false, p_wipeVariables);
	}

	public void ChangeDialog(Dialog p_dialog, bool p_display, bool p_wipeVariables) {
		m_currentDialog = p_dialog;
		m_currentDialogLine = 0;
		m_selectingChoice = false;

		CleanDisplay(!p_display, p_wipeVariables);

		if(p_display) Interact(m_interactor);
	}
}
