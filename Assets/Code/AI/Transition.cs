using System;
using UnityEngine;

[Serializable]
public class Transition {
	[Tooltip("The deciding factor in this transition, basically the meat of an if statement")]
	public Condition m_condition;

	[Tooltip("The state to transition to if the condition is true")]
	public State m_trueState;

	[Tooltip("The state to transition to if the condition is false")]
	public State m_falseState;
}
