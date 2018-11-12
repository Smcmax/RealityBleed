using UnityEngine;

public abstract class Ability {

	protected Class m_class;

	public Ability(Class p_class) {
		m_class = p_class;
	}

	public abstract void Use();
}
