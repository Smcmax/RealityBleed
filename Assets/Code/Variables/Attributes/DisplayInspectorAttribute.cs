using UnityEngine;

public class DisplayInspectorAttribute : PropertyAttribute {

	public bool DisplayScript;

	public DisplayInspectorAttribute(bool displayScriptField = true) {
		DisplayScript = displayScriptField;
	}
}