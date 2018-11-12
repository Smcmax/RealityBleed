using UnityEngine;
using System;

public class Inventory : MonoBehaviour {

	[Tooltip("The list of all items within this inventory")]
	public Item[] m_items;

	public Item Get(int p_id) {
		return Array.Find(m_items, i => i.m_id == p_id);
	}

	public bool Add(Item p_item) {
		for(int i = 0; i < m_items.Length; ++i)
			if(m_items[i].m_item == null) {
				m_items[i] = p_item;

				return true;
			}

		return false;
	}

	public bool Contains(Item p_item) {
		return Contains(p_item.m_id);
	}

	public bool Contains(int p_id) {
		return Array.Exists(m_items, i => i.m_id == p_id);
	}

	public bool Remove(Item p_item) {
		return Remove(p_item.m_id);
	}

	public bool Remove(int p_id) {
		int index = Array.FindIndex(m_items, i => i.m_id == p_id);

		if(index >= 0) m_items[index] = new Item();

		return index == -1 ? false : true;
	}

	public void Clear() {
		for(int i = 0; i < m_items.Length; ++i)
			m_items[i].m_item = null;
	}
}
