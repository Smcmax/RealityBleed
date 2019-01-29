using UnityEngine;

[CreateAssetMenu(menuName = "Projectile Behaviours/Wavy")]
public class WavyBehaviour : ProjectileBehaviour {

	[Tooltip("Size of the wave")]
	[Range(0, 0.1f)] public float m_magnitude;

	[Tooltip("Speed of the wave")]
	[Range(0, 25)] public int m_frequency;

	public override void Move(Projectile p_projectile, DataHolder p_data) { }

	public override void Die(Projectile p_projectile) { }
}