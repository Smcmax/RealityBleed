using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Inventory))]
public class Container : Interactable {

	[Tooltip("Whether or not this container is auto lootable or if it needs to be manually looted/used")]
	public bool m_autoLootable;

	[HideInInspector] public Inventory m_inventory;
	[HideInInspector] public Corpse m_corpse;

	protected override void Awake() { 
		base.Awake();
		m_inventory = GetComponent<Inventory>();
	}

	public override void Interact(Entity p_entity) { 
		if(m_inventory.m_interactor && MenuHandler.Instance.m_containerMenu.gameObject.activeSelf) {
			if(m_inventory.m_interactor == p_entity) Close();

			return;
		}

		m_inventory.m_interactor = p_entity;
		if(MenuHandler.Instance.m_handlingPlayer == null) MenuHandler.Instance.m_handlingPlayer = (p_entity as Player).m_rewiredPlayer;
		MenuHandler.Instance.m_containerMenu.GetComponent<InventoryLoader>().m_inventory = m_inventory;
		MenuHandler.Instance.OpenMenu(MenuHandler.Instance.m_containerMenu);

		if(m_autoLootable && p_entity is Player)
			if(Game.m_options.Get("AutoLoot", ((Player) p_entity).m_playerId).GetBool())
                StartCoroutine(AutoLoot(p_entity));
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
		} else Close();
	}

	public override void OutOfRange(Entity p_entity) {
		Close();
	}

	private void Close() {
        m_inventory.m_interactor = null;
        MenuHandler.Instance.CloseMenu(MenuHandler.Instance.m_containerMenu);

        if(m_corpse && m_inventory.Count() == 0 && m_corpse.m_health.GetHealth() <= 0) Destroy(gameObject);
	}
}
