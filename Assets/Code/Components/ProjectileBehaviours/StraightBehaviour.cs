using UnityEngine;

public class StraightBehaviour : ProjectileBehaviour {

	public override void Move(Projectile p_projectile, DataHolder p_data) {
		p_projectile.transform.position += (Vector3) (p_projectile.m_direction * p_projectile.m_speed * Time.deltaTime);
	}

	public override void Die(Projectile p_projectile) { }
}