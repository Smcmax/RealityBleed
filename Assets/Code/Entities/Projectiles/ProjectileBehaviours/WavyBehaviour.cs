using UnityEngine;

[System.Serializable]
public class WavyBehaviour : ProjectileBehaviour {

	[Tooltip("Size of the wave")]
	public float m_magnitude;

	[Tooltip("Speed of the wave")]
	public int m_frequency;

	public override void Move(Projectile p_projectile) { }

	public override void Die(Projectile p_projectile) { }

	public override ProjectileBehaviour Clone() {
		WavyBehaviour behaviour = (WavyBehaviour) base.Clone();

		behaviour.m_magnitude = m_magnitude;
		behaviour.m_frequency = m_frequency;

		return behaviour;
	}
}