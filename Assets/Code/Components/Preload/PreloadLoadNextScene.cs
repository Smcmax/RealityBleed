using UnityEngine;
using UnityEngine.SceneManagement;

public class PreloadLoadNextScene : MonoBehaviour {

	void Awake() {
		#if UNITY_EDITOR
		if(LoadingSceneIntegration.m_otherScene > 0) { 
			Debug.Log("Returning to scene: " + LoadingSceneIntegration.m_otherScene);
			SceneManager.LoadScene(LoadingSceneIntegration.m_otherScene);
			return;
		}
		#endif

		SceneManager.LoadScene(1);
	}
}
