using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Damage Type")]
public class DamageType : ScriptableObject { 

	public static List<DamageType> m_damageTypes = new List<DamageType>();

	void OnEnable() {
		m_damageTypes.Add(this);
	}

	[Tooltip("The type's name")]
	public string m_name;

	[Tooltip("The type's icon")]
	public Sprite m_icon;

	[Tooltip("Color to use in tooltips")]
	public ColorReference m_nameColor;

	[Tooltip("The types against which this type fares well")]
	public List<DamageType> m_strongAgainst;

	[Tooltip("The types against which this type doesn't fare well")]
	public List<DamageType> m_weakAgainst;

	// checks the effectiveness of the local type against the compared type, -1 if not effective, 0 if neutral, 1 if effective
	public int IsEffectiveAgainst(DamageType p_type) { 
		if(m_strongAgainst.Contains(p_type)) return 1;
		else if(m_weakAgainst.Contains(p_type)) return -1;

		return 0;
	}

	public static DamageType Get(string p_name) { 
		return m_damageTypes.Find(dt => dt.m_name == p_name);
	}
}
