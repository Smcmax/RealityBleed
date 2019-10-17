using System;
using UnityEngine;

[Serializable]
public class Armor : BaseItem {

	[Tooltip("The type of armor this item is")]
	public string m_armorType;

	[Tooltip("Stats gained when this item is equipped")]
	public int[] m_statGainValues;

	public override void Use(Entity p_entity, string[] p_args) { 
		
	}

	public ArmorType GetArmorType() { 
		ArmorType type;

		if(Enum.TryParse(m_armorType, true, out type))
			return type;

		return ArmorType.Cloth;
	}
}

public enum ArmorType { 
	Cloth, Leather, Mail, Plate
}