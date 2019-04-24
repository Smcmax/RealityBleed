using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class CurrentSceneText : MonoBehaviour {

	[Tooltip("The text to set to the current scene title")]
	public Text m_text;

	void Awake() { 
		string scene = SceneManager.GetActiveScene().name;

		scene = Regex.Replace(scene, "([a-z])([A-Z])", "$1 $2"); // fix camelCasing

		m_text.text = scene;
	}
}
