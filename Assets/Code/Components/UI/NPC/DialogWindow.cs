using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class DialogWindow : Selectable {

    public static List<DialogWindow> m_openedWindows = new List<DialogWindow>();

    [Tooltip("The panel's border size")]
    public float m_borderSize;

    private DialogController m_controller;
    private RectTransform m_panelTransform;
    private RectTransform m_headerTransform;
    private TextMeshProUGUI m_titleText;
    private TextMeshProUGUI m_bodyText;
    private GameObject m_choiceTemplate;
    private UIWorldSpaceFollower m_follower;
    private List<GameObject> m_spawnedChoices;
    private bool m_pressed = false;
    private float m_openedTime;

    void Update() {
        if((IsPressed() || m_controller.m_interactor.m_rewiredPlayer.GetButtonDown("UISubmit")) && 
            !m_pressed && Time.unscaledTime - m_openedTime > 0.2f) {
            m_pressed = true;
            m_controller.Interact();
        }
    }

    private void Init() {
        m_titleText = transform.Find("Header").Find("Title").GetComponent<TextMeshProUGUI>();
        m_bodyText = transform.Find("Body").GetComponent<TextMeshProUGUI>();
        m_choiceTemplate = transform.Find("Dialog Choice").gameObject;
        m_panelTransform = GetComponent<RectTransform>();
        m_headerTransform = m_titleText.transform.parent.GetComponent<RectTransform>();
        m_follower = GetComponent<UIWorldSpaceFollower>();
        m_spawnedChoices = new List<GameObject>();
    }

    public void Setup(DialogController p_controller, string p_title) {
        Init();

        m_controller = p_controller;
        m_follower.m_parent = p_controller.gameObject.transform;
        m_follower.m_offset += new Vector3(0, 1f);
        m_titleText.text = p_title;
        gameObject.SetActive(true);

        m_openedWindows.Add(this);

        Clear();
    }

    public void Display(string p_text) {
        Clear();

        m_bodyText.gameObject.SetActive(true);
        m_bodyText.text = p_text;

        float height = LayoutUtility.GetPreferredHeight(m_bodyText.rectTransform) + m_borderSize * 2 + 12;
        height += m_headerTransform.sizeDelta.y;
        
        m_panelTransform.sizeDelta = new Vector2(m_panelTransform.sizeDelta.x, height);
        m_follower.m_offset = new Vector3(0, height / 97.5f + 0.5f);

        m_openedTime = Time.unscaledTime;
        m_pressed = false;
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void DisplayChoices(List<Choice> p_choices) {
        Clear();

        float height = 0;

        foreach(Choice choice in p_choices) {
            GameObject spawned = Instantiate(m_choiceTemplate, transform);
            spawned.SetActive(true);

            RectTransform rect = spawned.GetComponent<RectTransform>();
            TextMeshProUGUI text = spawned.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            Button button = spawned.GetComponent<Button>();

            button.onClick.AddListener(delegate { if(Time.timeScale == 0f) return; choice.React(m_controller); });
            text.text = m_controller.GetFormattedLine(choice.m_line);

            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - height);
            height += rect.sizeDelta.y;

            m_spawnedChoices.Add(spawned);
        }

        m_openedTime = Time.unscaledTime;
        m_pressed = false;
        EventSystem.current.SetSelectedGameObject(m_spawnedChoices[0]);

        height += m_headerTransform.sizeDelta.y + m_borderSize * 2 + 12;
        m_panelTransform.sizeDelta = new Vector2(m_panelTransform.sizeDelta.x, height);
        m_follower.m_offset = new Vector3(0, height / 97.5f + 0.5f);
    }

    public void Clear() {
        m_bodyText.text = "";
        m_bodyText.gameObject.SetActive(false);

        if(m_spawnedChoices.Count > 0) {
            foreach(GameObject choice in new List<GameObject>(m_spawnedChoices))
                Destroy(choice);
            
            m_spawnedChoices.Clear();
        }
    }
}