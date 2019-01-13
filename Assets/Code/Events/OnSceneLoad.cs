using UnityEngine;
using UnityEngine.SceneManagement;

public class OnSceneLoad : MonoBehaviour {

	public GameEvent m_eventToFire;

	void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public void OnSceneLoaded(Scene p_scene, LoadSceneMode p_mode) { 
		m_eventToFire.Raise();
	}
}
