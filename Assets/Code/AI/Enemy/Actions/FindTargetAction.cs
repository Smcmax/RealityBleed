using UnityEngine;

[System.Serializable]
public class FindTargetAction : Action {

	public override void Execute(StateController p_controller) {
		Find(p_controller);
	}

	private void Find(StateController p_controller) {
		Transform transform = p_controller.m_entity.transform;
		EntityRuntimeSet enemies = p_controller.m_enemyEntities;
		Look look = p_controller.m_look;

		if(!enemies) return;

		foreach(Entity enemy in enemies.m_items) {
			Transform enemyTransform = enemy.transform;
			float distance = Vector2.Distance(transform.position, enemyTransform.position);

			if(distance <= p_controller.m_look.m_lookRange) {
				Vector2 direction = enemyTransform.position - transform.position;
				float angle = Vector2.Angle(direction, transform.right);

				if(look.m_fieldOfView / 2 >= angle || distance <= look.m_lookSphereRadius) {
					RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance + 1);

					if(hit.collider && hit.collider.gameObject == enemy.gameObject) {
						p_controller.m_target = enemy;
						return;
					}
				}
			}
		}
	}
}