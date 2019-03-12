using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rewired;
using System.Collections.Generic;

public class Player : Entity {

	public static List<Player> m_players; // TODO: replace with player manager

	[Tooltip("The id assigned to this player, corresponds to the Rewired player id")]
	public int m_playerId = 0;

	//[Tooltip("The image assigned to this player's pointer")]
	//public Image m_pointerImage;

	[HideInInspector] public PlayerController m_playerController;
	[HideInInspector] public Rewired.Player m_rewiredPlayer;

	private PlayerMouse m_mouse;
	private bool m_wasHoldingLeftClick = false;
	private bool m_wasHoldingRightClick = false;

	public override void Awake() { 
		base.Awake();

		// to be replaced
		m_players = new List<Player>();
		m_players.Add(this);

		m_rewiredPlayer = ReInput.players.GetPlayer(m_playerId);
		m_mouse = PlayerMouse.Factory.Create();
		m_mouse.playerId = m_playerId;
		m_mouse.xAxis.actionName = "AimX";
		m_mouse.yAxis.actionName = "AimY";
		m_playerController = GetComponent<PlayerController>();
		m_playerController.m_player = this;

		m_feedbackColor = Constants.TRANSPARENT;
	}

	void Update() {
		bool leftClick = true;
		bool fire = false;
		GameObject hover = (EventSystem.current.currentInputModule as CustomStandaloneInputModule).GetGameObjectUnderPointer();
		bool mouseOverGameObject = hover || UIItem.HeldItem;

		if(HideUIOnEvent.ObjectsHidden.Contains(hover)) mouseOverGameObject = UIItem.HeldItem;

		if(m_rewiredPlayer.GetButton("Primary Fire")) fire = true;
		else if(m_rewiredPlayer.GetButton("Secondary Fire")) { fire = true; leftClick = false; }

		if(mouseOverGameObject) { // make sure we can't fire when we hover outside of ui elements when dragging items
			m_wasHoldingLeftClick = m_rewiredPlayer.GetButton("Primary Fire");
			m_wasHoldingRightClick = m_rewiredPlayer.GetButton("Secondary Fire");
		} else if(m_wasHoldingLeftClick || m_wasHoldingRightClick) fire = false;

		if(m_rewiredPlayer.GetButtonUp("Primary Fire")) m_wasHoldingLeftClick = false;
		if(m_rewiredPlayer.GetButtonUp("Secondary Fire")) m_wasHoldingRightClick = false;

		if(fire && !mouseOverGameObject) {
			Weapon weapon = m_equipment.GetWeaponHandlingClick(leftClick);
			ShotPattern toFire = m_equipment.GetShotPatternHandlingClick(leftClick);

			if(weapon == null) return;
			if(toFire == null) return;

			m_shooter.SetPatternInfo(toFire, "forcedTarget", (Vector2) Camera.main.ScreenToWorldPoint(m_mouse.screenPosition));
			weapon.Use(this, new string[]{ weapon.m_leftClickPattern == toFire ? "true" : "false" }); // using weapon in case it has specific code to execute
		}

		for(int i = 1; i <= 6; i++) { 
			if(m_rewiredPlayer.GetButtonDown("Hotkey " + i)) { 
				AbilityWrapper wrapper = m_abilities.Find(a => a.HotkeySlot == i);

				if(wrapper != null) UseAbility(wrapper.Ability);
			}
		}
	}

	private float GetAxisRelative(string p_actionName, float p_absoluteToRelMult) { 
		float value = m_rewiredPlayer.GetAxis(p_actionName);

		if(m_rewiredPlayer.GetAxisCoordinateMode(p_actionName) == AxisCoordinateMode.Absolute)
			value *= Time.unscaledDeltaTime * p_absoluteToRelMult;

		return value;
	}

	// to be replaced
	public static Player GetPlayerFromId(int p_playerId) { 
		foreach(Player player in m_players)
			if(player.m_playerId == p_playerId)
				return player;

		return null;
	}
}
