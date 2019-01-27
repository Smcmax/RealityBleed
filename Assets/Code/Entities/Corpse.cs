using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Corpse : MonoBehaviour {

	public void Init(Entity p_entity, float p_destroyTime) {
		BoxCollider2D interactBounds = gameObject.AddComponent<BoxCollider2D>();
		BoxCollider2D innerCollider = p_entity.m_collisionRelay.GetComponent<BoxCollider2D>();

		interactBounds.size = innerCollider.size += new Vector2(0.25f, 0.25f);
		interactBounds.offset = innerCollider.offset;
		interactBounds.isTrigger = true;

		Container container = gameObject.AddComponent<Container>();
		container.m_interactBounds = interactBounds;
		container.m_onInteractEvent = p_entity.m_interactCorpseEvent;
		innerCollider.isTrigger = false;

		container.m_inventory = p_entity.m_inventory;

		if(!p_entity.m_dropInventoryOnDeath) container.m_inventory.Clear();

		if(p_entity.m_dropInventoryOnDeath && p_entity.m_equipment) 
			for(int i = 0; i < p_entity.m_equipment.m_items.Length; i++)
				if(p_entity.m_equipment.m_items[i].m_item)
					container.m_inventory.Add(p_entity.m_equipment.m_items[i]);

		if(p_entity.m_lootTable) p_entity.m_lootTable.Drop(container.m_inventory);

		FadeCorpse(p_entity);

		if(p_destroyTime > 0) StartCoroutine(DestroyLater(p_destroyTime));
	}

	private void FadeCorpse(Entity p_entity) { 
		p_entity.GetComponent<SpriteRenderer>().color = Constants.CORPSE_COLOR;
	}

	IEnumerator DestroyLater(float p_destroyTime) { 
		yield return new WaitForSeconds(p_destroyTime);

		Destroy(gameObject);
	}
}
