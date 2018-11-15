using System;
using UnityEngine;

public class Equipment : Inventory {

	public const int EQUIPMENT_SIZE = 9;

	[HideInInspector] public Entity m_entity;

	void Awake() {
		// stops tampering from the editor
		if(m_items.Length != EQUIPMENT_SIZE)
			m_items = new Item[EQUIPMENT_SIZE];
	}

	public void SetEntity(Entity p_entity) { 
		m_entity = p_entity;

		foreach(Item item in m_items)
			if(item.m_item != null) item.m_item.m_holder = p_entity;
	}

	private int GetSlotId(EquipmentSlot p_slot) {
		foreach(EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
			if(p_slot == slot) return (int) slot;

		return -1;
	}

	public Item Get(EquipmentSlot p_slot) {
		int slotId = GetSlotId(p_slot);

		return slotId == -1 ? null : m_items[slotId];
	}

	public Weapon GetWeaponHandlingClick(bool p_leftClick) { 
		Weapon mainHand = (Weapon) Get(EquipmentSlot.MainHand).m_item;

		if(mainHand.m_isTwoHanded || p_leftClick) return mainHand;

		return (Weapon) Get(EquipmentSlot.OffHand).m_item;
	}

	public bool Set(Item p_item, EquipmentSlot p_slot){
		if(p_item.m_item != null && !m_entity.RemoveFromInventory(p_item)) return false;

		int slotId = GetSlotId(p_slot);
		Item replaced = m_items[slotId];

		if(replaced.m_item != null)
			if(!m_entity.AddToInventory(replaced)) {
				m_entity.AddToInventory(p_item); // need to add item back since we removed it
				return false;
			}

		replaced.m_item.m_holder = null;
		p_item.m_item.m_holder = m_entity;

		m_items[slotId] = p_item;

		return true;
	}
}

public enum EquipmentSlot {
	MainHand, OffHand, Helmet, Chestpiece, Leggings, Boots, Trinket, Ring1, Ring2
}