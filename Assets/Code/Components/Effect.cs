using UnityEngine;

public abstract class Effect : ScriptableObject {

	[Tooltip("Duration of the effect on the target entity in seconds")]
	[Range(0, 30)] public float m_duration;

	public abstract void Tick(Entity p_target);
}