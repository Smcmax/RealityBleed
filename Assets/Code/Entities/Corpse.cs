using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Corpse : Destructible {

	private SpriteRenderer m_renderer;
	private CollisionRelay m_relay;
	private BoxCollider2D m_defaultCollider;
	private Container m_container;

	public override void Start() {}

	public void Init(Entity p_entity, float p_destroyTime) {
		m_relay = p_entity.m_collisionRelay;
        m_relay.m_damageable = this;
		m_type = p_entity.m_type;
		m_defense = p_entity.m_stats.GetBaseStatWithGear(Stats.DEF);
		m_feedbackTemplate = p_entity.m_feedbackTemplate;
		m_feedbackPositionRandomness = p_entity.m_feedbackPositionRandomness;

		m_health = gameObject.AddComponent<HealthInfo>();
        m_health.m_useLocalHealth = true;
        m_health.m_maxHealth = Constants.CORPSE_HEALTH_BASE + 
							   Mathf.RoundToInt(p_entity.m_health.GetMaxHealth() * Constants.CORPSE_HEALTH_PERCENTAGE);
        m_health.m_healthBarTemplate = p_entity.m_health.m_healthBarTemplate;
        m_health.m_healthBarOffset = p_entity.m_health.m_healthBarOffset;
		m_health.m_damageEvent = new UnityEvent();
		m_health.m_deathEvent = new UnityEvent();
        m_health.Init(this);

		m_defaultCollider = GetComponent<BoxCollider2D>();
		BoxCollider2D interactBounds = gameObject.AddComponent<BoxCollider2D>();
		BoxCollider2D innerCollider = m_relay.GetComponent<BoxCollider2D>();

		interactBounds.size = innerCollider.size += new Vector2(0.25f, 0.25f);
		interactBounds.offset = innerCollider.offset;
		interactBounds.isTrigger = true;

		m_container = gameObject.AddComponent<Container>();
        m_container.m_interactBounds = interactBounds;
        m_container.m_onInteractEvent = p_entity.m_interactCorpseEvent;
        m_container.m_autoLootable = true;
		m_container.m_corpse = this;
		innerCollider.isTrigger = false;

        m_container.m_inventory = p_entity.m_inventory;

		if(!p_entity.m_dropInventoryOnDeath) m_container.m_inventory.Clear();

		if(p_entity.m_dropInventoryOnDeath && p_entity.m_equipment) 
			for(int i = 0; i < p_entity.m_equipment.m_items.Length; i++)
				if(p_entity.m_equipment.m_items[i].m_item)
                    m_container.m_inventory.Add(p_entity.m_equipment.m_items[i]);

		if(p_entity.m_lootTable != null && p_entity.m_lootTable.m_items.Count > 0) p_entity.m_lootTable.Drop(m_container.m_inventory);

		m_renderer = p_entity.GetComponent<SpriteRenderer>();
		FadeCorpse(p_entity);

		if(p_destroyTime > 0) StartCoroutine(DestroyLater(p_destroyTime));
	}

	public override void OnDeath() {
		Destroy(m_relay.gameObject);
		Destroy(m_defaultCollider);
		Destroy(m_health);

		if(m_container.m_inventory.Count() > 0) DestroyCorpse();
		else Destroy(gameObject);
	}

	private void FadeCorpse(Entity p_entity) { 
		m_renderer.color = Constants.CORPSE_COLOR;
	}

	private void DestroyCorpse() {
		m_renderer.color = Constants.DESTROYED_CORPSE_COLOR;
	}

	IEnumerator DestroyLater(float p_destroyTime) { 
		yield return new WaitForSeconds(p_destroyTime);

		if(gameObject) Destroy(gameObject);
	}
}
