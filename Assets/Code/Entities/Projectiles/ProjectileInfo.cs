using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Projectile Info")]
public class ProjectileInfo : ScriptableObject {

	[Tooltip("The damage dealt by this projectile")]
	[Range(0, 100)] public int m_damage;

	[Tooltip("The stat applied to the damage dealt")]
	public Stats m_statApplied;

	[Tooltip("The type of damage dealt by this projectile")]
	public DamageType m_damageType;

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

}
