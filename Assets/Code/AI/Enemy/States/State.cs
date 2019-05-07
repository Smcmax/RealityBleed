using UnityEngine;

[CreateAssetMenu(menuName = "AI/Enemy/State")]
public class State : ScriptableObject {

	[Tooltip("Contains a state's actions, runs every update tick")]
	public Action[] m_actions;

	[Tooltip("Transitions are checked every update tick, if the conditions are met, the state will change")]
	public Transition[] m_transitions;

	public Color m_sceneGizmoColor;

	public void UpdateState(StateController p_controller) {
		ExecuteActions(p_controller);
		CheckTransitions(p_controller);
	}

	private void ExecuteActions(StateController p_controller) {
		foreach(Action action in m_actions)
			action.Execute(p_controller);
	}

	private void CheckTransitions(StateController p_controller) { 
		foreach(Transition transition in m_transitions)
			if(transition.m_condition.Test(p_controller)) {
				if(p_controller.TransitionToState(transition.m_trueState)) return;
			} else {
				if(p_controller.TransitionToState(transition.m_falseState)) return;
			}
	}

}
