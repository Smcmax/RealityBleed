using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Rewired.Integration.UnityUI;

public class UIItem : ClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {

	[Tooltip("The item represented in the UI")]
	public Item m_item;

	[Tooltip("The duplicate image used for dragging the item around instead of the original")]
	public Image m_ghost;
	private TextMeshProUGUI m_ghostAmount;

	public static Transform GhostCanvas;
	public static UIItem HeldItem; // not dragged, held
	public static Player Holder;

	[HideInInspector] public InventoryLoader m_loader;

	private bool m_validDrop;

	void Awake() {
		m_ghost.raycastTarget = false;
		m_ghost.gameObject.SetActive(false);
		m_ghostAmount = m_ghost.GetComponentInChildren<TextMeshProUGUI>();
		m_ghostAmount.raycastTarget = false;
	}

	void OnDisable() { 
		if(!m_item.m_item || !m_item.m_inventory) return;

		if(m_item.m_inventory.m_itemTooltip.gameObject.activeSelf)
			HideTooltip();

		if(this == HeldItem) KillHeldItem();
	}

	public void ShowTooltip() {
		if(!m_item.m_item) return;

		m_item.m_inventory.m_itemTooltip.SetItem(m_item);
	}

	public void HideTooltip() {
		if(!m_item.m_item) return;

		m_item.m_inventory.m_itemTooltip.Hide();
	}

	public void MoveItem(Vector3 p_position) {
		HeldItem.m_ghost.transform.position = p_position;

		if(HeldItem.m_item.m_inventory.m_itemDestroyModal && Holder.m_rewiredPlayer.GetButtonDown("UIInteract1") &&
			!Game.m_rewiredEventSystem.IsPointerOverGameObject(RewiredPointerInputModule.kMouseLeftId))
			HeldItem.OpenDestructionModal();
	}

	private void OpenDestructionModal() {
		Modal modal = m_item.m_inventory.m_itemDestroyModal;

		if(m_item.m_inventory.m_onItemDestroyEvent)
			modal.m_eventToFireOnSuccess = m_item.m_inventory.m_onItemDestroyEvent;
		else modal.m_eventToFireOnSuccess = null;

		modal.m_info = this;
		modal.m_description.text = Game.m_languages.FormatKeys("You are about to destroy {0}.\nAre you sure you want to do this?", m_item.m_item.m_name);
		modal.OpenModal();
	}

	private void CloseDestructionModal() {
		if(!m_item.m_inventory) return;
        if(Game.m_rewiredEventSystem.IsPointerOverGameObject(RewiredPointerInputModule.kMouseLeftId) &&
            m_item.m_inventory.m_itemDestroyModal && m_item.m_inventory.m_itemDestroyModal.gameObject.activeSelf) {
            m_item.m_inventory.m_itemDestroyModal.CloseModal();
        }
	}

	protected override void OnAnyClick(GameObject p_clicked, Player p_clicker) {
		CloseDestructionModal();
	}

	protected override void OnLeftDoubleClick(GameObject p_clicked, Player p_clicker) { 
		if(!m_item.m_item) return;

		if(this == HeldItem) HideHeldItem();

		Inventory targetInventory;
		Inventory currentInventory;

		if(!m_item.m_holder) { // from container
			targetInventory = m_item.m_inventory.m_interactor.m_inventory; 
			currentInventory = m_item.m_inventory; 
		} else if(m_item.m_inventory is Equipment) { // unequip
			if(m_item.m_item is Weapon || m_item.m_item is Armor) {
				targetInventory = m_item.m_holder.m_inventory;
				currentInventory = m_item.m_holder.m_equipment;
			} else return;
		} else { // equip
			if(m_item.m_item is Weapon || m_item.m_item is Armor) {
				targetInventory = m_item.m_holder.m_equipment;
				currentInventory = m_item.m_holder.m_inventory;
			} else return;
		}

		if(!targetInventory.IsFull()) {
			HideTooltip();

			int index = m_item.m_inventoryIndex;
			int targetIndex = targetInventory is Equipment ? 
									((Equipment) targetInventory).FindBestSwappableItemSlot(m_item.m_item.m_equipmentSlots) : -1;
			bool success = false;
			bool add = false;

			if(targetIndex == -1) { targetIndex = targetInventory.Add(m_item); success = targetIndex != -1; add = true; }
			else success = targetInventory.Swap(targetInventory.m_items[targetIndex], m_item);

			// if the item needs a slot in the target inventory when adding, remove the item here, it's useless
			if(success && add && targetIndex > -1) {
				currentInventory.RemoveAt(index);
				m_item = currentInventory.m_items[index];

				HideInfo(false, null);
				// if swapping while the target inventory is open, update both sides at once
			} else if(success && targetInventory.m_uiItems.Length > 0 && targetIndex > -1) SwapInfo(targetInventory.m_uiItems[targetIndex]);
			// unsure
			else if(success && currentInventory.m_items[index].m_item && targetIndex > -1) m_loader.LoadItem(this, currentInventory.m_items[index]);
			// unsure
			else if(success && targetIndex > -1) HideInfo(true, currentInventory.m_items[index]);
			// if we added to the target inventory, but did not need a slot, simply update this
			else if(targetIndex == -2) UpdateInfo();
			// if it didn't work, reset
			else if(!success) { currentInventory.SetAtIndex(m_item, index); UpdateInfo(); return; }

			targetInventory.RaiseInventoryEvent(true);
			currentInventory.RaiseInventoryEvent(false);
		}
	}

