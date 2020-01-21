using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class TabMenu : Menu {

	[Tooltip("The tabs making up this menu, the name MUST be the game object's name, first opened tab should be first")]
	public List<TabHeader> m_tabs;

	[HideInInspector] public string m_currentTab;

    new void OnEnable() {
        base.OnEnable();

        string current = m_currentTab;
        if(string.IsNullOrEmpty(current)) current = m_tabs[0].Name;

        EventSystem.current.SetSelectedGameObject(m_tabs.Find(th => th.Name == current)
                                                        .TabTransitioner.gameObject);
    }

    public void ResetTab() {
		m_currentTab = m_tabs[0].Name;
        Select(m_currentTab, false);
	}

	public void Select(string p_tabName, bool p_fireEvent) { 
		if(m_tabs.Exists(th => th.Name == p_tabName)) {
			OnClickButtonTransitioner transitioner = m_tabs.Find(th => th.Name == p_tabName).TabTransitioner;
			EventSystem.current.SetSelectedGameObject(transitioner.gameObject);
			transitioner.Select(p_fireEvent);
        }
	}
}

[Serializable]
public class TabHeader { 
	[Tooltip("MUST be the game object's name")]
	public string Name;
	public OnClickButtonTransitioner TabTransitioner;
}
