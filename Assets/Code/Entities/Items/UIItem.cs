using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIItem : ClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {

	[Tooltip("The item represented in the UI")]
	public Item m_item;

	[Tooltip("The duplicate image used for dragging the item around instead of the original")]
	public Image m_ghost;

	private bool m_validDrop;

	void Awake() {
		m_ghost.raycastTarget = false;
		m_ghost.enabled = false;
	}

	void OnDisable() { 
		if(m_item.m_inventory.m_itemTooltip.gameObject.activeSelf)
			HideTooltip();
	}

	public void ShowTooltip() {
		if(!m_item.m_item) return;

		m_item.m_inventory.m_itemTooltip.SetItem(m_item);
		m_item.m_inventory.m_itemTooltip.Show();
	}

	public void HideTooltip() {
		if(!m_item.m_item) return;

		m_item.m_inventory.m_itemTooltip.Hide();
	}

	protected override void OnLeftDoubleClick(GameObject p_clicked) { 
		if(!m_item.m_item) return;

		if(!m_item.m_inventory) { // if it's on the floor we want to pick it up
			HideTooltip();
			// TODO: double clicking an item from the floor
		} else {
			Equipment entityEquipment = m_item.m_holder.m_equipment;
			Inventory entityInventory = m_item.m_holder.m_inventory;

			if (m_item.m_inventory is Equipment) { // we want to unequip
				if(entityInventory.IsFull()) return;
				HideTooltip();

				int index = m_item.m_inventoryIndex;
				bool success = entityInventory.Add(entityEquipment.Take(m_item, m_item.m_amount));

				if(success && entityInventory.m_uiItems.Length > 0) SwapInfo(entityInventory.m_uiItems[m_item.m_inventoryIndex]);
				else if(success) HideInfo(entityEquipment.m_items[index]);
				else if(!success) { entityEquipment.SetAtIndex(m_item, index); return; }
			} else if(m_item.m_inventory.m_entity) { // if it's a normal inventory, we want to equip
				HideTooltip();
				List<EquipmentSlot> possibleSlots = new List<EquipmentSlot>();

				foreach(EquipmentSlot slotID in m_item.m_item.m_equipmentSlots)
					if(!entityEquipment.Get(slotID).m_item) possibleSlots.Add(slotID);

				bool success = false;
				int index = m_item.m_inventoryIndex;

				if(possibleSlots.Count > 0) 
					success = entityEquipment.SetAtIndex(m_item.m_inventory.Take(m_item, m_item.m_amount), (int)possibleSlots[0]);
				else success = entityEquipment.Swap(entityEquipment.Get(m_item.m_item.m_equipmentSlots[0]), m_item);

				if(success && entityEquipment.m_uiItems.Length > 0)
					SwapInfo(entityEquipment.m_uiItems[m_item.m_inventoryIndex]);
				else if(success) HideInfo(entityInventory.m_items[index]);
				else if(!success) { entityInventory.SetAtIndex(m_item, index); return; }
			}
		}

		m_item.m_inventory.m_onInventoryActionEvent.Raise();
	}

	public void OnBeginDrag(PointerEventData p_eventData) {
		if(!m_item.m_item) return;

		m_ghost.transform.position = Input.mousePosition;
		m_ghost.enabled = true;
		m_ghost.transform.SetParent(GetComponentInParent<Canvas>().transform);
	}

	public void OnDrag(PointerEventData p_eventData) {
		if(!m_item.m_item) return;

		m_ghost.transform.position += (Vector3) p_eventData.delta;

		foreach(GameObject hovered in p_eventData.hovered)
			if(hovered.name.Contains("Canvas")) {
				Canvas hover = hovered.GetComponent<Canvas>();

				m_ghost.transform.SetParent(hover.transform);

				break;
			}
	}

	public void OnEndDrag(PointerEventData p_eventData) {
		m_ghost.enabled = false;
		m_ghost.transform.SetParent(gameObject.transform);

		// making sure that when destroying, you can only drop out of a window and not in the middle of the UI and destroy
		if(m_item.m_item && !m_validDrop && !p_eventData.hovered.Find(h => h.name.Contains("Canvas")) && m_item.m_inventory.m_itemDestroyModal) {
			Modal modal = m_item.m_inventory.m_itemDestroyModal;

			if(m_item.m_inventory.m_onItemDestroyEvent)
				modal.m_eventToFireOnSuccess = m_item.m_inventory.m_onItemDestroyEvent;
			else modal.m_eventToFireOnSuccess = null;

			modal.m_info = this;
			modal.m_description.text = "You are about to destroy " + m_item.m_item.m_name + ".\nAre you sure you want to do this?";
			modal.OpenModal();
		}

		m_validDrop = false;
	}

	public void OnDrop(PointerEventData p_eventData) {
		m_validDrop = true;

		GameObject draggedItem = p_eventData.pointerDrag;
		if(!draggedItem) return;

		UIItem dragged = draggedItem.GetComponent<UIItem>();
		if(!dragged || !dragged.m_item.m_inventory || !dragged.m_item.m_item) return;

		// if stacking items together
		if(m_item.m_item && dragged.m_item.m_item.m_id == m_item.m_item.m_id && m_item.m_item.m_maxStackSize - m_item.m_amount > 0) { 
			bool addSuccess = m_item.m_inventory.AddToItem(dragged.m_item, m_item);

			if(addSuccess) {
				if(dragged.m_item.m_amount == 0) {
					int index = dragged.m_item.m_inventoryIndex;
					dragged.m_item.m_inventory.Remove(dragged.m_item);
					dragged.m_item = new Item(dragged.m_item.m_inventory, index);
				}

				GetComponentInChildren<Text>().text = m_item.m_amount.ToString();

				m_item.m_inventory.m_onInventoryActionEvent.Raise();
			}

			return;
		}

		bool swapSuccess = dragged.m_item.m_inventory.Swap(dragged.m_item, m_item);

		if(swapSuccess) {
			SwapInfo(dragged);
			m_item.m_inventory.m_onInventoryActionEvent.Raise();
		}
	}

	private void SwapInfo(UIItem p_dragged) {
		// swap the images/amounts around
		Image image = GetComponent<Image>();
		Image draggedImage = p_dragged.GetComponent<Image>();
		Image otherGhost = p_dragged.m_ghost;
		Text amount = GetComponentInChildren<Text>();
		Text otherAmount = draggedImage.GetComponentInChildren<Text>();

		image.sprite = draggedImage.sprite;
		draggedImage.sprite = m_ghost.sprite;
		otherGhost.sprite = draggedImage.sprite;
		m_ghost.sprite = image.sprite;

		string tempAmount = amount.text;
		amount.text = otherAmount.text;
		otherAmount.text = tempAmount;

		Color otherColor = draggedImage.color;
		bool otherAmountEnabled = otherAmount.enabled;
		draggedImage.color = image.color;
		image.color = otherColor;
		otherAmount.enabled = amount.enabled;
		amount.enabled = otherAmountEnabled;

		Item current = m_item;
		m_item = p_dragged.m_item;
		p_dragged.m_item = current;
	}

	private void HideInfo(Item p_swapped) {
		Image image = GetComponent<Image>();
		Text amount = GetComponentInChildren<Text>();

		image.color = new Color(255, 255, 255, 0);
		amount.enabled = false;
		m_item = p_swapped;
	}
}
