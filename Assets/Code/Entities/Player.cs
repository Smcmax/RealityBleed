using UnityEngine;
using Rewired;
using System.Collections.Generic;

public class Player : Entity {

	public static List<Player> m_players = new List<Player>(); // TODO: replace with player manager

	[Tooltip("The id assigned to this player, corresponds to the Rewired player id")]
	public int m_playerId = 0;

	[HideInInspector] public PlayerController m_playerController;
	[HideInInspector] public Rewired.Player m_rewiredPlayer;
	[HideInInspector] public PlayerCursor m_mouse;
	[HideInInspector] public bool m_interactingWithNPC = false;
	[HideInInspector] public List<string> m_completedQuests; // quest names
	[HideInInspector] public List<Quest> m_currentQuests; // actual player-specific quests with instantiated goals

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

		// TODO: TEMP
		int hotkey = 1;
		foreach(Ability ability in Ability.m_abilities) {
			AbilityWrapper wrapper = new AbilityWrapper();

			wrapper.AbilityName = ability.m_name;
			wrapper.Learned = true;
			wrapper.TrainingLevel = 1;
			wrapper.HotkeySlot = hotkey;

			m_abilities.Add(wrapper);

			hotkey++;
		}
	}

	void Update() {
		bool leftClick = true;
		bool fire = false;
		GameObject hover = Game.m_rewiredEventSystem.GetGameObjectUnderPointer(m_playerId);
		bool mouseOverGameObject = hover || UIItem.HeldItem;

		if(HideUIOnEvent.ObjectsHidden.Contains(hover)) mouseOverGameObject = UIItem.HeldItem;
		if(UIItem.HeldItem && this == UIItem.Holder) UIItem.HeldItem.MoveItem(m_mouse.GetPosition());
		if(MenuHandler.Instance.m_openedMenus.Count > 0) return;

		if(m_rewiredPlayer.GetButtonDown("SpawnNPC")) Game.m_npcGenerator.GenerateRandom(1);

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
			string toFire = m_equipment.GetShotPatternHandlingClick(leftClick);

			if(weapon == null) return;
			if(string.IsNullOrEmpty(toFire)) return;

			weapon.Use(this, new string[]{ weapon.m_leftClickPattern == toFire ? "true" : "false", "true" }); // using weapon in case it has specific code to execute
		}

		for(int i = 1; i <= 6; i++) { 
			if(m_rewiredPlayer.GetButtonDown("Hotkey " + i)) { 
				AbilityWrapper wrapper = m_abilities.Find(a => a.HotkeySlot == i);

				if(wrapper != null) UseAbility(wrapper.AbilityName);
			}
		}
	}

	public bool IsEligible(Quest p_quest) { 
		if(m_completedQuests.Contains(p_quest.m_name) || m_currentQuests.Exists(q => q.m_name == p_quest.m_name)) return false;

		if(p_quest.m_prerequisites != null && p_quest.m_prerequisites.Count > 0)
			foreach(string prerequisite in p_quest.m_prerequisites)
				if(!m_completedQuests.Contains(prerequisite)) return false;

		return true;
	}

	// to be replaced
	public static Player GetPlayerFromId(int p_playerId) { 
		foreach(Player player in m_players)
			if(player.m_playerId == p_playerId)
				return player;

		return null;
	}
}
