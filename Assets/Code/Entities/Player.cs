using UnityEngine;

public class Player : Entity {

	[HideInInspector] public PlayerController m_playerController;

	public override void Awake() { 
		base.Awake();

		m_playerController = GetComponent<PlayerController>();
	}

	void Update() {
		bool leftClick = true;
		bool fire = false;

		if(Input.GetButton("Fire1")) fire = true;
		else if(Input.GetButton("Fire2")) { fire = true; leftClick = false; }

		if(fire) {
			Weapon weapon = m_equipment.GetWeaponHandlingClick(leftClick);

			if(weapon == null) return;

			ShotPattern toFire = leftClick ? weapon.m_leftClickPattern : weapon.m_rightClickPattern;
			m_shooter.SetPatternInfo(toFire, "forcedTarget", (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition));

			weapon.Use(new string[]{ leftClick.ToString() }); // using weapon in case it has specific code to execute
		}
	}

}
