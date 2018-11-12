using UnityEngine;
using System;

public class Equipment : Inventory {

	public const int EQUIPMENT_SIZE = 9;

	void Awake() {
		// stops tampering from the editor
		if(m_items.Length != EQUIPMENT_SIZE)
			m_items = new Item[EQUIPMENT_SIZE];
	}

	public Item Get(string p_pieceName) {
		foreach(string name in Enum.GetNames(typeof(EquipmentSlot)))
			if(name.ToLower() == p_pieceName.ToLower())
				return m_items[(int) Enum.Parse(typeof(EquipmentSlot), name, false)];

		return null;
	}
}

public enum EquipmentSlot {
	MainHand, OffHand, Helmet, Chestpiece, Leggings, Boots, Trinket, Ring1, Ring2
}