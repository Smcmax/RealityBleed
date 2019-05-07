using UnityEngine;

[CreateAssetMenu(menuName = "AI/Enemy/Conditions/PatrolEnded")]
public class PatrolEndedCondition : Condition {
	public override bool Test(StateController p_controller) {
		bool finished = p_controller.m_patrolFinished;

		return finished;
	}
}