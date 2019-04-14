using UnityEngine;
using UnityEngine.UI;

public class UICursor : MonoBehaviour {

	[Tooltip("The image holding the cursor's current sprite")]
	public Image m_image;

	[Tooltip("The sprite used by the cursor when it is in cursor form")]
	public Sprite m_cursorSprite;
	
	[Tooltip("The sprite used by the cursor when it is in line form")]
	public Sprite m_lineSprite;

	void Start() {
		m_image.raycastTarget = false;
	}

	public void ChangeModes(CursorModes p_mode) {
		if(p_mode == CursorModes.LINE) {
			GetComponent<RectTransform>().sizeDelta = new Vector2(8, 640);

			m_image.sprite = m_lineSprite;
			m_image.type = Image.Type.Simple;
			m_image.preserveAspect = true;
			m_image.color = new Color(1, 1, 1, 0.5f);
		} else {
			transform.rotation = Quaternion.Euler(0, 0, 0);
			GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
			m_image.sprite = m_cursorSprite;
			m_image.color = new Color(1, 1, 1, 1);
		}
	}

	private void UpdateSprite(Sprite p_sprite) {
		m_image.sprite = p_sprite;
	}
}
