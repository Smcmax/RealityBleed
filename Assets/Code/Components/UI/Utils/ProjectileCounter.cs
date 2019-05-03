using UnityEngine;
using TMPro;

public class ProjectileCounter : MonoBehaviour {

	[Tooltip("The text to update with the current projectile count")]
	public TextMeshProUGUI m_text;

	void Update() {
		m_text.text = "Projectiles: " + Game.m_projPool.m_activeObjects.Count;
	}
}