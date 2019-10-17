using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ScrollRect))]
public class SelectableSelectionController : MonoBehaviour {

    [Tooltip("Smoothing time for the automatic vertical scroll")]
    public float m_smoothTime = 0.1f;

    [Tooltip("Force the selectables to select this selectable on left select")]
    public Selectable m_forceLeftSelectable;

    [Tooltip("Force the selectables to select this selectable on right select")]
    public Selectable m_forceRightSelectable;

    private ScrollRect m_scrollRect;
    private List<Selectable> m_selectables;
    private int m_index;
    private float m_verticalPosition;
    private float m_lastScrollRectPosition;
    private bool m_up;
    private bool m_down;
    private float m_lastMove;

    public void Init() {
        m_scrollRect = GetComponent<ScrollRect>();
        m_selectables = new List<Selectable>(GetComponentsInChildren<Selectable>());

        foreach(Selectable selectable in new List<Selectable>(m_selectables))
            if(selectable is Scrollbar)
                m_selectables.Remove(selectable);

        if(m_selectables.Count == 0) return;

        m_verticalPosition = 1f - ((float) m_index / (m_selectables.Count - 1));

        Navigation navigation = new Navigation();

        navigation.mode = Navigation.Mode.Explicit;
        navigation.selectOnLeft = m_forceLeftSelectable;
        navigation.selectOnRight = m_forceRightSelectable;

        foreach(Selectable selectable in m_selectables)
            selectable.navigation = navigation;
    }

    public void Update() {
        if(m_selectables == null || m_selectables.Count == 0) return;

        float yAxis = MenuHandler.Instance.m_handlingPlayer.GetAxisRaw("UIMoveY");

        if(yAxis == 0 || Time.time - m_lastMove < 0.2f) return;

        m_lastMove = Time.time;
        m_up = yAxis > 0;
        m_down = yAxis < 0;

        if(m_up ^ m_down) {
            if(m_up) m_index = Mathf.Clamp(m_index - 1, 0, m_selectables.Count - 1);
            else m_index = Mathf.Clamp(m_index + 1, 0, m_selectables.Count - 1);

            m_selectables[m_index].Select();
            m_verticalPosition = 1f - ((float) m_index / (m_selectables.Count - 1));
        }

        if(m_scrollRect.verticalNormalizedPosition != m_lastScrollRectPosition && 
            m_lastScrollRectPosition != 0 &&
            !m_up && !m_down) return;

        m_scrollRect.verticalNormalizedPosition = Mathf.Lerp(m_scrollRect.verticalNormalizedPosition, 
                                                             m_verticalPosition, 
                                                             Time.deltaTime / m_smoothTime);

        m_lastScrollRectPosition = m_scrollRect.verticalNormalizedPosition;
    }
}
