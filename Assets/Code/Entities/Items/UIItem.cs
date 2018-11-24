using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {

	[Tooltip("The item represented in the UI")]
	public Item m_item;

	[Tooltip("The duplicate image used for dragging the item around instead of the original")]
	public Image m_ghost;

	private Canvas m_hoveredCanvas;
	private bool m_validDrop;

	void Awake() {
		m_ghost.raycastTarget = false;
		m_ghost.enabled = false;
		m_hoveredCanvas = GetComponentInParent<Canvas>();
	}

	public void OnBeginDrag(PointerEventData p_eventData) {
		if(!m_item.m_item) return;

		m_ghost.transform.position = transform.position;
		m_ghost.enabled = true;
		m_ghost.transform.SetParent(m_hoveredCanvas.transform);
	}

	public void OnDrag(PointerEventData p_eventData) {
		if(!m_item.m_item) return;

		m_ghost.transform.position += (Vector3) p_eventData.delta;

		foreach(GameObject hovered in p_eventData.hovered)
			if(hovered.name.Contains("Canvas")) {
				Canvas hover = hovered.GetComponent<Canvas>();
				
				if(hovered != m_hoveredCanvas) m_hoveredCanvas.sortingOrder = 0;

				hover.sortingOrder = 1;
				m_ghost.transform.SetParent(hover.transform);
				m_hoveredCanvas = hover;

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

				m_item.m_inventory.m_onInventoryActionEvent.Raise();
			}

			return;
		}

		bool swapSuccess = dragged.m_item.m_inventory.Swap(dragged.m_item, m_item);

		if(swapSuccess) SwapInfo(dragged);

		m_item.m_inventory.m_onInventoryActionEvent.Raise();
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

		// in case there was nothing in the slot
		draggedImage.color = image.color;
		otherAmount.enabled = amount.enabled;
		amount.enabled = true;
		image.color = new Color(255, 255, 255, 255);

		Item current = m_item;

		m_item = p_dragged.m_item;
		p_dragged.m_item = current;
	}
}
