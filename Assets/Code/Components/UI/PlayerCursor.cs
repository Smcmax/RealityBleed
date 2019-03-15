using Rewired.Components;
using System.Collections.Generic;
using UnityEngine;

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

	private GameObject m_cursor;
	private GameObject m_uiCursor;
	private PlayerMouse m_mouse;

	void Start() {
		if(m_playerId >= 0)
			Init(m_playerId);
	}

	public void Init(int p_playerId) {
		if(m_mouseParent) m_cursor = Instantiate(m_mousePrefab, m_mouseParent.transform);
		else m_cursor = Instantiate(m_mousePrefab);

		m_uiCursor = Instantiate(m_uiCursorPrefab, m_uiCanvas.transform);
		m_uiCursor.transform.localScale = new Vector3(m_spriteScale, m_spriteScale, m_spriteScale);
		
		SetCursorState();

		m_mouse = m_cursor.GetComponent<PlayerMouse>();
		m_mouse.useHardwarePointerPosition = false;
		m_mouse.playerId = p_playerId;

		m_mouse.xAxis.actionName = "AimX";
		m_mouse.yAxis.actionName = "AimY";
		m_mouse.leftButton.actionName = "UIInteract1";
		m_mouse.rightButton.actionName = "UIInteract2";
		m_mouse.middleButton.actionName = "UIInteract3";
		m_mouse.wheel.yAxis.actionName = "UIWheelY";

		m_mouse.pointerSpeed = m_cursorSpeed;
		m_mouse.screenPosition = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

		List<PlayerMouse> mice = Game.m_rewiredEventSystem.PlayerMice;
		mice.Add(m_mouse);
		Game.m_rewiredEventSystem.PlayerMice = mice;

		m_mouse.ScreenPositionChangedEvent += OnScreenPositionChanged;
		OnScreenPositionChanged(m_mouse.screenPosition);
	}

	public Vector2 GetPosition() { 
		return m_mouse.screenPosition;
	}

	private void OnScreenPositionChanged(Vector2 p_position) {
		if(!m_useUICoordinatesOnly)
			m_cursor.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(p_position.x, p_position.y, m_distanceFromCamera));
		else m_cursor.transform.position = p_position;

		m_uiCursor.transform.position = p_position;
	}

	private void SetCursorState() {
		Cursor.visible = !m_hideHardwarePointer;
		Cursor.lockState = CursorLockMode.Confined;

#if UNITY_EDITOR
		Cursor.lockState = CursorLockMode.Locked;
#endif
	}
}
