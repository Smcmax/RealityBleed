using UnityEngine;
using System.Collections.Generic;

public class BehaviourManager : MonoBehaviour {

	[HideInInspector] public Dictionary<ProjectileBehaviour, bool> m_behaviours;

	public void SetUsingJob(ProjectileBehaviour p_behaviour) { 
		if(m_behaviours.ContainsKey(p_behaviour))
			m_behaviours[p_behaviour] = true;
	}

	public void Init(Projectile p_projectile) {
		foreach(ProjectileBehaviour behaviour in m_behaviours.Keys)
			behaviour.Init(p_projectile);
	}

	public void Move(Projectile p_projectile) {
		foreach(ProjectileBehaviour behaviour in m_behaviours.Keys) {
			if(!m_behaviours[behaviour]) behaviour.Move(p_projectile);
		}
	}

	public void Die(Projectile p_projectile) {
		foreach(ProjectileBehaviour behaviour in m_behaviours.Keys)
			behaviour.Die(p_projectile);

		m_behaviours.Clear();
	}
}