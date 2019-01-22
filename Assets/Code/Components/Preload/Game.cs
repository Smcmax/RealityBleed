using UnityEngine;

static class Game {
	
	public static KeybindManager m_keybinds;
	public static OptionManager m_options;
	public static ProjectilePooler m_projPool;

	static Game() {
		GameObject game = SafeFind("_app");

		m_keybinds = (KeybindManager) SafeComponent(game, "KeybindManager");
		m_options = (OptionManager) SafeComponent(game, "OptionManager");
		m_projPool = (ProjectilePooler) SafeComponent(SafeFind("ProjectilePooler"), "ProjectilePooler");
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
