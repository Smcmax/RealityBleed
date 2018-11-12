using UnityEngine;
using System;
using System.Collections;

public class UnitStats : MonoBehaviour {

	public const int STAT_AMOUNT = 9;

	// base stats, no modifiers applied
	public FloatReference m_maxHP;
	public FloatReference m_maxMP;
	public FloatReference m_strength;     // STR - Damage with melee-type items/weapons
	public FloatReference m_dexterity;    // DEX - Firing speed
	public FloatReference m_intellect;    // INT - Damage with caster-type items/weapons
	public FloatReference m_constitution; // CON - Affects health regen
	public FloatReference m_wisdom;       // WIS - Affects mana regen
	public FloatReference m_defense;      // DEF - Reduces incoming damage
	public FloatReference m_speed;        // SPD - Character speed
	
	// calculate these at runtime based on gear and such
	private float[] m_modifiers;
	private Entity m_entity;

	void Awake() { 
		m_entity = GetComponent<Entity>();
		m_modifiers = new float[9]{ 0,0,0,0,0,0,0,0,0 };

		// load modifiers from equipment set
	}

	private int GetModifierId(string p_modifier){
		foreach(string name in Enum.GetNames(typeof(Stats)))
			if(name.ToLower() == p_modifier.ToLower())
				return (int) Enum.Parse(typeof(Stats), name, false);

		return -1;
	}

	public float GetModifier(string p_modifier){
		return m_modifiers[GetModifierId(p_modifier)];
	}

	public void SetModifier(string p_modifier, float p_value){
		m_modifiers[GetModifierId(p_modifier)] = p_value;
	}

	// a ttl of 0 = permanent (can still be manually removed)
	public void AddModifier(string p_modifier, float p_value, float p_ttl) { 
		m_modifiers[GetModifierId(p_modifier)] += p_value;

		if(p_ttl > 0) StartCoroutine(RemoveModifier(p_modifier, p_value, p_ttl));
	}

	public void RemoveModifier(string p_modifier, float p_value){
		m_modifiers[GetModifierId(p_modifier)] -= p_value;
	}

	// for use with the ttl-enabled AddModifier only
	private IEnumerator RemoveModifier(string p_modifier, float p_value, float p_ttl){
		yield return new WaitForSeconds(p_ttl);

		RemoveModifier(p_modifier, p_value);
	}
}

public enum Stats { 
	HP, MP, STR, DEX, INT, CON, WIS, DEF, SPD	
}
