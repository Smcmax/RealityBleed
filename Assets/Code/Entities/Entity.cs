using UnityEngine;
using System.Collections.Generic;

public class Entity : MonoBehaviour {

	[Tooltip("The movement controller")]
	public CharController m_controller;

	[Tooltip("If this entity can die")]
	public bool m_canDie;
	protected bool m_isDead;

	[Tooltip("This entity's equipment")]
	public Equipment m_equipment;

	[Tooltip("Every inventory carried by this entity")]
	public List<Inventory> m_inventories; // TODO: do something better for extra bags and shit? maybe just a simple wrapper for it

	[Tooltip("The shot pattern fired when this entity uses left click")]
	public ShotPattern m_leftClickPattern; // replace these with class patterns instead? or just keep defaults in class and copy? idk

	[Tooltip("The shot pattern fired when this entity uses right click")]
	public ShotPattern m_rightClickPattern;

	[HideInInspector] public UnitHealth m_health;
	protected Shooter m_shooter;

	void Awake() {
		m_health = GetComponent<UnitHealth>();
		m_shooter = GetComponent<Shooter>();

		if(m_shooter) m_shooter.Init(this);

		if(!(this is Player)) {
			ShotPattern[] patterns = GetComponents<CirclePattern>();

			if (patterns.Length > 0)
				for (int i = 0; i < patterns.Length; ++i)
					patterns[i].StartPattern(GetComponent<Shooter>());
		}
	}

	public void Kill() {
		if (m_canDie) {
			m_isDead = true;
			Die();
		}
	}

	protected virtual void Die() {
		Destroy(gameObject);
	}
}
