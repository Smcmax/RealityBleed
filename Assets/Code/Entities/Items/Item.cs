using System;
using UnityEngine;

// add enchants maybe?

[Serializable]
public class Item {

	[Tooltip("Quantity of this item in this slot")]
	[Range(0, 99)] public int m_amount;

	[Tooltip("The durability of the item, if applicable")]
	[Range(0, 100)] public float m_durability;

	[Tooltip("The sprite shown when there's no item in the slot")]
	public Sprite m_outlineSprite;

	[Tooltip("The basic item representing this item")]
	public BaseItem m_item;

	[HideInInspector] public Entity m_holder;
	[HideInInspector] public Inventory m_inventory;
	[HideInInspector] public int m_inventoryIndex;

	public Item(Inventory p_inventory, int p_index) { 
		m_holder = p_inventory.m_entity;
		m_inventory = p_inventory;
		m_inventoryIndex = p_index;
	}
}
