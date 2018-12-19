using UnityEngine;
using UnityEngine.EventSystems;

public class Player : Entity {

	[HideInInspector] public PlayerController m_playerController;
	private bool m_wasHoldingLeftClick = false;
	private bool m_wasHoldingRightClick = false;

	public override void Awake() { 
		base.Awake();

		m_playerController = GetComponent<PlayerController>();
	}

	void Update() {
		bool leftClick = true;
		bool fire = false;
		bool mouseOverGameObject = EventSystem.current.IsPointerOverGameObject();

		if(Input.GetButton("Fire1")) fire = true;
		else if(Input.GetButton("Fire2")) { fire = true; leftClick = false; }

		if(mouseOverGameObject) { // make sure we can't fire when we hover outside of ui elements when dragging items
			m_wasHoldingLeftClick = Input.GetButton("Fire1");
			m_wasHoldingRightClick = Input.GetButton("Fire2");
		} else if (m_wasHoldingLeftClick || m_wasHoldingRightClick) fire = false;

		if(Input.GetButtonUp("Fire1")) m_wasHoldingLeftClick = false;
		if(Input.GetButtonUp("Fire2")) m_wasHoldingRightClick = false;

		if(fire && !mouseOverGameObject) {
			Weapon weapon = m_equipment.GetWeaponHandlingClick(leftClick);
			ShotPattern toFire = m_equipment.GetShotPatternHandlingClick(leftClick);

			if(weapon == null) return;
			if(toFire == null) return;

			m_shooter.SetPatternInfo(toFire, "forcedTarget", (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition));
			weapon.Use(this, new string[]{ weapon.m_leftClickPattern == toFire ? "true" : "false" }); // using weapon in case it has specific code to execute
		}
	}

}
