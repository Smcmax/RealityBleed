using System;

public class Equipment : Inventory {

	public Item Get(EquipmentSlot p_slot) {
		return m_items[(int) p_slot];
	}

	public Weapon GetWeaponHandlingClick(bool p_leftClick) { 
		Weapon mainHand = (Weapon) Get(EquipmentSlot.MainHand).m_item;

		if(mainHand.m_rightClickPattern || p_leftClick) return mainHand;

		return (Weapon) Get(EquipmentSlot.OffHand).m_item;
	}

	public ShotPattern GetShotPatternHandlingClick(bool p_leftClick) {
		Weapon mainHand = (Weapon) Get(EquipmentSlot.MainHand).m_item;

		if(p_leftClick) return mainHand.m_leftClickPattern;

		return mainHand.m_rightClickPattern ? mainHand.m_rightClickPattern : ((Weapon) Get(EquipmentSlot.OffHand).m_item).m_leftClickPattern;
	}
}

public enum EquipmentSlot {
	MainHand, OffHand, Helmet, Chestpiece, Leggings, Boots, Trinket1, Trinket2, Ring1, Ring2
}