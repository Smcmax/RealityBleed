using UnityEngine;

public abstract class Ability : ScriptableObject {

	public string m_name;

	public Sprite m_icon;

	// type

	public int m_manaCost;

	public int m_trainingExpCost;

	public int m_sellPrice;

	[Multiline] public string m_description;

	public abstract void Use();
}