using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour {

	public static List<Projectile> m_loadedProjectiles = new List<Projectile>();

	[Tooltip("Is this a projectile that can be used across the game using shot patterns with its name?")]
	public bool m_reference;

	[HideInInspector] public ProjectileInfo m_info;
	[HideInInspector] public BehaviourManager m_behaviourManager;
	[HideInInspector] public Shooter m_shooter;
	[HideInInspector] public Projectile m_original;
	[HideInInspector] public Vector2 m_start;
	[HideInInspector] public Vector2 m_target;
	[HideInInspector] public Vector2 m_direction;
	private bool m_shot;

	[HideInInspector] public SpriteRenderer m_render;
	[HideInInspector] public BoxCollider2D m_boxCollider;

	void Start() { 
		LoadComponents();

		if(m_reference) m_loadedProjectiles.Add(this);
	}

	public static Projectile Get(string p_name) {
		Projectile found = m_loadedProjectiles.Find(p => p.gameObject.name == p_name);

		return found;
	}

	private void LoadComponents() { 
		// preload to improve performance by doing less GetComponent calls
		if(!m_render) m_render = GetComponent<SpriteRenderer>();
		if(!m_boxCollider) m_boxCollider = GetComponent<BoxCollider2D>();
	}

	void FixedUpdate() {
		if(!m_shot || Time.timeScale == 0f) return;
		if(Vector2.Distance(transform.position, m_start) >= m_info.m_range) {
			Disable(true);
			return;
		}

		if(m_info.m_rotate) transform.Rotate(0, 0, m_info.m_rotationSpeed * Time.fixedDeltaTime);

		m_behaviourManager.Move(this);
	}

	public void Shoot(Shooter p_shooter, Vector2 p_target, Vector2 p_direction) {
		m_shot = true;
		m_start = transform.position;
		m_target = p_target;
		m_direction = p_direction;
		m_shooter = p_shooter;

		if(m_info.m_faceAtTarget)
			transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(m_direction.y, m_direction.x) * Mathf.Rad2Deg - 90, Vector3.forward);

		CollisionRelay relay = m_shooter.m_entity.m_collisionRelay;

		if(relay) Physics2D.IgnoreCollision(m_boxCollider, relay.GetComponent<BoxCollider2D>());

		m_render.enabled = true;
		m_boxCollider.enabled = true;
		gameObject.SetActive(true);

		m_behaviourManager.Init(this);
		Game.m_projPool.AddProjectileToJob(this);
	}

	// it is assumed the current projectile is a generic projectile with an empty reference behaviour to fill up
	public void Clone(Projectile p_projectile, ProjectileInfo p_projectileInfo, List<string> p_extraBehaviours) {
		m_reference = false;

		p_projectile.LoadComponents();
		LoadComponents();

		SpriteRenderer render = m_render;
		BoxCollider2D collider = m_boxCollider;

		render.sprite = p_projectile.m_render.sprite;
		collider.size = p_projectile.m_boxCollider.size;
		collider.offset = p_projectile.m_boxCollider.offset;

		// this will overwrite any assignment done to this projectile beforehand
		p_projectile.Copy(typeof(Projectile), gameObject);

		m_render = render;
		m_boxCollider = collider;
		m_info = p_projectileInfo;
		m_behaviourManager = GetComponent<BehaviourManager>();
		m_behaviourManager.m_behaviours = new Dictionary<ProjectileBehaviour, bool>();

		foreach(string behaviour in p_extraBehaviours) {
			ProjectileBehaviour clone = ProjectileBehaviour.Get(behaviour, false);

			if(clone != null) m_behaviourManager.m_behaviours.Add(clone, false);
		}

		m_original = p_projectile;
	}

	void OnCollisionEnter2D(Collision2D p_collision) {
		Collider2D collider = p_collision.collider;
		bool hit = false;

		if(collider.gameObject != gameObject) {
			CollisionRelay relay = collider.GetComponent<CollisionRelay>();

			if(relay != null && m_shooter) {
				m_shooter.Damage(this, relay.m_damageable);

				hit = true;
			}
		}

		if(m_info.m_piercing && hit && collider.transform.parent.tag == "NoProjCollision") Disable(true);
		if(m_info.m_piercing && hit) Physics2D.IgnoreCollision(p_collision.otherCollider, collider);
		if(collider.tag != "NoProjCollision" && (!m_info.m_piercing || !hit)) Disable(true);
	}

	public void Disable(bool p_removeFromProjPool) {
		m_behaviourManager.Die(this);

		m_shot = false;
		m_start = Vector2.zero;
		m_target = Vector2.zero;
		m_shooter = null;

		if(p_removeFromProjPool) Game.m_projPool.Remove(gameObject, this, false);
	}
}
