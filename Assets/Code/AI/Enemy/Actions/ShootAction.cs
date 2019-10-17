using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ShootAction : Action {

	[Tooltip("The pattern to start shooting in this action, will shoot as fast as possible")]
	public string m_patternToShoot;

	[Tooltip("Whether or not the shot pattern aims at the target")]
	public bool m_forceTarget;

	[Tooltip("Whether or not this shot pattern stops firing after transition")]
	public bool m_stopShootingOnTransition;

	public override void Execute(StateController p_controller) {
		if((m_forceTarget && !p_controller.m_target) || !p_controller.m_entity.m_shooter) return;

		Shoot(p_controller);
	}

	public override void OnTransition(StateController p_controller) {
		base.OnTransition(p_controller);

		if(!p_controller.m_entity.m_shooter) return;

		List<ShotPattern> patterns = new List<ShotPattern>();

		if(p_controller.m_shotPatterns.ContainsKey(this))
			p_controller.m_shotPatterns.TryGetValue(this, out patterns);

		if(patterns.Count > 0) {
			if(m_stopShootingOnTransition)
				foreach(ShotPattern pattern in patterns)
					p_controller.m_entity.m_shooter.StopShooting(pattern); 

			foreach(ShotPattern pattern in new List<ShotPattern>(patterns))
				if(!pattern.m_active) patterns.Remove(pattern);
		}
	}

	private void Shoot(StateController p_controller) {
		ShotPattern pattern = ShotPattern.Get(m_patternToShoot, false);
		List<ShotPattern> patterns = new List<ShotPattern>();
		
		if(p_controller.m_shotPatterns.ContainsKey(this))
			p_controller.m_shotPatterns.TryGetValue(this, out patterns);

		patterns.Add(pattern);
		p_controller.m_shotPatterns[this] = patterns;

		if(m_forceTarget)
			pattern.m_forcedTarget = p_controller.m_target.transform.position;

		p_controller.m_entity.m_shooter.Shoot(pattern);
	}
}