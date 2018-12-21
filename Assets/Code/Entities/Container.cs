using UnityEngine;

public class Container : Interactable {

	[Tooltip("The menu handler handling this container's menu")]
	public MenuHandler m_menuHandler;

	[Tooltip("The menu to open when interacted with")]
	public GameObject m_menu;

	[HideInInspector] public Inventory m_inventory;

	protected override void Awake() { 
		base.Awake();
		m_inventory = GetComponent<Inventory>();
	}

	public override void Interact(Entity p_entity) { 
		m_inventory.m_interactor = p_entity;
		m_menu.GetComponent<InventoryLoader>().m_inventory = m_inventory;

		m_menuHandler.OpenMenu(m_menu);
	}
}
