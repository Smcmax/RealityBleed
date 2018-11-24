using UnityEngine;

[CreateAssetMenu(menuName = "Items/Armor")]
public class Armor : BaseItem {

	[Tooltip("Stats gained when this item is equipped")]
	public int[] m_statGainValues;

	public override void Use(Entity p_entity, string[] p_args) { 
		
	}
}
