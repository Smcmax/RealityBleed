using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

[Serializable]
public class BaseItem {

	public static int ITEM_ID = 0;

	[Tooltip("All included items in the game")]
	public static List<BaseItem> m_items = new List<BaseItem>();

	[Tooltip("All externally loaded items")]
	public static List<BaseItem> m_externalItems = new List<BaseItem>();

	[Tooltip("Internal and external items in a single list. Note that if an external and an internal item have the same name, the external will load over it")]
	public static List<BaseItem> m_combinedItems = new List<BaseItem>();

	[Tooltip("Are we using external items on top of the internal ones?")]
	public static bool m_useExternalItems = true; // TODO: change to reflect modded usage instead of a hard true

	[Tooltip("The item's name")]
	public string m_name;

	[Tooltip("The item's type (Weapon, Armor, Item)")]
	public string m_type;

	[Tooltip("The item's sprite")]
	public SerializableSprite m_sprite;

	[Tooltip("The maximum amount of this item that can be stacked together in a single inventory slot")]
	public int m_maxStackSize;

	[Tooltip("How much this item sells for (base price without markups)")]
	public int m_sellPrice;

	[Tooltip("The equipment slots this item fits into, none if not an equippable item")]
	public List<string> m_equipmentSlots;

	[Tooltip("The name's color in the tooltip")]
	public ColorReference m_nameColor;

	[Tooltip("The item's description, shows up in the tooltip")]
	public string m_description;

	[Tooltip("The item's id")]
	[NonSerialized] public int m_id;

	public static void LoadAll() {
		m_items.Clear();
		m_externalItems.Clear();
		m_combinedItems.Clear();

        foreach(TextAsset loadedItem in Resources.LoadAll<TextAsset>("Items")) {
			BaseItem item = Load(loadedItem.text);

			item.m_id = ITEM_ID++;
			item.m_sprite.m_name = "Items/" + item.m_sprite.m_name; // TODO: check

			if(item) m_items.Add(item);
		}

        List<string> files = new List<string>();
        FileSearch.RecursiveRetrieval(Application.dataPath + "/Data/Items/", ref files);

		if(files.Count > 0)
			foreach(string file in files) {
				if(file.ToLower().EndsWith(".json")) {
					StreamReader reader = new StreamReader(file);
					BaseItem item = Load(reader.ReadToEnd());

                    if(item) {
                        item.m_id = ITEM_ID++;
                        item.m_sprite.m_name = "Items/" + item.m_sprite.m_name;
                        item.m_sprite.m_internal = false;

                        m_externalItems.Add(item);
                    }

                    reader.Close();
                }
			}

		foreach(BaseItem item in m_items) { 
			BaseItem external = m_externalItems.Find(bi => bi.m_name == item.m_name);

			if(external) m_combinedItems.Add(external);
			else m_combinedItems.Add(item);
		}

		if(m_externalItems.Count > 0)
			foreach(BaseItem external in m_externalItems)
				if(!m_items.Exists(bi => bi.m_name == external.m_name))
					m_combinedItems.Add(external);
	}

	private static BaseItem Load(string p_json) { 
		BaseItem item = JsonUtility.FromJson<BaseItem>(p_json);
		Type type = null;

        if(!item) return null;

		switch(item.m_type.ToLower()) {
			case "weapon": type = typeof(Weapon); break;
			case "armor": type = typeof(Armor); break;
			case "item": type = typeof(NormalItem); break;
		}

		if(type == null) return null;

		return (BaseItem) JsonUtility.FromJson(p_json, type);
	}

	public static BaseItem Get(string p_name) { 
		List<BaseItem> availableItems = m_useExternalItems ? m_combinedItems : m_items;
		BaseItem found = availableItems.Find(bi => bi.m_name == p_name);

		if(found) return found;
		
		return null;
	}

    public string GetDisplayName() {
        if(m_name.Contains("/")) {
            string[] split = m_name.Split('/');

            return split[split.Length - 1];
        }

        return m_name;
    }

    public int GetSellPrice(Entity p_holder) {
        if(p_holder.m_npc)
            return Mathf.RoundToInt((float) m_sellPrice * (1f + (float) p_holder.m_npc.m_saleMarkupPercentage / 100f));
        
        return m_sellPrice;
    }

	public List<EquipmentSlot> GetSlots() { 
		List<EquipmentSlot> slots = new List<EquipmentSlot>();

		if(m_equipmentSlots.Count > 0)
			foreach(string slotValue in m_equipmentSlots) {
				EquipmentSlot slot;

				if(Enum.TryParse(slotValue, true, out slot))
					slots.Add(slot);
			}

		return slots;
	}

	public string GetSlotInfoText() { 
		string text = GetType().Name;
		List<EquipmentSlot> slots = GetSlots();

		if(slots.Count > 0) {
			text = slots[0].ToString();

			if(slots.Contains(EquipmentSlot.MainHand) && slots.Contains(EquipmentSlot.OffHand))
				return ((Weapon) this).IsTwoHanded() ? "Two-Hand" : "One-Hand";

			if(text.EndsWith("1") || text.EndsWith("2")) text = text.Substring(0, text.Length - 1);
		}

		return string.Concat(text.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
	}

	// this can be null
	public virtual void Use(Entity p_entity, string[] p_args) { }

	// vs equipped
	public static int[] GetStatGainDifferences(BaseItem p_item, Equipment p_equipment, string p_slot) {
		Item item = p_equipment.Get(p_slot);
		if(item.m_item == p_item) return new int[UnitStats.STAT_AMOUNT];

		int[] statGainValues = GetStatGainValues(p_item);

		if(item.m_item != null) return statGainValues.Compare(GetStatGainValues(item.m_item));
		return statGainValues;
	}

	public static int[] GetStatGainValues(BaseItem p_item) {
		return p_item is Weapon ? ((Weapon)p_item).m_statGainValues :
				(p_item is Armor ? ((Armor)p_item).m_statGainValues : new int[UnitStats.STAT_AMOUNT]);
	}

	public static implicit operator bool(BaseItem p_instance) {
		return p_instance != null;
	}
}
