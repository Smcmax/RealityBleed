using UnityEngine;

public class Player : Entity {

	void Update() {
		ShotPattern toFire = null;

		if(Input.GetButton("Fire1")) toFire = m_leftClickPattern;
		else if(Input.GetButton("Fire2")) toFire = m_rightClickPattern;

		if(toFire) {
			toFire.m_forcedTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			m_shooter.Shoot(toFire);
		}
	}

}
