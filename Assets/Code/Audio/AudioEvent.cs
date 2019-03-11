using UnityEngine;

public abstract class AudioEvent : ScriptableObject {
	public abstract void Play(AudioSource p_source);
}
