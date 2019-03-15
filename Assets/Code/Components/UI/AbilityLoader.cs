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

	[Tooltip("The tooltip used to display every ability")]
	public AbilitySkillTooltip m_tooltip;

	[Tooltip("The contextual menu that opens when you click on an ability")]
	public GameObject m_contextualAbilityMenuObject;

	[HideInInspector] public Player m_entity;
	[HideInInspector] public List<UIAbility> m_loadedAbilities;
	[HideInInspector] public AbilityContextualMenu m_contextualAbilityMenuScript;
	[HideInInspector] public Menu m_contextualAbilityMenu;

	void OnEnable() { 
		m_entity = Player.GetPlayerFromId(MenuHandler.Instance.m_handlingPlayer.id);
		m_loadedAbilities = new List<UIAbility>();
		m_contextualAbilityMenuScript = m_contextualAbilityMenuObject.GetComponent<AbilityContextualMenu>();
		m_contextualAbilityMenu = m_contextualAbilityMenuObject.GetComponent<Menu>();

		Load();
	}

	void OnDisable() {
		int children = m_contentParent.childCount;

		for(int i = children - 1; i >= 0; --i) {
			GameObject child = m_contentParent.GetChild(i).gameObject;

			Destroy(child);
		}

		m_loadedAbilities.Clear();

		if(m_contextualAbilityMenuObject.activeSelf) { 
			if(m_contextualAbilityMenuScript.IsChaining()) m_contextualAbilityMenuScript.StopChaining(false);

			MenuHandler.Instance.CloseMenu(m_contextualAbilityMenu);
		}
	}

	public void Load() { 
		foreach(DamageType domain in m_domains) { 
			GameObject domainObject = Instantiate(m_domainPrefab, m_contentParent);
			domainObject.transform.Find("Icon").GetComponent<Image>().sprite = domain.m_icon;

			Transform contentParent = domainObject.transform.Find("Scroll View").GetChild(0).GetChild(0);

			List<AbilityWrapper> sortedDomainAbilities = new List<AbilityWrapper>(m_entity.m_abilities.FindAll(a => a.Ability.m_domain == domain));
			sortedDomainAbilities.Sort(new AbilityComparer());
			sortedDomainAbilities.Reverse();

			foreach(AbilityWrapper ability in sortedDomainAbilities) {
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

				uiAbility.m_ability = ability;
				uiAbility.m_loader = this;
				uiAbility.Init();

				m_loadedAbilities.Add(uiAbility);
			}
		}
	}
}

public class AbilityComparer : IComparer<AbilityWrapper> {
	public int Compare(AbilityWrapper x, AbilityWrapper y) {
		return x.TrainingLevel.CompareTo(y.TrainingLevel);
	}
}