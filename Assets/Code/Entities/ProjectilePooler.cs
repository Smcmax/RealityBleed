using UnityEngine;
using System.Collections.Generic;

public class ProjectilePooler : ObjectPooler {

	[Tooltip("The full list of movement jobs, make sure there aren't two of a type")]
	public List<ProjectileMovementJob> m_movementJobs;

	public void AddProjectileToJob(Projectile p_projectile) { 
		foreach(KeyValuePair<ProjectileBehaviour, bool> behaviour in new Dictionary<ProjectileBehaviour, bool>(p_projectile.m_behaviourManager.m_behaviours))
			if(!behaviour.Value) AddProjectileToJob(p_projectile, behaviour.Key);
	}

	public void AddProjectileToJob(Projectile p_projectile, ProjectileBehaviour p_behaviour) {
		foreach(ProjectileMovementJob movementJob in m_movementJobs) {
			if(movementJob.CanAdd(p_behaviour)) {
				movementJob.AddProjectile(p_projectile, p_behaviour);
				p_projectile.m_behaviourManager.SetUsingJob(p_behaviour);

				break;
			}
		}
	}

	public void Remove(GameObject p_obj, Projectile p_projectile, bool p_disable) {
		foreach(ProjectileMovementJob movementJob in m_movementJobs)
			if(movementJob.m_projectiles.ContainsKey(p_projectile)) 
				movementJob.RemoveProjectile(p_projectile);

		if(p_disable) p_projectile.Disable(false);
		base.Remove(p_obj);
	}

	public override void Remove(GameObject p_obj) {
		Projectile proj = p_obj.GetComponent<Projectile>();

		foreach(ProjectileMovementJob movementJob in m_movementJobs)
			if(movementJob.m_projectiles.ContainsKey(proj))
				movementJob.RemoveProjectile(proj);

		proj.Disable(false);
		base.Remove(p_obj);
	}
}
