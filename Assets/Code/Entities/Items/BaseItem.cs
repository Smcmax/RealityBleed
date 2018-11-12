using UnityEngine;

public abstract class BaseItem : ScriptableObject {

	[Tooltip("The item's name")]
	public string m_name;

	public abstract void Use();
}
