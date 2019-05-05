using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class FeedbackGenerator {

	// display color is transparent if no specified color
	public static void GenerateFeedback(Transform p_origin, GameObject p_template, DamageType p_type, int p_amount, Color p_defaultColor, Color p_displayColor, 
										float p_offsetX, float p_offsetY) {
		GameObject feedback = Object.Instantiate(p_template, p_template.transform.parent);
		TextMeshProUGUI feedbackText = feedback.GetComponent<TextMeshProUGUI>();
		Image damageTypeIcon = feedback.GetComponentInChildren<Image>();
		UIWorldSpaceFollower follow = feedback.GetComponent<UIWorldSpaceFollower>();
		Color feedbackColor = p_defaultColor;

		if(p_displayColor == Constants.TRANSPARENT) { // transparent means nothing specified
			if(p_defaultColor == Constants.TRANSPARENT)
				feedbackColor = p_amount > 0 ? Constants.GREEN : Constants.RED;
		} else feedbackColor = p_displayColor;

		if(p_type.m_icon) damageTypeIcon.sprite = p_type.m_icon;
		else damageTypeIcon.enabled = false;

		feedbackText.text = p_amount > 0 ? p_amount.ToString() : Mathf.Abs(p_amount).ToString();
		feedbackText.color = feedbackColor;
		follow.m_parent = p_origin;
		follow.m_offset += new Vector3(Random.Range(-p_offsetX / 2, p_offsetX / 2), Random.Range(-p_offsetY / 2, p_offsetY / 2));

		feedback.SetActive(true);
	}
}
