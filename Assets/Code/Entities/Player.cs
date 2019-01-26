using UnityEngine;
using UnityEngine.EventSystems;

public class Player : Entity {

	[HideInInspector] public PlayerController m_playerController;
	private bool m_wasHoldingLeftClick = false;
	private bool m_wasHoldingRightClick = false;

	public override void Awake() { 
		base.Awake();

		m_playerController = GetComponent<PlayerController>();
		m_feedbackColor = Constants.TRANSPARENT;
	}

	void Update() {
		bool leftClick = true;
		bool fire = false;
		GameObject hover = (EventSystem.current.currentInputModule as CustomStandaloneInputModule).GetGameObjectUnderPointer();
		bool mouseOverGameObject = hover || UIItem.HeldItem;

		if(HideUIOnEvent.ObjectsHidden.Contains(hover)) mouseOverGameObject = UIItem.HeldItem;

		if(Game.m_keybinds.GetButton("Primary Fire")) fire = true;
		else if(Game.m_keybinds.GetButton("Secondary Fire")) { fire = true; leftClick = false; }

		if(mouseOverGameObject) { // make sure we can't fire when we hover outside of ui elements when dragging items
			m_wasHoldingLeftClick = Game.m_keybinds.GetButton("Primary Fire");
			m_wasHoldingRightClick = Game.m_keybinds.GetButton("Secondary Fire");
		} else if(m_wasHoldingLeftClick || m_wasHoldingRightClick) fire = false;

		if(Game.m_keybinds.GetButtonUp("Primary Fire")) m_wasHoldingLeftClick = false;
		if(Game.m_keybinds.GetButtonUp("Secondary Fire")) m_wasHoldingRightClick = false;

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
