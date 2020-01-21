using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	private List<AudioSourceWrapper> m_audioSources;
    private Dictionary<AudioCategories, float> m_categoryVolumes;

	public void Awake() {
		m_audioSources = new List<AudioSourceWrapper>();
        m_categoryVolumes = new Dictionary<AudioCategories, float>();
	}

	public void AddAudioSource(AudioSource p_source, AudioCategories p_category) { 
		m_audioSources.Add(new AudioSourceWrapper(p_source, p_category));
	}

	public void RemoveAudioSource(AudioSource p_source) { 
		if(m_audioSources.Any(s => s.Source == p_source))
			m_audioSources.RemoveAll(s => s.Source == p_source);
	}

	public List<AudioSource> GetAllAudioSourcesByCategory(AudioCategories p_category) { 
		List<AudioSource> sources = new List<AudioSource>();

		foreach(AudioSourceWrapper wrapper in m_audioSources.FindAll(s => s.Category == p_category))
			sources.Add(wrapper.Source);

		return sources;
	}

    public float GetCategoryVolume(AudioCategories p_category) {
        float value = 0;

        m_categoryVolumes.TryGetValue(p_category, out value);

        return value;
    }

    public void SetCategoryVolume(AudioCategories p_category, float p_volume) {
        float oldVolume = 0;

        if(m_categoryVolumes.Keys.Contains(p_category)) 
            oldVolume = GetCategoryVolume(p_category);

        m_categoryVolumes[p_category] = p_volume;

        if(oldVolume != p_volume)
            foreach(AudioSource existing in GetAllAudioSourcesByCategory(p_category))
                if(existing == null) RemoveAudioSource(existing);
                else if(existing.isPlaying) {
                    if(p_volume == 0) existing.volume = 0;
                    else if(oldVolume == 0) existing.volume = p_volume;
                    else existing.volume *= p_volume / oldVolume;
                }
    }

    public AudioSource PlayClipAtPoint(AudioClip p_clip, AudioCategories p_category, Vector3 p_location, 
                                       float p_volume, float p_pitch) {
        GameObject temp = new GameObject("TempAudio");
        AudioSource audio = temp.AddComponent<AudioSource>();

        temp.transform.position = p_location;
        audio.clip = p_clip;
        audio.volume = GetCategoryVolume(p_category) * p_volume;
        audio.pitch = p_pitch;

        AddAudioSource(audio, p_category);

        audio.Play();
        StartCoroutine(DestroySourceAfterTime(audio, p_clip.length));

        return audio;
    }

    private IEnumerator DestroySourceAfterTime(AudioSource p_source, float p_ttl) {
        yield return new WaitForSecondsRealtime(p_ttl);

        RemoveAudioSource(p_source);
        Destroy(p_source.gameObject);
    }
}

public enum AudioCategories { Music, SFX }

[Serializable]
public class AudioSourceWrapper { 
	public AudioSource Source;
	public AudioCategories Category;

	public AudioSourceWrapper(AudioSource p_source, AudioCategories p_category) { 
		Source = p_source;
		Category = p_category;
	}
}