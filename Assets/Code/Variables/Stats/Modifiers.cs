using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Existing modifiers
 * 
 * - Every DamageType has its own modifier in this format: TypeName-Resistance
 * 
 */
public class Modifiers : MonoBehaviour {

	private Dictionary<string, float> m_modifiers;

	void Awake() { 
		m_modifiers = new Dictionary<string, float>();
	}

	// a ttl of 0 = permanent (can still be manually removed)
	public void AddToModifier(string p_modifier, float p_value, float p_ttl) {
		if(!m_modifiers.ContainsKey(p_modifier)) m_modifiers.Add(p_modifier, p_value);
		else m_modifiers[p_modifier] += p_value;

		if(p_ttl > 0) StartCoroutine(RemoveModifierCoroutine(p_modifier, p_value, p_ttl));
	}

	public void RemoveFromModifier(string p_modifier, float p_value) {
		if(m_modifiers.ContainsKey(p_modifier)) m_modifiers[p_modifier] -= p_value;
		else m_modifiers.Add(p_modifier, -p_value);
	}

	// for use with the ttl-enabled AddModifier only
	private IEnumerator RemoveModifierCoroutine(string p_modifier, float p_value, float p_ttl) {
		yield return new WaitForSeconds(p_ttl);

		RemoveFromModifier(p_modifier, p_value);
	}

	public float GetModifier(string p_modifier) { 
		if(m_modifiers.ContainsKey(p_modifier)) return m_modifiers[p_modifier];
		else return 0;
	}

	public void SetModifier(string p_modifier, float p_value) { 
		if(m_modifiers.ContainsKey(p_modifier)) m_modifiers[p_modifier] = p_value;
		else m_modifiers.Add(p_modifier, p_value);
	}
}