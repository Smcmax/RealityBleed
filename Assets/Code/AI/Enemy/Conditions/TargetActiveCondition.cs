using UnityEngine;

[System.Serializable]
public class TargetActiveCondition : Condition {

	[Tooltip("Whether or not this condition will let the entity drop its target whenever he's out of range")]
	public bool m_canDropCombat;

	public override bool Test(StateController p_controller) {
		bool inRange = true;
		bool active = p_controller.m_target ? p_controller.m_target.gameObject.activeSelf : false;

		if(active && m_canDropCombat)
			inRange = Vector2.Distance(p_controller.m_entity.transform.position, p_controller.m_target.transform.position)
					  <= p_controller.m_look.m_lookRange;

		if(!inRange && active && m_canDropCombat) p_controller.m_target = null;
		if(!active) p_controller.m_target = null;

		return m_canDropCombat ? p_controller.m_target : active;
	}
}