using UnityEngine;

[CreateAssetMenu(menuName = "AI/Conditions/CurrentHealth")]
public class CurrentHealthCondition : Condition {

	[Tooltip("Percentage of current health at which this condition becomes true")]
	public FloatReference m_healthPercentThreshold;

	public override bool Test(StateController p_controller) {
		UnitHealth health = p_controller.m_entity.m_health;

		return health && ((float) health.GetHealth() / (float) health.GetMaxHealth()) * 100 <= m_healthPercentThreshold;
	}
}