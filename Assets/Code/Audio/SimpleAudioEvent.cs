using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Audio Events/Simple")]
public class SimpleAudioEvent : AudioEvent {

	[Tooltip("List of clips to play, will pick at random")]
	public AudioClip[] m_clips;

	[Tooltip("The volume range the audio clips can play at, will pick at random")]
	public RangedFloat m_volume;

	[Tooltip("The pitch range the audio clips can play at, will pick at random")]
	public RangedFloat m_pitch;

	public override void Play(AudioSource p_source) {
		if(m_clips.Length == 0) return;

		p_source.clip = m_clips[Random.Range(0, m_clips.Length)];
		p_source.volume = Random.Range(m_volume.Min, m_volume.Max);
		p_source.pitch = Random.Range(m_pitch.Min, m_pitch.Max);

		p_source.Play();
	}
}
