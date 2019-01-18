using UnityEngine;

public class ProjectilePooler : ObjectPooler {

	[Tooltip("The movement job used to make straight-moving projectiles go straight, null if not using jobs")]
	public StraightProjectileMovementJob m_straightJob;

	public void AddProjectileToJob(Projectile p_projectile) { 
		foreach(ProjectileBehaviour behaviour in p_projectile.m_behaviours)
			if(behaviour is ReferenceBehaviour)
				foreach(ProjectileBehaviour refBehaviour in ((ReferenceBehaviour) behaviour).m_behaviours) {
					behaviour.SetJob();
					AddProjectileToJob(p_projectile, refBehaviour);
				}
			else AddProjectileToJob(p_projectile, behaviour);
	}

	public void AddProjectileToJob(Projectile p_projectile, ProjectileBehaviour p_behaviour) {
		if(p_behaviour is StraightBehaviour) {
			if(m_straightJob.m_projectiles.Contains(p_projectile)) return;

			m_straightJob.AddProjectile(p_projectile);
		}
	}

	public void Remove(GameObject p_obj, Projectile p_projectile) {
		m_straightJob.RemoveProjectile(p_projectile); // it checks if it's in
		Remove(p_obj);
	}

	public override void Remove(GameObject p_obj) {
		m_straightJob.RemoveProjectile(p_obj.GetComponent<Projectile>());
		base.Remove(p_obj);
	}
}
