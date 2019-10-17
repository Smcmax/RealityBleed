using UnityEngine;

public static class ComponentExtension {

	public static Component Copy(this Component p_original, System.Type p_type, GameObject p_destination) {
		Component copy = p_destination.GetComponent(p_type);

		if(!copy) copy = p_destination.AddComponent(p_type);

		// Copied fields can be restricted with BindingFlags
		System.Reflection.FieldInfo[] fields = p_type.GetFields();

		foreach(System.Reflection.FieldInfo field in fields)
			field.SetValue(copy, field.GetValue(p_original));

		return copy;
	}
}