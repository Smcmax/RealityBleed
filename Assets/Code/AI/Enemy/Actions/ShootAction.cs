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

    [Tooltip("The range within which this action will fire, will stop shooting when out of range")]
    public RangedFloat m_range = new RangedFloat();

    [Tooltip("Call this pattern every loop instead of letting it loop?")]
    public bool m_callMultipleTimes;

	public override void Execute(StateController p_controller) {
		if((m_forceTarget && !p_controller.m_target) || !p_controller.m_entity.m_shooter) return;

		Shoot(p_controller);
	}

	public override void OnTransition(StateController p_controller) {
		base.OnTransition(p_controller);

		if(!p_controller.m_entity.m_shooter) return;

        StopShooting(p_controller, true);
	}

	private void Shoot(StateController p_controller) {
        float dist = Vector2.Distance(p_controller.m_entity.transform.position,
                                      p_controller.m_target.transform.position);

        if(m_range.Min + m_range.Max > 0 && (dist > m_range.Max || dist < m_range.Min)) {
            StopShooting(p_controller, false);
            return;
        }

        ShotPattern pattern = ShotPattern.Get(m_patternToShoot, false);
		List<ShotPattern> patterns = new List<ShotPattern>();

        if(p_controller.m_shotPatterns.ContainsKey(this)) {
            p_controller.m_shotPatterns.TryGetValue(this, out patterns);

            if(!m_callMultipleTimes) {
                if(m_forceTarget)
                    foreach(ShotPattern ongoingPattern in patterns) {
                        pattern.m_forcedTarget = p_controller.m_target.transform.position;
                    }

                return;
            }
        }

		patterns.Add(pattern);
		p_controller.m_shotPatterns[this] = patterns;

		if(m_forceTarget)
			pattern.m_forcedTarget = p_controller.m_target.transform.position;

		if(!p_controller.m_entity.m_shooter.Shoot(pattern)) StopShooting(p_controller, false);
    }

    private void StopShooting(StateController p_controller, bool p_transition) {
        List<ShotPattern> patterns = new List<ShotPattern>();

        if(p_controller.m_shotPatterns.ContainsKey(this))
            p_controller.m_shotPatterns.TryGetValue(this, out patterns);
        else return;

        if(patterns.Count > 0) {
            if(!p_transition || m_stopShootingOnTransition)
                foreach(ShotPattern pattern in patterns)
                    p_controller.m_entity.m_shooter.StopShooting(pattern);

            foreach(ShotPattern pattern in new List<ShotPattern>(patterns))
                if(!pattern.m_active) patterns.Remove(pattern);
        }

        p_controller.m_shotPatterns.Remove(this);
    }
}