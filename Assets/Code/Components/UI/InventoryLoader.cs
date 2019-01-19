using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryLoader : MonoBehaviour {

	[Tooltip("The slot prefab to use when creating the inventory, make sure it has an item with an amount as a child")]
	public GameObject m_slot;

	[Tooltip("If the panel is to resize to fit the inventory, put it here")]
	public Image m_panelImage;

	[Tooltip("The inventory to load in")]
	public Inventory m_inventory;

	[Tooltip("How many items to fit in per row")]
	[Range(0, 16)] public int m_itemsPerRow;

	[Tooltip("The inventory panel's border size")]
	[Range(0, 32)] public float m_inventoryBorderSize;

	[Tooltip("The amount of padding to add around slots")]
	[Range(0, 5)] public float m_padding;

	[Tooltip("The index to start displaying at. This value is inclusive, meaning it will process from array[start]")]
	public int m_startIndex;

	[Tooltip("The index to stop displaying at, 0 = end. This value is inclusive, meaning it will process up to array[end]")]
	public int m_endIndex;

	void OnEnable() { 
		Load();
	}

	void OnDisable() { 
		int children = transform.childCount;

		for(int i = children - 1; i >= 0; --i)
			Destroy(transform.GetChild(i).gameObject);

		m_inventory.m_uiItems = new UIItem[0];
	}

	public void Load() {
		Item[] items = m_inventory.m_items;
		if(m_inventory.m_uiItems.Length == 0) m_inventory.m_uiItems = new UIItem[items.Length];

		if(m_endIndex > 0) {
			items = new Item[m_endIndex + 1 - m_startIndex];

			for(int i = 0; i <= m_endIndex - m_startIndex; ++i)
				items[i] = m_inventory.m_items[i + m_startIndex];
		}

		int cols = m_itemsPerRow;
		int rows = Mathf.CeilToInt((float) items.Length / (float) cols); // casting to float to make sure no truncation happens
		Image slotImage = m_slot.GetComponent<Image>();
		float slotWidth = slotImage.rectTransform.rect.width;
		float slotHeight = slotImage.rectTransform.rect.height;

		if(m_panelImage) {
			float invWidth = m_panelImage.rectTransform.rect.width;
			float invHeight = m_panelImage.rectTransform.rect.height;
			float calculatedInvWidth = cols * (slotWidth + m_padding) + m_inventoryBorderSize * 2 + m_padding / 2;
			float calculatedInvHeight = rows * (slotHeight + m_padding) + m_inventoryBorderSize * 2 + m_padding / 2;

			m_panelImage.rectTransform.sizeDelta = new Vector2(calculatedInvWidth, calculatedInvHeight);
		}

		for(int row = 0; row < rows; ++row) { 
			for(int col = 0; col < cols; ++col) { 
				if(row * cols + col >= items.Length) return;

				Item item = items[row * cols + col];
				GameObject slot = Instantiate(m_slot, gameObject.transform);
				RectTransform rect = slot.GetComponent<RectTransform>();
				float border = m_inventoryBorderSize + m_padding;

				rect.anchoredPosition = new Vector2(border * 2 + (col * (slotWidth + m_padding)),
															-(border * 2 + (row * (slotHeight + m_padding))));

				UIItem uiItem = slot.GetComponentInChildren<UIItem>();
				uiItem.m_loader = this;

				if(item.m_item && item.m_amount > 0) LoadItem(uiItem, item);
				else if(item.m_outlineSprite) LoadOutline(uiItem, item);
				else LoadEmptyItem(uiItem, item);

				m_inventory.m_uiItems[row * cols + col + m_startIndex] = uiItem;
			}
		}
	}

	public void LoadItem(UIItem p_uiItem, Item p_item) {
		Image image = p_uiItem.GetComponent<Image>();
		Text amount = p_uiItem.GetComponentInChildren<Text>();

		p_uiItem.m_item = p_item;
		image.sprite = p_item.m_item.m_sprite;
		p_uiItem.m_ghost.sprite = image.sprite;

		image.color = new Color(255, 255, 255, 255);
		p_uiItem.m_ghost.color = new Color(255, 255, 255, 255);

		if(p_item.m_amount == 1 && p_item.m_item.m_maxStackSize == 1) amount.text = "";
		else amount.text = p_item.m_amount.ToString();
	}

	public void LoadOutline(UIItem p_uiItem, Item p_item) {
		Image image = p_uiItem.GetComponent<Image>();
		Text amount = p_uiItem.GetComponentInChildren<Text>();

		image.sprite = p_item.m_outlineSprite;
		image.color = new Color(255, 255, 255, 255);
		amount.text = "";
	}

	public void LoadEmptyItem(UIItem p_uiItem, Item p_item) {
		Image image = p_uiItem.GetComponent<Image>();

		p_uiItem.m_item = p_item;
		image.color = new Color(255, 255, 255, 0);
		p_uiItem.GetComponentInChildren<Text>().enabled = false;
	}
}
