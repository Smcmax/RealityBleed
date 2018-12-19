using UnityEngine;

[CreateAssetMenu(menuName = "Items/Armor")]
public class Armor : BaseItem {

	[Tooltip("The type of armor this item is")]
	public ArmorType m_armorType;

	[Tooltip("Stats gained when this item is equipped")]
	public int[] m_statGainValues;

	public override void Use(Entity p_entity, string[] p_args) { 
		
	}
}

public enum ArmorType { 
	Cloth, Leather, Mail, Plate
}