using UnityEngine;

public class ProjectilePooler : ObjectPooler {

	public static ProjectilePooler m_projectilePooler;

	void OnEnable() {
		m_projectilePooler = this;	
	}

	public override void Remove(GameObject p_obj) {
		base.Remove(p_obj);
	}
}
