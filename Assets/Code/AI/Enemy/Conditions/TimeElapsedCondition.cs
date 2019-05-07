using UnityEngine;

[CreateAssetMenu(menuName = "AI/Enemy/Conditions/TimeElapsed")]
public class TimeElapsedCondition : Condition {

	[Tooltip("Time (in seconds) to wait until this condition turns true")]
	public FloatReference m_time;

	public override bool Test(StateController p_controller) {
		return p_controller.CheckCountdown(m_time);
	}
}