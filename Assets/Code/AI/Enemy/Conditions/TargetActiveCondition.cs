using UnityEngine;

[System.Serializable]
public class TargetActiveCondition : Condition {

	[Tooltip("Whether or not this condition will let the entity drop its target whenever he's out of range")]
	public bool m_canDropFromRange;

    [Tooltip("Whether or not this condition will let the entity drop its target whenever he's out of sight")]
    public bool m_canDropFromLOS;

	public override bool Test(StateController p_controller) {
		bool inRange = true;
        bool inSight = true;
		bool active = p_controller.m_target ? p_controller.m_target.gameObject.activeSelf : false;

		if(active && m_canDropFromRange)
			inRange = Vector2.Distance(p_controller.m_entity.transform.position, 
                                       p_controller.m_target.transform.position)
					  <= p_controller.m_look.m_lookRange;

        if(active && m_canDropFromLOS) {
            Vector2 dir = (p_controller.m_target.transform.position - p_controller.m_entity.transform.position)
                          .normalized;
            RaycastHit2D[] hits = Physics2D.LinecastAll(p_controller.m_entity.transform.position,
                                                        p_controller.m_target.transform.position);

            foreach(RaycastHit2D hit in hits) {
                Collider2D collider = hit.collider;

                if(collider.gameObject.layer == LayerMask.NameToLayer("Walls") ||
                   (collider.gameObject.layer == LayerMask.NameToLayer("Entity") && 
                   collider.gameObject != p_controller.m_entity.gameObject &&
                   collider.gameObject != p_controller.m_target.gameObject)) {
                    inSight = false;
                    break;
                }
            }
        }

        if((!m_canDropFromRange || inRange) && (!m_canDropFromLOS || inSight))
            p_controller.m_combatDropAttemptTime = 0;

        float timeSinceDropAttempt = p_controller.m_combatDropAttemptTime == 0 ?
                                     0 : Time.time - p_controller.m_combatDropAttemptTime;

        if(active && ((!inRange && m_canDropFromRange)) || (!inSight && m_canDropFromLOS)) {
            if(timeSinceDropAttempt == 0) p_controller.m_combatDropAttemptTime = Time.time;
            else if(timeSinceDropAttempt >= p_controller.m_look.m_combatDropTime)
                p_controller.m_target = null;
        }

        if(!active) {
            p_controller.m_target = null;
            p_controller.m_combatDropAttemptTime = 0;
        }

		return p_controller.m_target;
	}
}