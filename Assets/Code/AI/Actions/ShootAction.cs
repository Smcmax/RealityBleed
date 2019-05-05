using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Shoot")]
public class ShootAction : Action {

	[Tooltip("The pattern to start shooting in this action, will shoot as fast as possible")]
	public ShotPattern m_patternToShoot;

	[Tooltip("Whether or not the shot pattern aims at the target")]
	public bool m_forceTarget;

	[Tooltip("Whether or not this shot pattern stops firing after transition")]
	public bool m_stopShootingOnTransition;

	public override void Execute(StateController p_controller) {
		if(m_forceTarget && !p_controller.m_target) return;

		Shoot(p_controller);
	}

	public override void OnTransition(StateController p_controller) {
		base.OnTransition(p_controller);

		if(m_stopShootingOnTransition) p_controller.m_entity.m_shooter.StopShooting(m_patternToShoot);
	}

	private void Shoot(StateController p_controller) {
		if(m_forceTarget)
			p_controller.m_entity.m_shooter.SetPatternInfo(m_patternToShoot, "forcedTarget", (Vector2) p_controller.m_target.transform.position);

		p_controller.m_entity.m_shooter.Shoot(m_patternToShoot);
	}
}