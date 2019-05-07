using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class NPC : Interactable {

	[HideInInspector] public Entity m_entity;
	[HideInInspector] public DialogueController m_dialogue;

	void Start() {
		m_entity = GetComponent<Entity>();
		m_entity.m_npc = this;
	}

    public override void Interact(Entity p_entity) {
		if(m_dialogue) m_dialogue.Interact(p_entity);
	}

    public override void OutOfRange(Entity p_entity) {
		if(m_dialogue) m_dialogue.ChangeToStartingDialogue();
	}

	public void Die() {
		if(m_dialogue) {
			m_dialogue.ChangeToStartingDialogue();
			Destroy(m_dialogue);
		}
		
		if(m_tooltipRenderer) Destroy(m_tooltipRenderer);
		if(m_interactBounds) Destroy(m_interactBounds);
		Destroy(this);
	}
}
