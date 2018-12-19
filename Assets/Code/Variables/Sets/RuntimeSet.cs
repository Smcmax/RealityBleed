﻿using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeSet<T> : ScriptableObject {

	public List<T> m_items = new List<T>();

	public void Add(T p_item) {
		if(!m_items.Contains(p_item)) m_items.Add(p_item);
	}

	public void Remove(T p_item) {
		if(m_items.Contains(p_item)) m_items.Remove(p_item);
	}

	public int Length() {
		return m_items.Count;
	}
}
