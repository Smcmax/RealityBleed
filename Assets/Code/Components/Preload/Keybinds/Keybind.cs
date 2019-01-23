using UnityEngine;
using System;

[Serializable]
public class Keybind {
	public SimpleInput.AxisInput m_axis;
	public string m_profile;
	public bool m_useRawValues;
	public KeyCode m_negativeKey;
	public KeyCode m_positiveKey;
	public KeyCode m_altNegativeKey;
	public KeyCode m_altPositiveKey;

	public string m_positiveDisplayName;
	public string m_negativeDisplayName;

	[HideInInspector] public bool m_pressedThisFrame;
	[HideInInspector] public bool m_pressedLastFrame;
	[HideInInspector] public bool m_releasedThisFrame;
	[HideInInspector] public bool m_releasedLastFrame;
}
