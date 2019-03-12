using UnityEngine;
using System.Collections.Generic;

public abstract class Interactable : MonoBehaviour {
	
	[Tooltip("The collider within which the user can interact with this object, if null will grab the one on this GameObject")]
	public Collider2D m_interactBounds;

	[Tooltip("The renderer used to render the tooltip showing when the object is interactable")]
	public SpriteRenderer m_tooltipRenderer;

	[Tooltip("Event called when interacting with an object")]
	public GameEvent m_onInteractEvent;

	private List<Player> m_interactors;

	protected virtual void Awake() {
		m_interactors = new List<Player>();

		if(m_interactBounds)
			m_interactBounds.isTrigger = true;
	}

	void OnTriggerEnter2D(Collider2D p_collider) { 
		if(p_collider.gameObject.tag == "Player") {
			m_interactors.Add(p_collider.gameObject.GetComponent<Player>());
			if(m_tooltipRenderer) m_tooltipRenderer.enabled = true;
		}
	}

	void OnTriggerExit2D(Collider2D p_collider) { 
		if(p_collider.gameObject.tag == "Player") {
			Player player = p_collider.gameObject.GetComponent<Player>();

			m_interactors.Remove(player);
			if(m_tooltipRenderer && m_interactors.Count == 0) m_tooltipRenderer.enabled = false;

			OutOfRange(player);
		}
	}

	void Update() {
		if(m_interactors.Count > 0) { 
			foreach(Player player in m_interactors)
				if(player.m_rewiredPlayer.GetButtonDown("Interact")) {
					if(m_onInteractEvent) m_onInteractEvent.Raise();
					Interact(player);
				}
		}
	}

	public abstract void Interact(Entity p_entity);
	public abstract void OutOfRange(Entity p_entity);
}
