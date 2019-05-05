using UnityEngine;

[System.Serializable]
public class ColorReference {
	public bool m_useConstant = true;      // Whether or not the reference should use a constant value

	[ConditionalField("m_useConstant", "true")]
	public ConstantColors m_constantValue; // The constant value of this reference

	[ConditionalField("m_useConstant", "false")]
	public Color m_variable;			   // The variable value of this reference

	public ColorReference() { }

	public ColorReference(ConstantColors p_value) {
		m_useConstant = true;
		m_constantValue = p_value;
	}

	// The reference's current value
	public Color Value {
		get { return m_useConstant ? m_constantValue.GetColor() : m_variable; }
		set { if(!m_useConstant) m_variable = value; }
	}
}
