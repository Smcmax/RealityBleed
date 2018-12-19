using UnityEngine;

[CreateAssetMenu(menuName = "AI/Conditions/ScanComplete")]
public class ScanCompleteCondition : Condition {
	public override bool Test(StateController p_controller) {
		return p_controller.CheckCountdown(p_controller.m_look.m_scanDuration);
	}
}