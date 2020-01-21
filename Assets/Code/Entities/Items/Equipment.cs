using UnityEngine;
using System;
using System.Collections.Generic;

public class Equipment : Inventory {

	public override void Init(Entity p_entity) {
		base.Init(p_entity);

		m_entity.m_stats.UpdateGearModifiers(CalculateStatModifiers());
	}

	public int FindBestSwappableItemSlot(List<string> p_slots) { 
		int length = Enum.GetValues(typeof(EquipmentSlot)).Length;

		// if there are any empty slots, we return those first
		for(int i = 0; i < length; i++)
			if(p_slots.Contains(((EquipmentSlot) i).ToString()) && !Get((EquipmentSlot) i).m_item)
				return i;

		// otherwise we just return the first available slot from standard order
		for(int i = 0; i < length; i++)
			if(p_slots.Contains(((EquipmentSlot) i).ToString()))
				return i;

		return 0;
	}

	public Item Get(EquipmentSlot p_slot) {
		return m_items[(int) p_slot];
	}

	public Item Get(string p_slot) { 
		return m_items[(int) Enum.Parse(typeof(EquipmentSlot), p_slot)];
	}

	public Weapon GetWeaponHandlingClick(bool p_leftClick) { 
		Weapon mainHand = Get(EquipmentSlot.MainHand).m_item as Weapon;

		if(mainHand && (!String.IsNullOrEmpty(mainHand.m_rightClickPattern) || p_leftClick)) return mainHand;

		Weapon offHand = Get(EquipmentSlot.OffHand).m_item as Weapon;

		return offHand ? offHand : null;
	}

	public string GetShotPatternHandlingClick(bool p_leftClick) {
		Weapon mainHand = Get(EquipmentSlot.MainHand).m_item as Weapon;

		if(mainHand && p_leftClick) return mainHand.m_leftClickPattern;
		if(mainHand && !String.IsNullOrEmpty(mainHand.m_rightClickPattern)) return mainHand.m_rightClickPattern;

		Weapon offHand = Get(EquipmentSlot.OffHand).m_item as Weapon;

		return offHand ? offHand.m_leftClickPattern : "";
	}

    public override void RaiseEquipEvent(BaseItem p_item, bool p_equip, bool p_raise) {
        base.RaiseEquipEvent(p_item, p_equip, p_raise);

        m_entity.m_stats.UpdateGearModifiers(CalculateStatModifiers());
    }

    private int[] CalculateStatModifiers() { 
		int[] statModifiers = new int[m_items.Length];

		foreach(Item item in m_items) { 
			if(item.m_item != null && (item.m_item is Weapon || item.m_item is Armor)) {
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