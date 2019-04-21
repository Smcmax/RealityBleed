using UnityEngine;
using UnityEngine.SceneManagement;

public class PreloadLoadNextScene : MonoBehaviour {

	public static bool m_loaded = false;

	void Awake() {
		#if UNITY_EDITOR
		if(LoadingSceneIntegration.m_otherScene > 0) { 
			Debug.Log("Returning to scene: " + LoadingSceneIntegration.m_otherScene);
			SceneManager.LoadScene(LoadingSceneIntegration.m_otherScene);
            m_loaded = true;
			return;
		}
		#endif

		SceneManager.LoadScene(1);
	}
}
