using UnityEngine;
using UnityEngine.UI;

public class DestroyItemModal : Modal {

	public override void AcceptModal() {
		UIItem toDelete = m_info as UIItem;
		Image image = toDelete.GetComponent<Image>();

		if(toDelete == UIItem.HeldItem) { 
			toDelete.HideHeldItem();
		}

		toDelete.m_item.m_inventory.Remove(toDelete.m_item);
		toDelete.m_item = new Item(toDelete.m_item.m_inventory, toDelete.m_item.m_inventoryIndex);
		image.color = new Color(255, 255, 255, 0);
		toDelete.GetComponentInChildren<Text>().enabled = false;

		if(m_eventToFireOnSuccess) m_eventToFireOnSuccess.Raise();
		CloseModal();
	}

	public override void DeclineModal() {
		CloseModal();
	}
}
