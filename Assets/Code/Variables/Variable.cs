using UnityEngine;

public class Variable<T> : ScriptableObject {
	[Multiline, SerializeField]
	private string m_developerDescription; // The developer's description

	[SerializeField]
	private T m_value; // The variable's internal value

	// The variable's public value
	public T Value {
		get { return m_value; }
		set { m_value = value; }
	}
}
