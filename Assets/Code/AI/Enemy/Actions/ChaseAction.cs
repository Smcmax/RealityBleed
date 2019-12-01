using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ChaseAction : Action {

	public override void Execute(StateController p_controller) {
		Chase(p_controller);
	}

	private void Chase(StateController p_controller) {
		if(!p_controller.m_target) return;

        if(Time.time >= p_controller.m_lastPathfindingUpdate + Constants.PATHFINDING_REFRESH_RATE) {
            p_controller.m_lastPathfindingUpdate = Time.time;
            p_controller.m_path = Pathfinding.FindPath(StateController.m_pathfindingGrid,
                                                       StateController.m_currentTilemap,
                                                       p_controller.m_entity, p_controller.m_target);
            p_controller.m_currentPathfindingCount = 0;
        }

        List<Point> points = p_controller.m_path;

        if(points.Count > 0) {
            Vector3 current = p_controller.m_entity.transform.position;
            Vector3 target = points[p_controller.m_currentPathfindingCount]
                             .ConvertToWorld(StateController.m_currentTilemap);

            bool arrived = Vector3.Distance(current, target) <= 0.1f;
            bool pathEnded = Vector3.Distance(current, points[points.Count - 1]
                                                       .ConvertToWorld(StateController.m_currentTilemap)) <= 0.5f;

            if(arrived && points.Count > p_controller.m_currentPathfindingCount + 1) {
                p_controller.m_currentPathfindingCount++;
                target = points[p_controller.m_currentPathfindingCount]
                         .ConvertToWorld(StateController.m_currentTilemap);
            } else if(pathEnded) {
                p_controller.m_entity.m_controller.Stop();
                return;
            }

            Vector2 direction = (target - current).normalized;
            bool canMoveTowards = CanMoveTowards(current, direction, p_controller);

            if(p_controller.m_tempTarget == Vector2.zero && !canMoveTowards && 
                Time.time >= p_controller.m_lastTempTarget + 1.5f) {
                for(float x = -1; x <= 1; x += 0.5f) {
                    if(p_controller.m_tempTarget != Vector2.zero) break;

                    for(float y = -1; y <= 1; y += 0.5f) {
                        Vector3 pos = new Vector2(current.x + x, current.y + y);

                        if(CanMoveTowards(current, (pos - current).normalized, p_controller) &&
                            CanMoveTowards(pos, (target - pos).normalized, p_controller)) {
                            p_controller.m_tempTarget = pos;
                            direction = (target - pos).normalized;
                            p_controller.m_lastTempTarget = Time.time;
                            break;
                        }
                    }
                }
            } else if(p_controller.m_tempTarget != Vector2.zero && canMoveTowards)
                p_controller.m_tempTarget = Vector2.zero;
            else if(p_controller.m_tempTarget != Vector2.zero)
                direction = (p_controller.m_tempTarget - (Vector2) current).normalized;

            p_controller.m_entity.m_controller.Move(direction * Time.deltaTime);
            Look.LookAt(p_controller.m_entity, (p_controller.m_target.transform.position - 
                                                p_controller.m_entity.transform.position).normalized);
        }
	}

    private bool CanMoveTowards(Vector2 p_current, Vector2 p_direction, StateController p_controller) {
        return !Physics2D.BoxCast(p_current, new Vector2(p_controller.m_entity.m_colliderSize.x + 0.1f,
                                                         p_controller.m_entity.m_colliderSize.y + 0.1f),
                                  0f, p_direction, 1.5f).collider;
    }
}