	protected override void OnLeftSingleClick(GameObject p_clicked, Player p_clicker) {
		if(HeldItem == null && m_item.m_item) {
			ActivateGhost();
			HeldItem = this;
			Holder = p_clicker;
			ObscureInfo();
		} else if(HeldItem != null && HeldItem.gameObject == p_clicked) HideHeldItem();
		else if(HeldItem != null) Swap(HeldItem, true); // swap stacks / drop in a slot
	}

	protected override void OnRightDoubleClick(GameObject p_clicked, Player p_clicker) {
		OnRightSingleClick(p_clicked, p_clicker);
	}

	protected override void OnRightSingleClick(GameObject p_clicked, Player p_clicker) { 
		if(HeldItem != null && HeldItem.gameObject == p_clicked) HideHeldItem();
		else if(HeldItem != null) {
			if(m_item.m_amount > 0) // adding 1 to an existing slot
				Add(HeldItem, 1, false);
			else { // adding 1 to an empty slot
				m_item.m_item = HeldItem.m_item.m_item;
				m_item.m_outlineSprite = HeldItem.m_item.m_outlineSprite;
				m_item.m_durability = HeldItem.m_item.m_durability;
				m_item.m_amount = 1;
				HeldItem.m_item.m_amount -= 1;

				UpdateInfo();
			}

			if(!HeldItem.m_item.m_item || HeldItem.m_item.m_amount == 0) HideHeldItem();
			else { 
				HeldItem.UpdateInfo();
				HeldItem.ObscureInfo();
			}
		}
	}

	public void OnBeginDrag(PointerEventData p_eventData) {
		if(!m_item.m_item) return;
		if(HeldItem != null) HideHeldItem();

		ActivateGhost();
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
		HideGhost();

		// making sure that when destroying, you can only drop out of a window and not in the middle of the UI and destroy
		if(m_item.m_item && !m_validDrop && !p_eventData.hovered.Find(h => h.name.Contains("Canvas")) && m_item.m_inventory.m_itemDestroyModal)
			OpenDestructionModal();

		m_validDrop = false;
	}

	public void OnDrop(PointerEventData p_eventData) {
		CloseDestructionModal();
		m_validDrop = true;

		GameObject draggedItem = p_eventData.pointerDrag;
		if(!draggedItem) return;

		UIItem dragged = draggedItem.GetComponent<UIItem>();
		if(!dragged || !dragged.m_item.m_inventory || !dragged.m_item.m_item) return;

		Swap(dragged, true);
	}

	private void Swap(UIItem p_dragged, bool p_add) {
		if(p_add && Add(p_dragged, p_dragged.m_item.m_amount, true)) return;

		bool swapSuccess = p_dragged.m_item.m_inventory.Swap(p_dragged.m_item, m_item);

		if(swapSuccess) {
			if(p_dragged == HeldItem || this == HeldItem) HideHeldItem();

			SwapInfo(p_dragged);

			UpdateInfo();
			p_dragged.UpdateInfo();

			m_item.m_inventory.RaiseInventoryEvent(true);
			p_dragged.m_item.m_inventory.RaiseInventoryEvent(false);
		}
	}

