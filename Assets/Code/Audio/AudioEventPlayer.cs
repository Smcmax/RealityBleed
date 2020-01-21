using UnityEngine;

public class AudioEventPlayer : MonoBehaviour {

    [Tooltip("The audio event to play with this player")]
    public string m_audioEvent;

    [Tooltip("The category of audio this player plays")]
    public AudioCategories m_category;

    [Tooltip("Should this player play on start?")]
    public bool m_playOnStart;

    [HideInInspector] public AudioSource m_audioSource;

    public void Start() {
        m_audioSource = gameObject.AddComponent<AudioSource>();
        Game.m_audio.AddAudioSource(m_audioSource, m_category);

        if(m_playOnStart) Play();
    }

    public void Play() {
        AudioEvent.Play(m_audioEvent, m_category, m_audioSource);
    }
}
