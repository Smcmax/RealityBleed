using UnityEngine;
using UnityEngine.EventSystems;
using Rewired;
using System.Collections.Generic;
using Rewired.Integration.UnityUI;

public class Player : Entity {

	public static List<Player> m_players = new List<Player>(); // TODO: replace with player manager

	[Tooltip("The id assigned to this player, corresponds to the Rewired player id")]
	public int m_playerId = 0;

	[HideInInspector] public PlayerController m_playerController;
	[HideInInspector] public Rewired.Player m_rewiredPlayer;
	[HideInInspector] public PlayerCursor m_mouse;

	private bool m_wasHoldingLeftClick = false;
	private bool m_wasHoldingRightClick = false;

	public override void Start() { 
		base.Start();

		// to be replaced
		m_players.Add(this);

		m_rewiredPlayer = ReInput.players.GetPlayer(m_playerId);
		m_mouse = GetComponent<PlayerCursor>();
		m_mouse.Init(m_playerId);
		m_playerController = GetComponent<PlayerController>();
		m_playerController.m_player = this;

		m_feedbackColor = Constants.TRANSPARENT;
	}

	void Update() {
		bool leftClick = true;
		bool fire = false;
		GameObject hover = Game.m_rewiredEventSystem.GetGameObjectUnderPointer(m_playerId);
		bool mouseOverGameObject = hover || UIItem.HeldItem; // change uiitem to support multiple holds...

		if(HideUIOnEvent.ObjectsHidden.Contains(hover)) mouseOverGameObject = UIItem.HeldItem;
		if(UIItem.HeldItem && this == UIItem.Holder) UIItem.HeldItem.MoveItem(m_mouse.GetPosition());

		//if(m_mouse.m_currentMode == CursorModes.CURSOR) return; // stop all actions if we're in a menu, this is also done in PlayerController
		if(MenuHandler.Instance.m_openedMenus.Count > 0) return;

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

			m_shooter.SetPatternInfo(toFire, "forcedTarget", (Vector2) Camera.main.ScreenToWorldPoint(m_mouse.GetPosition()));
			weapon.Use(this, new string[]{ weapon.m_leftClickPattern == toFire ? "true" : "false" }); // using weapon in case it has specific code to execute
		}

		for(int i = 1; i <= 6; i++) { 
			if(m_rewiredPlayer.GetButtonDown("Hotkey " + i)) { 
				AbilityWrapper wrapper = m_abilities.Find(a => a.HotkeySlot == i);

				if(wrapper != null) UseAbility(wrapper.Ability);
			}
		}
	}

	// to be replaced
	public static Player GetPlayerFromId(int p_playerId) { 
		foreach(Player player in m_players)
			if(player.m_playerId == p_playerId)
				return player;

		return null;
	}
}