	private bool Add(UIItem p_dragged, int p_amount, bool p_hideHeldItem) {
		// if stacking items together
		if(m_item.m_item && p_dragged.m_item.m_item.m_id == m_item.m_item.m_id && m_item.m_item.m_maxStackSize - p_amount > 0) {
			bool addSuccess = m_item.m_inventory.AddToItem(p_dragged.m_item, m_item, p_amount);

			if(addSuccess) {
				GetComponentInChildren<TextMeshProUGUI>().text = m_item.m_amount.ToString();

				if(p_dragged.m_item.m_amount == 0) {
					int index = p_dragged.m_item.m_inventoryIndex;

					p_dragged.m_item.m_inventory.Remove(p_dragged.m_item);
					p_dragged.m_item = p_dragged.m_item.m_inventory.m_items[index];

					p_dragged.HideInfo(false, null);
				}

				UpdateInfo();
				p_dragged.UpdateInfo();

				if(p_dragged == HeldItem || this == HeldItem) { 
					if(p_hideHeldItem) HideHeldItem();
					else HeldItem.UpdateInfo();
				}

				m_item.m_inventory.RaiseInventoryEvent(true);
				p_dragged.m_item.m_inventory.RaiseInventoryEvent(false);
			}

			return true;
		}

		return false;
	}

	private void SwapInfo(UIItem p_dragged) {
		// swap the images/amounts around
		Image image = GetComponent<Image>();
		Image draggedImage = p_dragged.GetComponent<Image>();
		Image otherGhost = p_dragged.m_ghost;
        TextMeshProUGUI amount = GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI otherAmount = draggedImage.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI otherGhostAmount = p_dragged.m_ghostAmount;

		image.sprite = draggedImage.sprite;
		draggedImage.sprite = m_ghost.sprite;
		otherGhost.sprite = draggedImage.sprite;
		m_ghost.sprite = image.sprite;

		amount.text = otherAmount.text;
		otherAmount.text = m_ghostAmount.text;
		m_ghostAmount.text = otherAmount.text;
		otherGhostAmount.text = otherAmount.text;

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

	private void CopyInfo(UIItem p_dragged) {
		Image image = GetComponent<Image>();
		Image draggedImage = p_dragged.GetComponent<Image>();
        TextMeshProUGUI amount = GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI otherAmount = draggedImage.GetComponentInChildren<TextMeshProUGUI>();

		image.sprite = draggedImage.sprite;
		m_ghost.sprite = image.sprite;
		
		amount.text = otherAmount.text;
		m_ghostAmount.text = otherAmount.text;

		image.color = draggedImage.color;
		amount.enabled = otherAmount.enabled;
	}

	public void UpdateInfo() {
		if(!m_item.m_item || m_item.m_amount == 0) {
			HideInfo(false, null);
			return;
		}

		Image image = GetComponent<Image>();
        TextMeshProUGUI amount = GetComponentInChildren<TextMeshProUGUI>();

		amount.enabled = true;

		if(m_item.m_amount == 1 && m_item.m_item.m_maxStackSize == 1) 
			amount.text = "";
		else amount.text = m_item.m_amount.ToString();

		image.sprite = m_item.m_item.m_sprite;
		m_ghost.sprite = m_item.m_item.m_sprite;

		image.color = new Color(1, 1, 1, 1);
		amount.color = new Color(1, 1, 1, 1);

		UpdateGhostAmountText();
	}

	private void HideInfo(bool p_swapItems, Item p_swapped) {
		Image image = GetComponent<Image>();
        TextMeshProUGUI amount = GetComponentInChildren<TextMeshProUGUI>();

		image.color = new Color(1, 1, 1, 0);
		amount.enabled = false;

		if(p_swapItems) m_item = p_swapped;
	}

	private void ObscureInfo() {
		Image image = GetComponent<Image>();
        TextMeshProUGUI amount = GetComponentInChildren<TextMeshProUGUI>();

		image.color = new Color(1, 1, 1, 0.5f);
		amount.color = new Color(1, 1, 1, 0.5f);
	}

	private void ActivateGhost() {
		UpdateGhostAmountText();

		m_ghost.transform.position = Input.mousePosition;
		m_ghost.gameObject.SetActive(true);
		m_ghost.transform.SetParent(GhostCanvas);
	}

	public void HideHeldItem() {
		HeldItem.HideGhost();
		HeldItem.UpdateInfo();
		HeldItem = null;
		Holder = null;
	}

	private void KillHeldItem() {
		Destroy(HeldItem.m_ghost);
		HeldItem = null;
		Holder = null;
	}

	private void UpdateGhostAmountText() {
		if(m_item.m_amount == 1 && m_item.m_item.m_maxStackSize == 1)
			m_ghostAmount.text = "";
		else m_ghostAmount.text = m_item.m_amount.ToString();
	}

	public void HideGhost() {
		m_ghost.transform.SetParent(gameObject.transform);
		m_ghost.gameObject.SetActive(false);
	}
}
