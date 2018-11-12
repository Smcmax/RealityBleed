using UnityEngine;

public abstract class ProjectileBehaviour : MonoBehaviour {

	[HideInInspector] public BehaviourData m_data;

	void Awake() {
		m_data = new BehaviourData();
	}

	public void PreMove(Projectile p_projectile) {
		PreMove(p_projectile, m_data);
	}

	public void PreMove(Projectile p_projectile, BehaviourData p_data) {
		if(p_projectile.m_rotate)
			p_projectile.transform.Rotate(0, 0, p_projectile.m_rotationSpeed * Time.deltaTime);

		Move(p_projectile);
	}

	public void Move(Projectile p_projectile) {
		Move(p_projectile, m_data);
	}

	public abstract void Move(Projectile p_projectile, BehaviourData p_data);

	public abstract void Die(Projectile p_projectile);
}
