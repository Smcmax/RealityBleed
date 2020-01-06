using UnityEngine;
using System;
using System.Linq;
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
        m_categoryVolumes[p_category] = p_volume;
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