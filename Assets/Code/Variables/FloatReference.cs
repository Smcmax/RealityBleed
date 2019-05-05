[System.Serializable]
public class FloatReference {
	public bool m_useConstant = true; // Whether or not the reference should use a constant value

	[ConditionalField("m_useConstant", "true")] 
	public float m_constantValue;     // The constant value of this reference

	[ConditionalField("m_useConstant", "false")] 
	public FloatVariable m_variable;  // The variable value of this reference

	public FloatReference() { }

	public FloatReference(float p_value) {
		m_useConstant = true;
		m_constantValue = p_value;
	}

	// The reference's current value
	public float Value {
		get { return m_useConstant ? m_constantValue : m_variable.Value; }
		set { if(!m_useConstant) m_variable.Value = value; }
	}

	// Allows comparison of a reference with a float
	public static implicit operator float(FloatReference reference) {
		return reference.Value;
	}
}
