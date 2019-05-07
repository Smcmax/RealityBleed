using UnityEngine;

[CreateAssetMenu(menuName = "AI/Enemy/Actions/Chase")]
public class ChaseAction : Action {

	public override void Execute(StateController p_controller) {
		Chase(p_controller);
	}

	private void Chase(StateController p_controller) {
		if(!p_controller.m_target) return;

		Vector2 direction = (p_controller.m_target.transform.position - p_controller.m_entity.transform.position).normalized;

		p_controller.m_entity.m_controller.Move(direction * Time.deltaTime);
		Look.LookAt(p_controller.m_entity, direction);
	}
}