using UnityEngine;
using System.Collections.Generic;

public class EffectGiver : MonoBehaviour {

	public List<Effect> m_effectsGiven;

	void OnCollisionEnter2D(Collision2D p_collision) { 
		Collider2D collider = p_collision.collider;
		Entity entity = collider.GetComponent<Entity>();

		if(entity) entity.ApplyEffects(m_effectsGiven);
	}
}