using System;

public class Equipment : Inventory {

	public void SetEntity(Entity p_entity) { 
		m_entity = p_entity;

		foreach(Item item in m_items)
			if(item.m_item != null) item.m_holder = p_entity;
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

		if(mainHand.m_rightClickPattern || p_leftClick) return mainHand;

		return (Weapon) Get(EquipmentSlot.OffHand).m_item;
	}
}

public enum EquipmentSlot {
	MainHand, OffHand, Helmet, Chestpiece, Leggings, Boots, Trinket1, Trinket2, Ring1, Ring2
}