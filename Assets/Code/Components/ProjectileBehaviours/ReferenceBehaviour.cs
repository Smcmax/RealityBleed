using UnityEngine;
using System.Collections.Generic;

public class ReferenceBehaviour : ProjectileBehaviour {

	[Tooltip("List of behaviours to call for")]
	public List<ProjectileBehaviour> m_behaviours;

	public override void Init(Projectile p_projectile) {
		foreach (ProjectileBehaviour behaviour in m_behaviours)
			behaviour.Init(p_projectile, m_data);
	}

	// the p_data is ignored here to use the reference behaviour's data instead
	public override void Move(Projectile p_projectile, DataHolder p_data) {
		foreach(ProjectileBehaviour behaviour in m_behaviours)
			behaviour.Move(p_projectile, m_data);
	}

	public override void Die(Projectile p_projectile) {
		foreach(ProjectileBehaviour behaviour in m_behaviours)
			behaviour.Die(p_projectile);

		m_behaviours.Clear();
	}
}