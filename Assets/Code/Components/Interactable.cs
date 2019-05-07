using UnityEngine;
using System.Collections.Generic;

public abstract class Interactable : MonoBehaviour {
	
	[Tooltip("The collider within which the user can interact with this, if null will grab the one on this GameObject")]
	public Collider2D m_interactBounds;

	[Tooltip("The renderer used to render the tooltip showing when this is interactable")]
	public SpriteRenderer m_tooltipRenderer;

	[Tooltip("Should the tooltip always show?")]
    public bool m_showTooltipAtAllTimes;

	[Tooltip("Event called when interacting")]
	public GameEvent m_onInteractEvent;

	private List<Player> m_interactors;

	protected virtual void Awake() {
		m_interactors = new List<Player>();

		if(m_interactBounds)
			m_interactBounds.isTrigger = true;

		if(m_tooltipRenderer && m_showTooltipAtAllTimes)
			m_tooltipRenderer.enabled = true;
	}

	void OnTriggerEnter2D(Collider2D p_collider) { 
		if(p_collider.gameObject.tag == "Player") {
			m_interactors.Add(p_collider.gameObject.GetComponent<Player>());

			if(m_tooltipRenderer && !m_tooltipRenderer.enabled) 
				m_tooltipRenderer.enabled = true;
		}
	}

	void OnTriggerExit2D(Collider2D p_collider) { 
		if(p_collider.gameObject.tag == "Player") {
			Player player = p_collider.gameObject.GetComponent<Player>();

			m_interactors.Remove(player);
			if(m_tooltipRenderer && m_interactors.Count == 0 && !m_showTooltipAtAllTimes) 
				m_tooltipRenderer.enabled = false;

			OutOfRange(player);
		}
	}

	void Update() {
        if(Time.timeScale == 0f) return;

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
