using UnityEngine;

public class CollisionRelay : MonoBehaviour {

	[HideInInspector] public IDamageable m_damageable;

	void Awake() { 
		m_damageable = GetComponentInParent<IDamageable>();

		if(m_damageable is Entity)
			((Entity) m_damageable).m_collisionRelay = this;
	}
}
