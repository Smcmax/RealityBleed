using UnityEngine;
using System.Collections.Generic;
using static WavyProjectileMovementJob;
using static StraightProjectileMovementJob;

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

	[HideInInspector] public Shooter m_shooter;
	[HideInInspector] public Projectile m_original;
	[HideInInspector] public Vector2 m_start;
	[HideInInspector] public Vector2 m_target;
	[HideInInspector] public Vector2 m_direction;
	[HideInInspector] public StraightProjData m_straightProjData;
	[HideInInspector] public WavyProjData m_wavyProjData;
	private bool m_shot;

	private SpriteRenderer m_render;
	private PolygonCollider2D m_polyCollider;
	private BoxCollider2D m_boxCollider;

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

		foreach(ProjectileBehaviour behaviour in m_behaviours)
			behaviour.PreMove(this);
	}

	public void SetDamage(int p_damage) {
		m_damage = p_damage;
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

		foreach(ProjectileBehaviour behaviour in m_behaviours)
			behaviour.Init(this);

		Game.m_projPool.AddProjectileToJob(this);
	}

	// it is assumed the current projectile is a generic projectile with an empty reference behaviour to fill up
	public void Clone(Projectile p_projectile) {
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

		// take p_projectile's behaviours and clone them over to a generic projectile from the pooler
		ReferenceBehaviour reference = GetComponent<ReferenceBehaviour>();

		foreach (ProjectileBehaviour behaviour in p_projectile.m_behaviours)
			if(!(behaviour is ReferenceBehaviour)) reference.m_behaviours.Add(behaviour);

		// clear the list since the projectile copy keeps the same pointer for the list, which screws everything up
		m_behaviours = new List<ProjectileBehaviour>();
		m_behaviours.Add(reference);

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
		foreach(ProjectileBehaviour behaviour in m_behaviours)
			behaviour.Die(this);

		m_shot = false;
		m_start = Vector2.zero;
		m_target = Vector2.zero;
		m_shooter = null;

		if(p_removeFromProjPool) Game.m_projPool.Remove(gameObject, this, false);
	}
}
