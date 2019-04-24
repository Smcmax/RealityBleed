using UnityEngine;
using UnityEngine.UI;

public class ProjectileCounter : MonoBehaviour {

	[Tooltip("The text to update with the current projectile count")]
	public Text m_text;

	void Update() {
		m_text.text = "Projectiles: " + Game.m_projPool.m_activeObjects.Count;
	}
}