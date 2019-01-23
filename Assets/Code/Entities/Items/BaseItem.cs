using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class BaseItem : ScriptableObject {

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

	[Tooltip("The equipment slots this item fits into, none if not an equippable item")]
	public List<EquipmentSlot> m_equipmentSlots;

	[Tooltip("The name's color in the tooltip")]
	public Color m_nameColor;

	[Tooltip("The item's description, shows up in the tooltip")]
	[Multiline] public string m_description;

	public string GetSlotInfoText() { 
		string text = GetType().Name;

		if(m_equipmentSlots.Count > 0) {
			text = m_equipmentSlots[0].ToString();

			if(m_equipmentSlots.Contains(EquipmentSlot.MainHand) && m_equipmentSlots.Contains(EquipmentSlot.OffHand))
				return ((Weapon) this).IsTwoHanded() ? "Two-Hand" : "One-Hand";

			if(text.EndsWith("1") || text.EndsWith("2")) text = text.Substring(0, text.Length - 1);
		}

		return string.Concat(text.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
	}

	// this can be null
	public virtual void Use(Entity p_entity, string[] p_args) { }

	// vs equipped
	public static int[] GetStatGainDifferences(BaseItem p_item, Equipment p_equipment, EquipmentSlot p_slot) {
		Item item = p_equipment.Get(p_slot);
		if(item.m_item == p_item) return new int[UnitStats.STAT_AMOUNT];

		int[] statGainValues = GetStatGainValues(p_item);

		if(item.m_item) return statGainValues.Compare(GetStatGainValues(item.m_item));
		return statGainValues;
	}

	public static int[] GetStatGainValues(BaseItem p_item) {
		return p_item is Weapon ? ((Weapon)p_item).m_statGainValues :
				(p_item is Armor ? ((Armor)p_item).m_statGainValues : new int[UnitStats.STAT_AMOUNT]);
	}
}
