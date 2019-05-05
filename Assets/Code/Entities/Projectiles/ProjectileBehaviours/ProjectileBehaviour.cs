using UnityEngine;

public abstract class ProjectileBehaviour : ScriptableObject {

	public virtual void Init(Projectile p_projectile) { }
	public virtual void Init(Projectile p_projectile, DataHolder p_data) { }
	public abstract void Move(Projectile p_projectile, DataHolder p_data);
	public abstract void Die(Projectile p_projectile);
}
