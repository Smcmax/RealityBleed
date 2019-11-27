using UnityEngine;

[System.Serializable]
public class Drop {

	[Tooltip("The item to drop")]
	public Item m_item;

	[Tooltip("The chance to drop this item in a loot roll")]
	[Range(0, 100)] public float m_dropChance;

	[Tooltip("The minimum and maximum of this item to drop")]
	[MinMaxRange(0, 128)] public RangedInt m_dropAmount;

	public bool RollDrop() { 
		return Random.Range(0, 100) <= m_dropChance;
	}

	public void AddToInventory(Inventory p_inventory) { 
		m_item.m_amount = Random.Range(m_dropAmount.Min, m_dropAmount.Max);
		p_inventory.Add(m_item);
	}

    public void Equip(Equipment p_equipment) {
        m_item.m_amount = Random.Range(m_dropAmount.Min, m_dropAmount.Max);
        p_equipment.SetAtIndex(m_item, p_equipment.FindBestSwappableItemSlot(m_item.m_item.m_equipmentSlots));
    }
}
