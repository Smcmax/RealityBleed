using UnityEngine;

[System.Serializable]
public class SerializableColor {
	
	[Tooltip("The red, green, blue and alpha channels making up this color")]
	public float[] m_colors = new float[4]{ 1f, 1f, 1f, 1f };

	public Color Color { 
		get { 
			return new Color(m_colors[0], m_colors[1], m_colors[2], m_colors[3]);
		} set {
			m_colors = new float[4]{ value.r, value.g, value.b, value.a };
		}
	}

	public static implicit operator Color(SerializableColor p_instance) { 
		return p_instance.Color;
	}

	public static implicit operator SerializableColor(Color p_color) { 
		return new SerializableColor { Color = p_color };
	}
}