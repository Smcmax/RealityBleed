using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DropTable {

    [Tooltip("The items allowed to drop from this drop table")]
    public List<Drop> m_items; 
	
	[Tooltip("How many times should this loot table be rolled?")]
	[Range(0, 24)] public int m_dropChances;

	[Tooltip("Should items be allowed to drop multiple times?")]
	public bool m_allowDuplicateDrops;

	private List<Drop> m_droppedItems;

	public void Drop(Inventory p_inventory) { 
		m_droppedItems = new List<Drop>();

		for(int i = 0; i < m_dropChances; i++)
			if(m_allowDuplicateDrops || m_droppedItems.Count < m_items.Count)
				RollTable(p_inventory);
			else break;
	}

	private void RollTable(Inventory p_inventory) {
		for(int i = 0; i < m_items.Count; i++) { 
			Drop drop = m_items[i];

			if((m_allowDuplicateDrops || !m_droppedItems.Contains(drop)) && drop.RollDrop()) { 
				if(!m_allowDuplicateDrops) m_droppedItems.Add(drop);

				drop.AddToInventory(p_inventory);
				break;
			}
		}
	}
}
