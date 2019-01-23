using UnityEngine;

public abstract class ProjectileBehaviour : MonoBehaviour {

	[HideInInspector] public DataHolder m_data;

	void Awake() {
		m_data = new DataHolder();
	}

	public virtual void Init(Projectile p_projectile) { }
	public virtual void Init(Projectile p_projectile, DataHolder p_data) { }

	public bool UsingJob() { 
		return m_data.Has("Job");
	}

	public void SetJob() { 
		m_data.Set("Job", true);
	}

	public void PreMove(Projectile p_projectile) {
		if(UsingJob()) return;

		PreMove(p_projectile, m_data);
	}

	public void PreMove(Projectile p_projectile, DataHolder p_data) {
		if(UsingJob()) return;

		Move(p_projectile);
	}

	public void Move(Projectile p_projectile) {
		Move(p_projectile, m_data);
	}

	public abstract void Move(Projectile p_projectile, DataHolder p_data);

	public abstract void Die(Projectile p_projectile);
}
