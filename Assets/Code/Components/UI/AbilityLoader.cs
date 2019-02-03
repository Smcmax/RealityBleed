using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityLoader : MonoBehaviour {

	[Tooltip("The prefab to use when creating the ability panel, it should be able to show every ability in that domain")]
	public GameObject m_domainPrefab;

	[Tooltip("The object under which every domain prefab will be added, should be Content in the ScrollView")]
	public Transform m_contentParent;

	[Tooltip("The prefab to use when creating an ability")]
	public GameObject m_abilityPrefab;

	[Tooltip("The alpha to use on abilities which aren't learned")]
	[Range(0, 255)] public int m_nonLearnedAlpha;

	[Tooltip("The list of domains which will have their abilities listed")]
	public List<DamageType> m_domains;

	[Tooltip("The tooltip used to display every ability/skill")]
	public AbilitySkillTooltip m_tooltip;

	[HideInInspector] public Entity m_entity;

	void OnEnable() { 
		m_entity = Game.m_keybinds.m_entity; // TODO: support local co-op

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
		foreach(DamageType domain in m_domains) { 
			GameObject domainObject = Instantiate(m_domainPrefab, m_contentParent);
			domainObject.transform.Find("Icon").GetComponent<Image>().sprite = domain.m_icon;

			Transform contentParent = domainObject.transform.Find("Scroll View").GetChild(0).GetChild(0);

			foreach(AbilityWrapper ability in m_entity.m_abilities.FindAll(a => a.Ability.m_domain == domain)) {
				GameObject abilityObject = Instantiate(m_abilityPrefab, contentParent);
				Image abilityIcon = abilityObject.GetComponent<Image>();
				Text trainingLevel = abilityObject.GetComponentInChildren<Text>();

				abilityIcon.sprite = ability.Ability.m_icon;
				trainingLevel.text = ability.TrainingLevel.ToString();

				if(!ability.Learned) {
					trainingLevel.text = "";
					abilityIcon.color = new Color(1, 1, 1, m_nonLearnedAlpha / 255f);
				}
				
				UIAbility uiAbility = abilityObject.GetComponent<UIAbility>();

				uiAbility.m_abilitySkill = new AbilitySkillWrapper();
				uiAbility.m_abilitySkill.Ability = ability;
				uiAbility.m_loader = this;
				
			}
		}
	}
}
