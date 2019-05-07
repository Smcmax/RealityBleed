using UnityEngine;

public abstract class Action : ScriptableObject {
	public abstract void Execute(StateController p_controller);
	public virtual void OnTransition(StateController p_controller) { }
}
