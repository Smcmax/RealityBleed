using UnityEngine;

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
	}

	public override void OutOfRange(Entity p_entity) {
		if(MenuHandler.Instance.m_openedMenus.Contains(MenuHandler.Instance.m_containerMenu))
			MenuHandler.Instance.OpenMenu(MenuHandler.Instance.m_containerMenu);
	}
}
