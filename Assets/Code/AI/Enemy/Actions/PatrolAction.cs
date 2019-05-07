using UnityEngine;

[CreateAssetMenu(menuName = "AI/Enemy/Actions/Patrol")]
public class PatrolAction : Action {

	public override void Execute(StateController p_controller) {
		Patrol(p_controller);
	}

	private void Patrol(StateController p_controller) {
		Vector2 waypoint = p_controller.m_waypoints[p_controller.m_nextWaypoint].position;
		Vector2 position = p_controller.m_entity.transform.position;
		Vector2 direction = (waypoint - position).normalized;

		p_controller.m_entity.m_controller.Move((direction * Time.deltaTime) / 2); // move slower while patrolling
		Look.LookAt(p_controller.m_entity, direction);

		if(Vector2.Distance(position, waypoint) <= 0.15) {
			int nextWaypoint = (p_controller.m_nextWaypoint + 1) % p_controller.m_waypoints.Length;
			p_controller.m_nextWaypoint = nextWaypoint;

			if(nextWaypoint == 0) p_controller.m_patrolFinished = true;
		}
	}
}