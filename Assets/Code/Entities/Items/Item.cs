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
	public SerializableSprite m_outlineSprite;

	[Tooltip("The basic item reference representing this item")]
	public string m_itemRef;

	[HideInInspector] private BaseItem item;
	[HideInInspector] public BaseItem m_item { 
		get {
			if(item == null) UpdateItemRef();

			return item;
		} set { 
			item = value;
		}
	}
	[HideInInspector] public Entity m_holder;
	[HideInInspector] public Inventory m_inventory;
	[HideInInspector] public int m_inventoryIndex;

	public Item(Inventory p_inventory, int p_index) {
		m_item = BaseItem.Get(m_itemRef);
		m_holder = p_inventory.m_entity;
		m_inventory = p_inventory;
		m_inventoryIndex = p_index;
	}

	public void UpdateItemRef() { 
		UpdateItemRef(m_itemRef);
	}

	// this is mostly there just in case the ref needs to be updated
	public void UpdateItemRef(string p_itemRef) {
		m_itemRef = p_itemRef;
		m_item = BaseItem.Get(m_itemRef);
	}
}
