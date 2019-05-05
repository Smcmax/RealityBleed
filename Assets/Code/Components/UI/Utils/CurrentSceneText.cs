using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class CurrentSceneText : MonoBehaviour {

	[Tooltip("The text to set to the current scene title")]
	public TextMeshProUGUI m_text;

	void Start() { 
		string scene = SceneManager.GetActiveScene().name;

		scene = Regex.Replace(scene, "([a-z])([A-Z])", "$1 $2"); // fix camelCasing

		m_text.text = scene;
	}
}
