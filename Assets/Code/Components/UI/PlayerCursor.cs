using Rewired.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCursor : MonoBehaviour {

	[Tooltip("Initialize on start with the following player ID")]
	public int m_playerId = -1;

	[Tooltip("How far should the cursor be from the camera?")]
	public float m_distanceFromCamera;

	[Tooltip("The cursor's speed")]
	public float m_cursorSpeed = 1f;

	[Tooltip("The sprite scale that should be used for the cursor's sprite")]
	public float m_spriteScale = 0.05f;

	[Tooltip("The prefab used for the player mouse")]
	public GameObject m_mousePrefab;

	[Tooltip("The parent to put the player mouse under")]
	public GameObject m_mouseParent;

	[Tooltip("The prefab used for the ui representation of the player mouse")]
	public GameObject m_uiCursorPrefab;

	[Tooltip("The canvas to put the ui representation of the player mouse under")]
	public Canvas m_uiCanvas;

	[Tooltip("Should the player mouse use UI coordinates instead of world coordinates?")]
	public bool m_useUICoordinatesOnly;

	[Tooltip("Should the hardware pointer be hidden? (the normal cursor)")]
	public bool m_hideHardwarePointer = true;

	[HideInInspector] public CursorModes m_currentMode;
	private Rewired.Player m_rewiredPlayer;
	private Player m_player;
	private bool m_usingController;
	private bool m_updateCalled;
	private Vector2 m_direction;

	private GameObject m_cursor;
	private UICursor m_uiCursor;
	private PlayerMouse m_mouse;

	void Start() {
		m_updateCalled = false;

		if(m_playerId >= 0)
			Init(m_playerId);
	}

	public void Init(int p_playerId) {
		if(m_mouseParent) m_cursor = Instantiate(m_mousePrefab, m_mouseParent.transform);
		else m_cursor = Instantiate(m_mousePrefab);
		
		m_uiCursor = Instantiate(m_uiCursorPrefab, m_uiCanvas.transform).GetComponent<UICursor>();
		m_uiCursor.transform.localScale = new Vector3(m_spriteScale, m_spriteScale, m_spriteScale);

		m_mouse = m_cursor.GetComponent<PlayerMouse>();
		m_mouse.useHardwarePointerPosition = false;
		m_mouse.playerId = p_playerId;
		m_rewiredPlayer = Rewired.ReInput.players.GetPlayer(p_playerId);

		if(Player.m_players.Count > 0) m_player = Player.GetPlayerFromId(p_playerId);

		m_mouse.leftButton.actionName = "UIInteract1";
		m_mouse.rightButton.actionName = "UIInteract2";
		m_mouse.middleButton.actionName = "UIInteract3";
		m_mouse.wheel.yAxis.actionName = "UIWheelY";
		m_mouse.pointerSpeed = m_cursorSpeed;

		List<PlayerMouse> mice = Game.m_rewiredEventSystem.PlayerMice;
		mice.Add(m_mouse);
		Game.m_rewiredEventSystem.PlayerMice = mice;

		ChangeMode(m_useUICoordinatesOnly ? CursorModes.CURSOR : CursorModes.LINE, true);

		m_mouse.ScreenPositionChangedEvent += OnScreenPositionChanged;
		m_mouse.screenPosition = new Vector2(Screen.width / 2, Screen.height / 2 + 1);
	}

	public void ChangeMode(CursorModes p_mode, bool p_force) { 
		if(!p_force && p_mode == m_currentMode) return;

		m_uiCursor.transform.rotation = Quaternion.Euler(0, 0, 0);
		
		string axisUsed = "Aim";

		if(p_mode == CursorModes.CURSOR)
			axisUsed = "UI" + axisUsed;

		m_mouse.xAxis.actionName = axisUsed + "X";
		m_mouse.yAxis.actionName = axisUsed + "Y";

		m_currentMode = p_mode;
		
		// ui cursor doesn't know the real cursor mode, only player cursor does
		if(p_mode == CursorModes.LINE && !p_force) {
			if(m_usingController) {
				m_uiCursor.ChangeModes(p_mode);
				m_mouse.screenPosition = new Vector2(Screen.width / 2, Screen.height / 2 + 1);
			}
		} else m_uiCursor.ChangeModes(p_mode);

		SetCursorState(p_mode);
	}

	public Vector2 GetPosition() { 
		return m_mouse.screenPosition;
	}

	private void OnScreenPositionChanged(Vector2 p_position) {
		Vector2 pos = p_position;
		Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);

		if(m_currentMode == CursorModes.LINE && m_usingController && Vector2.Distance(pos, center + m_direction * 32) > 0.25) {
			SetRotation(pos);
			return;
		} else if(m_currentMode == CursorModes.LINE && m_usingController) return;
		else m_uiCursor.transform.rotation = new Quaternion();

		if(!m_useUICoordinatesOnly)
			m_cursor.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, m_distanceFromCamera));
		else m_cursor.transform.position = pos;

		m_uiCursor.transform.position = pos;
		EventSystem.current.SetSelectedGameObject(null);
	}

	private void SetRotation(Vector2 p_position) {
		Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);

		float angle = Quaternion.FromToRotation(Vector2.up, p_position - center).eulerAngles.z;
		m_direction = (p_position - center).normalized;

		m_uiCursor.transform.position = center + m_direction * (Screen.width / 5);
		m_uiCursor.transform.rotation = Quaternion.Euler(0, 0, angle);

		m_mouse.screenPosition = center + m_direction * 32;
	}

	private void SetCursorState(CursorModes p_mode) {
		Cursor.visible = !m_hideHardwarePointer;

		if(p_mode == CursorModes.LINE && m_usingController)
			Cursor.lockState = CursorLockMode.Locked;
		else {
			Cursor.lockState = CursorLockMode.Confined;

			#if UNITY_EDITOR
				Cursor.lockState = CursorLockMode.Locked;
			#endif
		}
	}

	private void Update() {
		if(m_player == null || m_rewiredPlayer.controllers.GetLastActiveController() == null) return;

		Rewired.ControllerType type = m_rewiredPlayer.controllers.GetLastActiveController().type;
		bool controller = type != Rewired.ControllerType.Mouse && type != Rewired.ControllerType.Keyboard;

		// ui cursor doesn't know the real cursor mode, only player cursor does
		if(m_currentMode == CursorModes.LINE) {
			if(controller && !m_usingController) {
				m_uiCursor.ChangeModes(CursorModes.LINE);
				SetCursorState(m_currentMode);
			} else if(!controller && m_usingController) {
				m_uiCursor.ChangeModes(CursorModes.CURSOR);
				SetCursorState(m_currentMode);
			}

			if(!m_updateCalled && !controller) {
				m_uiCursor.ChangeModes(CursorModes.CURSOR);
				SetCursorState(m_currentMode);
			}
		}

		m_updateCalled = true;
		m_usingController = controller;
	}
}

public enum CursorModes {
	CURSOR, LINE
}