using UnityEngine;

public abstract class BaseItem : ScriptableObject {

	[Tooltip("The item's name")]
	public string m_name;

	[Tooltip("The item's sprite")]
	public Sprite m_sprite;

	[HideInInspector] public Entity m_holder;

	// this can be null
	public abstract void Use(string[] p_args);
}
