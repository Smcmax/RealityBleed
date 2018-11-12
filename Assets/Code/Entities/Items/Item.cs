using System;
using UnityEngine;

// add enchants maybe?

[Serializable]
public class Item {
	private static int LastGeneratedId = 0;

    [HideInInspector] public int m_id;

	[Tooltip("The durability of the item, if applicable")]
	[Range(0, 100)] public float m_durability;

	[Tooltip("The basic item representing this item")]
	public BaseItem m_item;

	public Item() {
		m_id = LastGeneratedId++;
	}
}
