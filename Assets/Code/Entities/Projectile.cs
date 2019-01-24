using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour {

	[Tooltip("The damage dealt by this projectile")]
	[Range(0, 100)] public int m_damage;

	[Tooltip("The stat applied to the damage dealt")]
	public Stats m_statApplied;

	[Tooltip("Whether or not this projectile pierces opponents")]
	public bool m_piercing;

	[Tooltip("Whether or not this projectile passes through armor")]
	public bool m_armorPiercing;

	[Tooltip("The range the projectile will travel before being removed")]
	[Range(0, 50)] public float m_range;

	[Tooltip("The speed at which the projectile travels")]
	[Range(0, 10)] public float m_speed;

	[Tooltip("Whether or not the projectile rotates on itself")]
	public bool m_rotate;

	[Tooltip("Speed at which the projectile is rotating")]
	[ConditionalField("m_rotate")][Range(0, 1000)] public float m_rotationSpeed;

	[Tooltip("If the projectile is faced towards the target")]
	public bool m_faceAtTarget;

	[Tooltip("Rotation needed for the sprite to be oriented properly")]
	[Range(-360, 360)] public float m_spriteRotation;

	[Tooltip("The behaviours this projectile will use throughout its lifetime")]
	public List<ProjectileBehaviour> m_behaviours;

	[HideInInspector] public BehaviourManager m_behaviourManager;
	[HideInInspector] public Shooter m_shooter;
	[HideInInspector] public Projectile m_original;
	[HideInInspector] public Vector2 m_start;
	[HideInInspector] public Vector2 m_target;
	[HideInInspector] public Vector2 m_direction;
	private bool m_shot;

	[HideInInspector] public SpriteRenderer m_render;
	[HideInInspector] public PolygonCollider2D m_polyCollider;
	[HideInInspector] public BoxCollider2D m_boxCollider;

	void Start() { 
		LoadComponents();
	}

	private void LoadComponents() { 
		// preload to improve performance by doing less GetComponent calls
		if(!m_render) m_render = GetComponent<SpriteRenderer>();
		if(!m_polyCollider) m_polyCollider = GetComponent<PolygonCollider2D>();
		if(!m_boxCollider) m_boxCollider = GetComponent<BoxCollider2D>();
	}

	void FixedUpdate() {
		if(!m_shot || Time.timeScale == 0f) return;
		if(Vector2.Distance(transform.position, m_start) >= m_range) {
			Disable(true);
			return;
		}

		if(m_rotate) transform.Rotate(0, 0, m_rotationSpeed * Time.fixedDeltaTime);

		m_behaviourManager.Move(this);
	}

	public void Shoot(Shooter p_shooter, Vector2 p_target, Vector2 p_direction) {
		m_shot = true;
		m_start = transform.position;
		m_target = p_target;
		m_direction = p_direction;
		m_shooter = p_shooter;

		if(m_faceAtTarget)
			transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(m_direction.y, m_direction.x) * Mathf.Rad2Deg + m_spriteRotation, Vector3.forward);

		CollisionRelay relay = m_shooter.m_entity.m_collisionRelay;
		if(relay) Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), relay.GetComponent<BoxCollider2D>());

		gameObject.SetActive(true);

		foreach(ProjectileBehaviour behaviour in m_behaviourManager.m_behaviours.Keys)
			behaviour.Init(this);

		Game.m_projPool.AddProjectileToJob(this);
	}

	// it is assumed the current projectile is a generic projectile with an empty reference behaviour to fill up
	public void Clone(Projectile p_projectile, List<ProjectileBehaviour> p_extraBehaviours) {
		p_projectile.LoadComponents();
		LoadComponents();

		SpriteRenderer render = m_render;
		PolygonCollider2D polyCollider = m_polyCollider;
		BoxCollider2D collider = m_boxCollider;

		render.sprite = p_projectile.m_render.sprite;
		polyCollider.points = p_projectile.m_polyCollider.points;
		polyCollider.offset = p_projectile.m_polyCollider.offset;
		collider.size = p_projectile.m_boxCollider.size;
		collider.offset = p_projectile.m_boxCollider.offset;

		// this will overwrite any assignment done to this projectile beforehand
		p_projectile.Copy(typeof(Projectile), gameObject);

		m_render = render;
		m_polyCollider = polyCollider;
		m_boxCollider = collider;

		m_behaviourManager = GetComponent<BehaviourManager>();
		m_behaviourManager.m_behaviours = new Dictionary<ProjectileBehaviour, bool>();

		foreach(ProjectileBehaviour behaviour in p_projectile.m_behaviours)
			m_behaviourManager.m_behaviours.Add(behaviour, false);

		foreach(ProjectileBehaviour behaviour in p_extraBehaviours)
			m_behaviourManager.m_behaviours.Add(behaviour, false);

		m_original = p_projectile;
	}

	void OnCollisionEnter2D(Collision2D p_collision) {
		Collider2D collider = p_collision.collider;

		bool hitEntity = false;

		if(collider.gameObject != gameObject) {
			CollisionRelay relay = collider.GetComponent<CollisionRelay>();

			if(relay != null) {
				m_shooter.Damage(this, relay.m_entity, m_armorPiercing);

				hitEntity = true;
			}
		}

		if(m_piercing && hitEntity) Physics2D.IgnoreCollision(p_collision.otherCollider, collider);
		if(!m_piercing || !hitEntity) Disable(true);
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
