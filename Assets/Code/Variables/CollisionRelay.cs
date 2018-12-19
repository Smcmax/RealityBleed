using UnityEngine;

public class CollisionRelay : MonoBehaviour {

	[Tooltip("The entity to relay the collision to")]
	public Entity m_entity;

	void Awake() { 
		m_entity.m_collisionRelay = this;
	}
}
