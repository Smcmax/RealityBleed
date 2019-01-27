using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Inventory))]
public class Container : Interactable {

	[HideInInspector] public Inventory m_inventory;

	protected override void Awake() { 
		base.Awake();
		m_inventory = GetComponent<Inventory>();
	}

	public override void Interact(Entity p_entity) { 
		m_inventory.m_interactor = p_entity;
		MenuHandler.Instance.m_containerMenu.GetComponent<InventoryLoader>().m_inventory = m_inventory;

		MenuHandler.Instance.OpenMenu(MenuHandler.Instance.m_containerMenu);

		if(Game.m_options.Get("AutoLoot").GetBool()) StartCoroutine(AutoLoot(p_entity));
	}

	public IEnumerator AutoLoot(Entity p_entity) { 
		yield return new WaitForSeconds(Constants.AUTO_LOOT_DELAY);

		if(m_inventory.Count() > 0) {
			foreach(Item item in m_inventory.m_items) {
				if(item.m_item) {
					if(p_entity.m_inventory.IsFull()) break;

					m_inventory.Remove(item);
					p_entity.m_inventory.Add(item);
				}
			}

			if(!p_entity.m_inventory.IsFull()) Close();
		}
	}

	public override void OutOfRange(Entity p_entity) {
		Close();
	}

	private void Close() {
		if(MenuHandler.Instance.m_openedMenus.Contains(MenuHandler.Instance.m_containerMenu))
			MenuHandler.Instance.OpenMenu(MenuHandler.Instance.m_containerMenu);
	}
}
