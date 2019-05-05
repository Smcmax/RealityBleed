/*using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ReferencePattern))]
[CanEditMultipleObjects]
public class ReferencePatternEditor : Editor {

	SerializedProperty m_patternsProperty;

	void OnEnable() {
		m_patternsProperty = serializedObject.FindProperty("m_patterns");
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();

		m_patternsProperty.arraySize = EditorGUILayout.IntField("Size", m_patternsProperty.arraySize);

		for(int i = 0; i < m_patternsProperty.arraySize; ++i) {
			SerializedProperty pattern = m_patternsProperty.GetArrayElementAtIndex(i);

			EditorGUILayout.PropertyField(pattern);
		}

		serializedObject.ApplyModifiedProperties();
	}
}*/