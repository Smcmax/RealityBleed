using UnityEngine;
using System.Collections.Generic;

public abstract class BaseItem : ScriptableObject {

	[Tooltip("The item's id, SHOULD BE UNIQUE")]
	public int m_id;

	[Tooltip("The item's name")]
	public string m_name;

	[Tooltip("The item's sprite")]
	public Sprite m_sprite;

	[Tooltip("The maximum amount of this item that can be stacked together in a single inventory slot")]
	[Range(1, 99)] public int m_maxStackSize;

	[Tooltip("How much this item sells for")]
	public float m_sellPrice;

	[Tooltip("The level required to equip this item")]
	[Range(0, 99)] public int m_levelRequired;

	[Tooltip("The equipment slots this item fits into, none if not an equippable item")]
	public List<EquipmentSlot> m_equipmentSlots;

	[Tooltip("The name's color in the tooltip")]
	public Color m_nameColor;

	[Tooltip("The item's description, shows up in the tooltip")]
	[Multiline] public string m_description;

	// this can be null
	public abstract void Use(Entity p_entity, string[] p_args);
}
