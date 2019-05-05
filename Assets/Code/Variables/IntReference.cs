[System.Serializable]
public class IntReference {
	public bool m_useConstant = true; // Whether or not the reference should use a constant value

	[ConditionalField("m_useConstant", "true")] 
	public int m_constantValue;       // The constant value of this reference

	[ConditionalField("m_useConstant", "false")] 
	public IntVariable m_variable;    // The variable value of this reference

	public IntReference() { }

	public IntReference(int p_value) {
		m_useConstant = true;
		m_constantValue = p_value;
	}

	// The reference's current value
	public int Value {
		get { return m_useConstant ? m_constantValue : m_variable.Value; }
		set { if(!m_useConstant) m_variable.Value = value; }
	}

	// Allows comparison of a reference with an int
	public static implicit operator int(IntReference reference) {
		return reference.Value;
	}
}
