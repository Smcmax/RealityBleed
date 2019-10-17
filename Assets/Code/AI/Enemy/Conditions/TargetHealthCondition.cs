using UnityEngine;

[System.Serializable]
public class TargetHealthCondition : Condition {

	[Tooltip("Percentage of target's health at which this condition becomes true")]
	public float m_healthPercentThreshold;

	public override bool Test(StateController p_controller) {
		UnitHealth health = p_controller.m_target.m_health;

		return health && ((float) health.GetHealth() / (float) health.GetMaxHealth()) * 100 <= m_healthPercentThreshold;
	}
}