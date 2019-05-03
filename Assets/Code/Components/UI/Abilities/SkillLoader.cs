using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SkillLoader : MonoBehaviour {

	[Tooltip("The object under which every domain prefab will be added, should be Content in the ScrollView")]
	public Transform m_contentParent;

	[Tooltip("The prefab to use when creating a skill")]
	public GameObject m_skillPrefab;

	[Tooltip("The alpha to use on skills which aren't learned")]
	[Range(0, 255)] public int m_nonLearnedAlpha;

	[Tooltip("The tooltip used to display every skill")]
	public AbilitySkillTooltip m_tooltip;

	[HideInInspector] public Entity m_entity;

	void OnEnable() {
		m_entity = Player.GetPlayerFromId(MenuHandler.Instance.m_handlingPlayer.id);

		Load();
	}

	void OnDisable() {
		int children = m_contentParent.childCount;

		for(int i = children - 1; i >= 0; --i) {
			GameObject child = m_contentParent.GetChild(i).gameObject;

			Destroy(child);
		}
	}

	public void Load() { 
		List<SkillWrapper> sortedSkills = new List<SkillWrapper>(m_entity.m_skills);
		sortedSkills.Sort(new SkillComparer());
		sortedSkills.Reverse();

		foreach(SkillWrapper skill in sortedSkills) {
			GameObject skillObject = Instantiate(m_skillPrefab, m_contentParent);
			Image skillIcon = skillObject.GetComponent<Image>();
            TextMeshProUGUI trainingLevel = skillObject.GetComponentInChildren<TextMeshProUGUI>();

			skillIcon.sprite = skill.Skill.m_icon;
			trainingLevel.text = skill.TrainingLevel.ToString();

			if(!skill.Learned) {
				trainingLevel.text = "";
				skillIcon.color = new Color(1, 1, 1, m_nonLearnedAlpha / 255f);
			}
				
			UISkill uiSkill = skillObject.GetComponent<UISkill>();

			uiSkill.m_skill = skill;
			uiSkill.m_loader = this;
		}
	}
}

public class SkillComparer : IComparer<SkillWrapper> {
	public int Compare(SkillWrapper x, SkillWrapper y) {
		return x.TrainingLevel.CompareTo(y.TrainingLevel);
	}
}