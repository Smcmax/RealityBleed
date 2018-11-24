using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour {

	[Tooltip("The damage dealt by this projectile")]
	[Range(0, 100)] public float m_damage;

	[Tooltip("Whether or not this projectile pierces opponents")]
	public bool m_piercing;

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
	private bool m_shot;

	void FixedUpdate() {
		if(!m_shot) return;
		if(Vector2.Distance(transform.position, m_start) >= m_range) {
			Disable();
			return;
		}

		foreach(ProjectileBehaviour behaviour in m_behaviours)
			behaviour.PreMove(this);
	}

	public void SetDamage(float p_damage) {
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
	}

	// it is assumed the current projectile is a generic projectile with an empty reference behaviour to fill up
	public void Clone(Projectile p_projectile) {
		SpriteRenderer otherRender = p_projectile.GetComponent<SpriteRenderer>();
		BoxCollider2D otherCollider = p_projectile.GetComponent<BoxCollider2D>();
		SpriteRenderer render = GetComponent<SpriteRenderer>();
		BoxCollider2D collider = GetComponent<BoxCollider2D>();

		render.sprite = otherRender.sprite;
		collider.size = otherCollider.size;
		collider.offset = otherCollider.offset;

		// this will overwrite any assignment done to this projectile beforehand
		p_projectile.Copy(typeof(Projectile), gameObject);

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

		if(collider.gameObject.name != gameObject.name) {
			CollisionRelay relay = collider.GetComponent<CollisionRelay>();

			if(relay != null) {
				m_shooter.Damage(this, relay.m_entity);

				hitEntity = true;
			}
		}

		if(m_piercing && hitEntity) Physics2D.IgnoreCollision(p_collision.otherCollider, collider);
		if(!m_piercing || !hitEntity) Disable();
	}

	private void Disable() {
		foreach(ProjectileBehaviour behaviour in m_behaviours)
			behaviour.Die(this);

		m_shot = false;
		m_start = Vector2.zero;
		m_target = Vector2.zero;
		m_shooter = null;

		ProjectilePooler.m_projectilePooler.Remove(gameObject);
	}
}
