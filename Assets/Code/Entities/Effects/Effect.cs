using UnityEngine;

public abstract class Effect : ScriptableObject {

	[Tooltip("Duration of the effect on the target entity in seconds")]
	[Range(0, 30)] public float m_duration;

	[Tooltip("Chance for this effect to apply itself")]
	[Range(0, 100)] public float m_triggerChance;

	public bool TriggerCheck() {
		return Random.Range(0, 100) <= m_triggerChance;
	}

	public abstract void Tick(IEffectable p_target);
}