using UnityEngine;
using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;
using System.IO;

[Serializable]
public class AudioEvent {

    private static List<AudioEvent> m_loadedAudioEvents = new List<AudioEvent>();

    [Tooltip("The audio effect's name")]
    public string m_name;

	[Tooltip("List of clips to play (INCLUDING FILE EXTENSION: .wav), will pick at random")]
	public string[] m_clips;

	[Tooltip("The volume range (0.0 to 1.0) the audio clips can play at, will pick at random")]
	public RangedFloat m_volume;

	[Tooltip("The pitch range the audio clips can play at, will pick at random")]
	public RangedFloat m_pitch;

    private AudioClip[] m_loadedClips; // internal loaded m_clips array

    public static void Play(string p_name, AudioSource p_source) {
        AudioEvent audio = Get(p_name);

        if(audio != null) audio.Play(p_source);
    }

    public static AudioEvent Get(string p_name) {
        AudioEvent audio = m_loadedAudioEvents.Find(ae => ae.m_name == p_name);
        if(audio != null) return audio;

        return Load(p_name);
    }

    private static AudioEvent Load(string p_name) {
        TextAsset loadedText = Resources.Load<TextAsset>("Audio/" + p_name);
        string loadedJson = "";
        string rootPath = Application.dataPath + "/Resources/Audio/";
        bool isInternal = true;

        if(loadedText != null) loadedJson = loadedText.text;
        else {
            string filePath = Application.dataPath + "/Data/Audio/" + p_name;

            if(File.Exists(filePath + ".json")) {
                StreamReader reader = new StreamReader(filePath + ".json");

                loadedJson = reader.ReadToEnd();
                rootPath = Application.dataPath + "/Data/Audio/";
                isInternal = false;
            }
        }

        if(loadedJson.Length > 0) {
            AudioEvent loaded = JsonUtility.FromJson<AudioEvent>(loadedJson);

            if(loaded != null && loaded.m_clips.Length > 0) {
                loaded.m_loadedClips = new AudioClip[loaded.m_clips.Length];

                for(int index = 0; index < loaded.m_clips.Length; index++) {
                    if(isInternal) {
                        AudioClip clip = Resources.Load<AudioClip>("Audio/" + loaded.m_clips[index].Replace(Path.GetExtension(loaded.m_clips[index]), ""));

                        if(clip) loaded.m_loadedClips[index] = clip;
                    } else {
                        string clipPath = rootPath + loaded.m_clips[index];

                        if(File.Exists(clipPath)) {
                            WWW www = new WWW("file://" + clipPath);

                            if(www.error != null) Debug.Log(www.error);
                            else loaded.m_loadedClips[index] = www.GetAudioClip();
                        }
                    }
                }
            }

            m_loadedAudioEvents.Add(loaded);

            return loaded;
        }

        return null;
    }

	public void Play(AudioSource p_source) {
		if(m_loadedClips.Length == 0) return;

		p_source.clip = m_loadedClips[Random.Range(0, m_clips.Length)];
		p_source.volume = Game.m_audio.GetCategoryVolume(AudioCategories.SFX) * Random.Range(m_volume.Min, m_volume.Max);
		p_source.pitch = Random.Range(m_pitch.Min, m_pitch.Max);

		p_source.Play();
	}
}
