using UnityEngine;

[System.Serializable]
public class ColorReference {

	[Tooltip("Whether or not the reference should use a constant value")]
	public bool m_useConstant = true;

	[Tooltip("The constant value of this reference (if loading json, use enum number or m_variable instead)")]
	[ConditionalField("m_useConstant", "true")]
	public ConstantColors m_constantValue;

	[Tooltip("The variable value of this reference")]
	[ConditionalField("m_useConstant", "false")]
	public SerializableColor m_variable;

	public ColorReference() { }

	public ColorReference(ConstantColors p_value) {
		m_useConstant = true;
		m_constantValue = p_value;
	}

	// The reference's current value
	public Color Value {
		get { return m_useConstant ? m_constantValue.GetColor() : (Color) m_variable; }
		set { if(!m_useConstant) m_variable = value; }
	}
}
