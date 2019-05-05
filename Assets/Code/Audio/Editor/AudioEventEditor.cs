using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioEvent), true)]
public class AudioEventEditor : Editor {
	[SerializeField] private AudioSource m_previewer;

	public void OnEnable() {
		m_previewer = EditorUtility.CreateGameObjectWithHideFlags("Audio Preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
	}

	public void OnDisable() {
		DestroyImmediate(m_previewer.gameObject);
	}

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

		if(GUILayout.Button("Preview")) ((AudioEvent)target).Play(m_previewer);

		EditorGUI.EndDisabledGroup();
	}
}