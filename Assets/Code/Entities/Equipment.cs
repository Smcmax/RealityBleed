using UnityEngine;

public class Equipment : Inventory {

	public void Init(Entity p_entity) {
		m_entity = p_entity;
		m_entity.m_stats.UpdateGearModifiers(CalculateStatModifiers());
	}

	public Item Get(EquipmentSlot p_slot) {
		return m_items[(int) p_slot];
	}

	public Weapon GetWeaponHandlingClick(bool p_leftClick) { 
		Weapon mainHand = (Weapon) Get(EquipmentSlot.MainHand).m_item;

		if(mainHand && (mainHand.m_rightClickPattern || p_leftClick)) return mainHand;

		Weapon offHand = (Weapon) Get(EquipmentSlot.OffHand).m_item;

		return offHand ? offHand : null;
	}

	public ShotPattern GetShotPatternHandlingClick(bool p_leftClick) {
		Weapon mainHand = (Weapon) Get(EquipmentSlot.MainHand).m_item;

		if(mainHand && p_leftClick) return mainHand.m_leftClickPattern;
		if(mainHand && mainHand.m_rightClickPattern) return mainHand.m_rightClickPattern;

		Weapon offHand = (Weapon) Get(EquipmentSlot.OffHand).m_item;

		return offHand ? offHand.m_leftClickPattern : null;
	}

	public override void RaiseInventoryEvent(bool p_raise) {
		base.RaiseInventoryEvent(p_raise);
		m_entity.m_stats.UpdateGearModifiers(CalculateStatModifiers());
	}

	private int[] CalculateStatModifiers() { 
		int[] statModifiers = new int[m_items.Length];

		foreach(Item item in m_items) { 
			if(item.m_item && (item.m_item is Weapon || item.m_item is Armor)) {
				// not sure if I can straight up cast armor to both of them to get the value, so I don't take my chances
				int[] statValues = item.m_item is Weapon ? ((Weapon) item.m_item).m_statGainValues : ((Armor) item.m_item).m_statGainValues;

				for(int i = 0; i < statValues.Length; i++)
					statModifiers[i] += statValues[i];
			}
		}

		return statModifiers;
	}
}

public enum EquipmentSlot {
	MainHand, OffHand, Helmet, Chestpiece, Leggings, Boots, Trinket1, Trinket2, Ring1, Ring2
}