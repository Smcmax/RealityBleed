using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneIntegration {

#if UNITY_EDITOR
    public static int m_otherScene = -2;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitLoadingScene() {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if(sceneIndex == 0) return;

        Debug.Log("Loading preload scene...");
		m_otherScene = sceneIndex;

        SceneManager.LoadScene(0); // loading preload scene, needs to be first in build order
    }
#endif
}