using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Tooltip : MonoBehaviour {

	[Tooltip("The canvas used by this tooltip. It moves with the mouse, so be careful!")]
	public Canvas m_canvas;

	[Tooltip("A dictionary assigning a name to each text field, make sure the name matches in the code")]
	public List<TooltipInfo> m_modifiableInfo;

	[Tooltip("The percentage of the screen's worth of offset applied to the tooltip's position. 0-100")]
	public Vector2 m_offsetPercentage;

	private RectTransform m_rectTransform;
	private RectTransform m_canvasRect;

	void Awake() { 
		m_rectTransform = GetComponent<RectTransform>();
		m_canvasRect = m_canvas.GetComponent<RectTransform>();
	}

	void Update() {
		Vector2 mouse = Player.GetPlayerFromId(MenuHandler.Instance.m_handlingPlayer.id).m_mouse.GetPosition();
		Vector2 adjustedMouse = new Vector2((mouse.x / (float) Screen.width) * m_canvasRect.sizeDelta.x, 
											(mouse.y / (float) Screen.height) * m_canvasRect.sizeDelta.y);
		float tooltipWorldWidth = (m_rectTransform.sizeDelta.x / m_canvasRect.sizeDelta.x) * Screen.width;
		float tooltipWorldHeight = (m_rectTransform.sizeDelta.y / m_canvasRect.sizeDelta.y) * Screen.height;
		float offsetPercentX = m_offsetPercentage.x / 100f;
		float offsetPercentY = m_offsetPercentage.y / 100f;

		// if the tooltip is too far to the right, shift it to cursor's left side
		if(mouse.x + (Screen.width * offsetPercentX) + tooltipWorldWidth / 2 > Screen.width) 
			offsetPercentX = -offsetPercentX;

		// just don't change the canvas' reference res please
		float calcX = adjustedMouse.x + m_canvasRect.sizeDelta.x * offsetPercentX * (1065f / m_canvasRect.sizeDelta.x);
		float calcY = adjustedMouse.y + m_canvasRect.sizeDelta.y * offsetPercentY;

		// if the tooltip is too far down, cap it to the bottom of the screen
		if(mouse.y + (Screen.height * offsetPercentY) - tooltipWorldHeight / 2 < 0)
			calcY = m_rectTransform.sizeDelta.y / 2;

		// if the tooltip is too far up, cap it to the top of the screen
		if(mouse.y + (Screen.height * offsetPercentY) + tooltipWorldHeight / 2 > Screen.height)
			calcY = m_canvasRect.sizeDelta.y - m_rectTransform.sizeDelta.y / 2;

		m_rectTransform.anchoredPosition = new Vector3(calcX, calcY);
	}

	public void Show(float p_panelHeight) {
		m_canvas.gameObject.SetActive(true);
		gameObject.SetActive(true);

		m_rectTransform.sizeDelta = new Vector2(m_rectTransform.sizeDelta.x, p_panelHeight);
	}

	public void Hide() {
		gameObject.SetActive(false);
		m_canvas.gameObject.SetActive(false);

		if(m_rectTransform) 
			m_rectTransform.anchoredPosition = new Vector3(-5000, -5000, 0); // throw it out of the screen to avoid flashing...
	}

	protected void FillModifiableInfo() { 
		foreach(Transform child in GetComponentsInChildren<Transform>())
			if(m_modifiableInfo.Find(ti => ti.m_name == child.name) == null)
				m_modifiableInfo.Add(new TooltipInfo(child.name, child.gameObject, child.GetComponent<RectTransform>()));
	}
}

// unity doesn't show dictionaries in the inspector, so this works as such
[System.Serializable]
public class TooltipInfo {
	public string m_name;
	public GameObject m_info;
	public RectTransform m_rect;
	private Language m_lastUpdatedLanguage;

	public TooltipInfo(string p_name, GameObject p_info, RectTransform p_rect) {
		m_name = p_name;
		m_info = p_info;
		m_rect = p_rect;
		m_lastUpdatedLanguage = Game.m_languages.m_languages.Find(l => l.m_name == "English");
	}

	public T Get<T>() {
		m_info.SetActive(true);

		Translate();
		return m_info.GetComponent<T>();
	}

	public T GetAligned<T>(ref float p_tooltipInfoOffset) {
		m_info.SetActive(true);
		m_rect.anchoredPosition = new Vector2(m_rect.anchoredPosition.x, p_tooltipInfoOffset - m_rect.rect.height / 2);

        Translate();
		return m_info.GetComponent<T>();
	}

	// only use this if adding the object's height into the panel size
	public T Get<T>(ref float p_totalHeight, ref float p_tooltipInfoOffset) {
		return Get<T>(ref p_totalHeight, ref p_tooltipInfoOffset, m_rect.rect.height);
	}

	public T Get<T>(ref float p_totalHeight, ref float p_tooltipInfoOffset, float p_rectHeight) {
		float newY = p_tooltipInfoOffset - p_rectHeight / 2;

		if(m_info.name.Contains("Separator")) newY -= 2;

		m_rect.anchoredPosition = new Vector2(m_rect.anchoredPosition.x, newY);
		p_totalHeight += p_rectHeight;
		p_tooltipInfoOffset -= p_rectHeight;
		m_info.SetActive(true);

        Translate();
		return m_info.GetComponent<T>();
	}

	private void Translate() {
        if(m_lastUpdatedLanguage == Game.m_languages.GetCurrentLanguage()) return;

		Game.m_languages.UpdateObjectLanguageNoChildren(m_info, m_lastUpdatedLanguage);
		m_lastUpdatedLanguage = Game.m_languages.GetCurrentLanguage();
	}
}