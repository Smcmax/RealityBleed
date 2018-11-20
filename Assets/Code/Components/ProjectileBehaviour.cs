using UnityEngine;

public abstract class ProjectileBehaviour : MonoBehaviour {

	[HideInInspector] public DataHolder m_data;

	void Awake() {
		m_data = new DataHolder();
	}

	public void PreMove(Projectile p_projectile) {
		PreMove(p_projectile, m_data);
	}

	public void PreMove(Projectile p_projectile, DataHolder p_data) {
		if(p_projectile.m_rotate)
			p_projectile.transform.Rotate(0, 0, p_projectile.m_rotationSpeed * Time.fixedDeltaTime);

		Move(p_projectile);
	}

	public void Move(Projectile p_projectile) {
		Move(p_projectile, m_data);
	}

	public abstract void Move(Projectile p_projectile, DataHolder p_data);

	public abstract void Die(Projectile p_projectile);
}
