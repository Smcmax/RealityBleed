using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

	[Tooltip("The list of all items within this inventory")]
	public Item[] m_items;

	[Tooltip("The modal used when destroying an item")]
	public Modal m_itemDestroyModal;

	[Tooltip("The tooltip used by this inventory")]
	public ItemTooltip m_itemTooltip;

	[Tooltip("The event called when items are moved around")]
	public GameEvent m_onInventoryMoveEvent;

    [Tooltip("The event called when an item is equipped")]
    public GameEvent m_onItemEquipEvent;

    [Tooltip("The event called when a cloth item is equipped")]
    public GameEvent m_onItemClothEquipEvent;

    [Tooltip("The event called when a leather item is equipped")]
    public GameEvent m_onItemLeatherEquipEvent;

    [Tooltip("The event called when a mail item is equipped")]
    public GameEvent m_onItemMailEquipEvent;

    [Tooltip("The event called when a plate item is equipped")]
    public GameEvent m_onItemPlateEquipEvent;

    [Tooltip("The event called when a jewel item is equipped")]
    public GameEvent m_onItemJewelEquipEvent;

    [Tooltip("The event called when a weapon is equipped")]
    public GameEvent m_onItemWeaponEquipEvent;

    [Tooltip("The event called when an item is unequipped")]
    public GameEvent m_onItemUnequipEvent;

    [Tooltip("The event called when a cloth item is unequipped")]
    public GameEvent m_onItemClothUnequipEvent;

    [Tooltip("The event called when a leather item is unequipped")]
    public GameEvent m_onItemLeatherUnequipEvent;

    [Tooltip("The event called when a mail item is unequipped")]
    public GameEvent m_onItemMailUnequipEvent;

    [Tooltip("The event called when a plate item is unequipped")]
    public GameEvent m_onItemPlateUnequipEvent;

    [Tooltip("The event called when a jewel item is unequipped")]
    public GameEvent m_onItemJewelUnequipEvent;

    [Tooltip("The event called when a weapon is unequipped")]
    public GameEvent m_onItemWeaponUnequipEvent;

    [Tooltip("The event called when an item is destroyed")]
	public GameEvent m_onItemDestroyEvent;

    [Tooltip("The event called when an item is purchased")]
    public GameEvent m_boughtItemEvent;

    [Tooltip("The event called when an item is sold")]
    public GameEvent m_soldItemEvent;

	[HideInInspector] public Entity m_entity;
	[HideInInspector] public Entity m_interactor;
	[HideInInspector] public UIItem[] m_uiItems; // updates itself, null if inventory not shown on screen

	public virtual void Init(Entity p_entity) {
		m_entity = p_entity;
		UpdateItemInfo();
	}

	public Item Get(int p_id) {
		return Array.Find(m_items, i => i.m_item && i.m_item.m_id == p_id);
	}

	public Item[] GetAll(int p_id) {
		return Array.FindAll(m_items, i => i.m_item && i.m_item.m_id == p_id);
	}

	public int GetMaxStackSize(int p_id) {
		if(!Contains(p_id)) return 0;

		return Get(p_id).m_item.m_maxStackSize;
	}

	public int Count() { 
		int count = 0;

		foreach(Item item in m_items)
			if(item.m_item) count++;

		return count;
	}

	public bool IsFull() { 
		foreach(Item item in m_items)
			if(!item.m_item) return false;
		
		return true;
	}

	public int Add(Item p_item) {
		// if we can fill up existing stacks, we do it
		if(p_item.m_item && ContainsNonFull(p_item.m_item.m_id)) {
			foreach(Item item in GetAll(p_item.m_item.m_id)) { 
				AddToItem(p_item, item);

				if(m_uiItems != null && m_uiItems.Length >= item.m_inventoryIndex + 1)
					m_uiItems[item.m_inventoryIndex].UpdateInfo();

				if(p_item.m_amount == 0) return -2; // successful, but nothing to remove on this side
			}
		}

		int index = FindFirstEmpty();

		if(index >= 0) {
			m_items[index] = p_item;
			SetEntity(m_items[index]);
			m_items[index].m_inventory = this;
			m_items[index].m_inventoryIndex = index;

			if(m_uiItems != null && m_uiItems.Length >= index + 1) {
				m_uiItems[index].m_item = m_items[index];
				m_uiItems[index].UpdateInfo();
			}

			return index;
		}

		return -1;
	}

	public int FindFirstEmpty() {
		for(int i = 0; i < m_items.Length; ++i)
			if(!m_items[i].m_item) return i;

		return -1;
	}

	public bool AddToItem(Item p_item, Item p_itemToAddTo) { 
		return AddToItem(p_item, p_itemToAddTo, p_item.m_amount);
	}

	public bool AddToItem(Item p_item, Item p_itemToAddTo, int p_amount) {
		if(p_item.m_item && p_itemToAddTo.m_item && p_item.m_item.m_id == p_itemToAddTo.m_item.m_id) {
			int maxStack = p_itemToAddTo.m_item.m_maxStackSize;

			if(p_amount < maxStack) {
				int amountToAdd = Mathf.Clamp(p_amount, 1, maxStack - p_amount);

				p_itemToAddTo.m_amount += amountToAdd;
				p_item.m_amount -= amountToAdd;
			} else return false;

			return true;
		} else return false;
	}

	public bool SetAtIndex(Item p_item, int p_index) {
		Item atIndex = m_items[p_index];
		
		if(!atIndex.m_item) {
			m_items[p_index] = p_item;
		} else if(atIndex.m_item.m_id == p_item.m_item.m_id) { 
			if(atIndex.m_amount + p_item.m_amount > atIndex.m_item.m_maxStackSize) return false;

			atIndex.m_amount += p_item.m_amount;
		} else return false;

		SetEntity(m_items[p_index]);
		m_items[p_index].m_inventoryIndex = p_index;
		m_items[p_index].m_inventory = this;

		return true;
	}

	public bool Contains(Item p_item) {
		return Contains(p_item.m_item.m_id);
	}

	public bool Contains(int p_id) {
		return Array.Exists(m_items, i => i.m_item && i.m_item.m_id == p_id);
	}

	public bool ContainsNonFull(Item p_item) { 
		return ContainsNonFull(p_item.m_item.m_id);
	}

	public bool ContainsNonFull(int p_id) {
		return Array.Exists(m_items, i => i.m_item && i.m_item.m_id == p_id && i.m_amount < i.m_item.m_maxStackSize);
	}

	// swaps across inventories and respects equipment limitations
	public bool Swap(Item p_first, Item p_second) {
		bool isFirstEquipped = p_first.m_inventory is Equipment;
		bool isSecondEquipped = p_second.m_inventory is Equipment;
		int firstIndex = p_first.m_inventoryIndex;
		int secondIndex = p_second.m_inventoryIndex;

		if(firstIndex == -1 || secondIndex == -1) return false;

		// check if items are compatible in their respective slots
		if(isFirstEquipped && 
            (p_second.m_item && 
            !p_second.m_item.m_equipmentSlots.Contains(((EquipmentSlot) firstIndex).ToString()))) 
            return false;
		if(isSecondEquipped && 
            (p_first.m_item && 
            !p_first.m_item.m_equipmentSlots.Contains(((EquipmentSlot) secondIndex).ToString()))) 
            return false;

		p_first.m_inventory.RemoveAt(firstIndex);
		p_second.m_inventory.RemoveAt(secondIndex);

		Inventory firstInv = p_first.m_inventory;

		p_second.m_inventory.SetAtIndex(p_first, secondIndex);
		firstInv.SetAtIndex(p_second, firstIndex);

		return true;
	}

	public Item[] TakeAll(Item p_item, int p_amount) { 
		return TakeAll(p_item.m_item.m_id, p_amount);
	}

	public Item[] TakeAll(int p_id, int p_amount) {
		if(!Contains(p_id)) return new Item[]{};

		Item[] items = GetAll(p_id);

		if(items.Length == 0) return new Item[]{};

		List<Item> itemsTaken = new List<Item>();
		Item currentItem = null;
		int maxStack = items[0].m_item.m_maxStackSize;
		int totalTaken = 0;

		foreach(Item item in items) {
            if(currentItem == null) {
                currentItem = new Item(this, -1);
                currentItem.UpdateItemRef(item.m_item.m_name);
                itemsTaken.Add(currentItem);
            }

			int takenAmount = item.m_amount;

            while(takenAmount + currentItem.m_amount > maxStack) {
                takenAmount = maxStack - currentItem.m_amount;
                currentItem.m_amount += takenAmount;
                totalTaken += takenAmount;
                item.m_amount -= takenAmount;

                currentItem = new Item(this, -1);
                currentItem.UpdateItemRef(item.m_item.m_name);
                itemsTaken.Add(currentItem);
                takenAmount = item.m_amount;
            }

			if(takenAmount + totalTaken > p_amount)
				takenAmount = p_amount - totalTaken;
			
			currentItem.m_amount += takenAmount;
			totalTaken += takenAmount;
            item.m_amount -= takenAmount;

            if(item.m_amount <= 0)
                RemoveAt(item.m_inventoryIndex);

            if(m_uiItems != null && m_uiItems.Length >= item.m_inventoryIndex + 1)
                m_uiItems[item.m_inventoryIndex].UpdateInfo();

            if(totalTaken >= p_amount) break;
		}

		return itemsTaken.ToArray();
	}

	public void RemoveAt(int p_index) {
        if(m_items.Length - 1 >= p_index) {
            m_items[p_index] = new Item(this, p_index);
            m_uiItems[p_index].m_item = m_items[p_index];
            m_uiItems[p_index].UpdateInfo();
        }
	}

	public void Remove(Item p_item) {
        if(p_item.m_inventoryIndex != -1 && m_items.Length - 1 >= p_item.m_inventoryIndex)
            RemoveAt(p_item.m_inventoryIndex);
	}

	public bool RemoveAll(Item p_item) {
		return RemoveAll(p_item.m_item.m_id);
	}

	// note: removes ALL occurences of this item
	public bool RemoveAll(int p_id) {
		int index = Array.FindIndex(m_items, i => i.m_item && i.m_item.m_id == p_id);
		int removed = 0;

		while(index != -1) {
            RemoveAt(index);
            removed++;
			index = Array.FindIndex(m_items, i => i.m_item && i.m_item.m_id == p_id);
		}

		return removed == 0 ? false : true;
	}

	public void Clear() {
		for(int i = 0; i < m_items.Length; ++i) {
            RemoveAt(i);
            SetEntity(m_items[i]);
		}
	}

	public void SetEntity(Item p_item) {
		p_item.m_holder = m_entity ? m_entity : null;
	}

	public void UpdateItemInfo() {
		if(m_items == null) return;

		for(int i = 0; i < m_items.Length; ++i) {
			Item item = m_items[i];

			item.m_inventory = this;
			item.m_inventoryIndex = i;
			SetEntity(item);
		}
	}

    public void RefreshUIItems() {
        foreach(UIItem item in m_uiItems)
            item.UpdateInfo();
    }

	public virtual void RaiseInventoryMoveEvent() { 
		m_onInventoryMoveEvent.Raise();
	}

    public virtual void RaiseEquipEvent(BaseItem p_item, bool p_equip, bool p_raise) {
        if(p_raise) {
            string type = p_item is Armor ? ((Armor) p_item).m_armorType.ToLower() : 
                                            (p_item is Weapon ? "weapon" : "");
            GameEvent ev = p_equip ? m_onItemEquipEvent : m_onItemUnequipEvent;

            switch(type) {
                case "cloth": ev = p_equip ? m_onItemClothEquipEvent : m_onItemClothUnequipEvent; break;
                case "leather": ev = p_equip ? m_onItemLeatherEquipEvent : m_onItemLeatherUnequipEvent; break;
                case "mail": ev = p_equip ? m_onItemMailEquipEvent : m_onItemMailUnequipEvent; break;
                case "plate": ev = p_equip ? m_onItemPlateEquipEvent : m_onItemPlateUnequipEvent; break;
                case "jewel": ev = p_equip ? m_onItemJewelEquipEvent : m_onItemJewelUnequipEvent; break;
                case "weapon": ev = p_equip ? m_onItemWeaponEquipEvent : m_onItemWeaponUnequipEvent; break;
                default: break;
            }

            ev.Raise();
        }
    }

    public virtual void RaiseTradeEvent(bool p_sold) {
        if(p_sold && m_soldItemEvent) m_soldItemEvent.Raise();
        else if(!p_sold && m_boughtItemEvent) m_boughtItemEvent.Raise();
    }
}