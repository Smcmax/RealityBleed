﻿using UnityEngine;
using System.Collections.Generic;

public abstract class Interactable : MonoBehaviour {
	
	[Tooltip("The collider within which the user can interact with this object, if null will grab the one on this GameObject")]
	public Collider2D m_interactBounds;

	[Tooltip("The renderer used to render the tooltip showing when the object is interactable")]
	public SpriteRenderer m_tooltipRenderer;

	[Tooltip("Event called when interacting with an object")]
	public GameEvent m_onInteractEvent;

	private List<Entity> m_interacters;
	private bool m_interactable = false;

	protected virtual void Awake() { 
		m_interacters = new List<Entity>();
		if(!m_interactBounds) m_interactBounds = GetComponent<Collider2D>();

		if(m_interactBounds != null)
			m_interactBounds.isTrigger = true;
	}

	void OnTriggerEnter2D(Collider2D p_collider) { 
		if(p_collider.gameObject.tag == "Player") { 
			m_interacters.Add(p_collider.gameObject.GetComponent<Entity>());
			m_tooltipRenderer.enabled = true;
			m_interactable = true;
		}
	}

	void OnTriggerExit2D(Collider2D p_collider) { 
		if(p_collider.gameObject.tag == "Player") { 
			m_interacters.Remove(p_collider.gameObject.GetComponent<Entity>());
			m_tooltipRenderer.enabled = false;
			m_interactable = false;
		}
	}

	void Update() {
		if(m_interactable && Input.GetButtonDown("Interact")) {
			m_onInteractEvent.Raise();
			Interact(m_interacters[0]);
		}
	}

	public abstract void Interact(Entity p_entity);
}
