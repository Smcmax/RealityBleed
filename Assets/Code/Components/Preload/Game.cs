using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired.UI.ControlMapper;
using Rewired.Integration.UnityUI;
using System.Threading;

static class Game {
	
	public static AudioManager m_audio;
	public static OptionManager m_options;
	public static LanguageManager m_languages;
	public static ProjectilePooler m_projPool;
	public static NPCGenerator m_npcGenerator;
	public static RewiredStandaloneInputModule m_rewiredEventSystem;
	public static ControlMapper m_controlMapper;
	public static Menu m_controlMapperMenu;

	static Game() {
		GameObject game = SafeFind("_app");

        m_options = (OptionManager)SafeComponent(game, "OptionManager");
        m_languages = (LanguageManager) SafeComponent(game, "LanguageManager");
		m_audio = (AudioManager) SafeComponent(game, "AudioManager");
		m_projPool = (ProjectilePooler) SafeComponent(SafeFind("ProjectilePooler"), "ProjectilePooler");
		m_npcGenerator = (NPCGenerator) SafeComponent(game, "NPCGenerator");
		m_rewiredEventSystem = (RewiredStandaloneInputModule) SafeComponent(SafeFind("Rewired Event System"), "RewiredStandaloneInputModule");
        m_controlMapper = (ControlMapper) SafeComponent(SafeFind("ControlMapper"), "ControlMapper");
		m_controlMapperMenu = (Menu) SafeComponent(SafeFind("Canvas"), "Menu");

		m_controlMapperMenu.gameObject.SetActive(false);
	}

	private static GameObject SafeFind(string p_name) {
		GameObject obj = GameObject.Find(p_name);

		if(obj == null) Error("GameObject " + p_name + "  not in preload.");

		return obj;
	}

	private static Component SafeComponent(GameObject p_obj, string p_name) {
		Component comp = p_obj.GetComponent(p_name);

		if(comp == null) Error("Component " + p_name + " not in preload.");

		return comp;
	}

	private static void Error(string p_error) {
		Debug.Log(">>> Cannot proceed... " + p_error);
		Debug.Log(">>> Make sure you launch from the preload scene first!");
	}
}
