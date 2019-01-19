using UnityEngine;

public class ProjectilePooler : ObjectPooler {

	[Tooltip("The movement job used to make straight-moving projectiles go straight, null if not using jobs")]
	public StraightProjectileMovementJob m_straightJob;

	[Tooltip("The movement job used to make straight-moving projectiles go straight, null if not using jobs")]
	public WavyProjectileMovementJob m_wavyJob;

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
		} else if(p_behaviour is WavyBehaviour) {
			if(m_wavyJob.m_projectiles.Contains(p_projectile)) return;

			m_wavyJob.AddProjectile(p_projectile, (WavyBehaviour) p_behaviour);
		}
	}

	public void Remove(GameObject p_obj, Projectile p_projectile, bool p_disable) {
		m_straightJob.RemoveProjectile(p_projectile); // it checks if it's in
		m_wavyJob.RemoveProjectile(p_projectile); // it checks if it's in

		if(p_disable) p_projectile.Disable(false);
		base.Remove(p_obj);
	}

	public override void Remove(GameObject p_obj) {
		Projectile proj = p_obj.GetComponent<Projectile>();

		m_straightJob.RemoveProjectile(proj);
		m_wavyJob.RemoveProjectile(proj);

		proj.Disable(false);
		base.Remove(p_obj);
	}
}
