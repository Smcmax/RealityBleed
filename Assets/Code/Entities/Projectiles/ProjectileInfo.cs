using UnityEngine;

[System.Serializable]
public class ProjectileInfo {

	[Tooltip("The damage dealt by this projectile")]
	public int m_damage;

	[Tooltip("The stat applied to the damage dealt (STR, DEX, INT)")]
	public string m_statApplied;

	[Tooltip("The type of damage dealt by this projectile (Air, Earth, Electric, Fire, Holy, Ice, Normal, Poison, Void, Water)")]
	public string m_damageType;

	[Tooltip("Whether or not this projectile pierces opponents")]
	public bool m_piercing;

	[Tooltip("Whether or not this projectile passes through armor")]
	public bool m_armorPiercing;

	[Tooltip("The range the projectile will travel before being removed")]
	public float m_range;

	[Tooltip("The speed at which the projectile travels")]
	public float m_speed;

	[Tooltip("Whether or not the projectile rotates on itself")]
	public bool m_rotate;

	[Tooltip("Speed at which the projectile is rotating")]
	public float m_rotationSpeed;

	[Tooltip("If the projectile is faced towards the target")]
	public bool m_faceAtTarget;
}
