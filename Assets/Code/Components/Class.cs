using UnityEngine;
using System;
using System.Collections.Generic;

// TODO: REMOVE
public class Class : MonoBehaviour {

	public Classes m_classType;

	[Tooltip("Determines the amount of each stat gained per level")]
	public StatGains m_statGains;

	[Tooltip("List of abilities learned")]
	public List<Ability> m_abilities;

	// migrate to weapons

	private Entity m_entity;
	private UnitStats m_stats;

	void Awake() { 
		m_stats = GetComponent<UnitStats>();
		m_entity = GetComponent<Entity>();

		//m_entity.m_class = this;
	}

	public void LevelUp() { 
		foreach(Stats stat in Enum.GetValues(typeof(Stats)))
			m_stats.GetStat(stat).Value += m_statGains.Random(stat);
	}
}

public enum Classes { 
	Archer, Wizard, Warrior, Priest	
}
