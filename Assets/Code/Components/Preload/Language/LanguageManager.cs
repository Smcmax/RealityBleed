using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LanguageManager : MonoBehaviour {

    [Tooltip("The list of loaded languages")]
    public List<Language> m_languages;

    void Awake() {
        foreach(string file in System.IO.Directory.GetFiles(Application.dataPath + "/Data/Languages/"))
            if(file.EndsWith(".JSON"))
                m_languages.Add(Language.Load(file));

        Game.m_options.LoadOptionString("Language", "English");
    }

    public Language GetLanguage(string p_language) {
        return m_languages.Find(l => l.m_name == p_language);
    }

    public Language GetCurrentLanguage() {
        string currentLanguage = Game.m_options.Get("Language").GetString();
        Language language = GetLanguage(currentLanguage);

        if(language == null) language = GetLanguage("English");

        return language;
    }

    public string GetLine(string p_key) {
        Language current = GetCurrentLanguage();
		Language english = GetLanguage("English");

		if(!current.HasEntry(p_key)) {
            if(english.HasEntry(p_key))
                SetDefaults();
            else {
				english.SetLine(p_key, p_key);
				english.Save();

				SetDefaults();
			}
        }

        return current.GetLine(p_key);
    }

    public string FormatKeys(string p_key, params string[] p_keysToFormat) {
        List<string> textsToFormat = new List<string>();

        foreach(string key in p_keysToFormat)
            textsToFormat.Add(GetLine(key));

        return FormatTexts(GetLine(p_key), textsToFormat.ToArray());
    }

    public string FormatTexts(string p_text, params string[] p_toFormat) {
        string finalText = p_text;

        for(int i = 0; i < p_toFormat.Length; i++)
            finalText = finalText.Replace("{" + i + "}", p_toFormat[i]);
        
        return finalText;
    }

    public void UpdateUILanguage() {
        //Language language = GetCurrentLanguage();

        //if(language.m_rewiredLanguageData)
            //Game.m_controlMapper.language = language.m_rewiredLanguageData;

        if(MenuHandler.Instance && MenuHandler.Instance.m_openedMenus.Count > 0)
            foreach(Menu menu in MenuHandler.Instance.m_openedMenus)
                menu.UpdateMenuLanguage();
    }

    public void UpdateObjectLanguageNoChildren(GameObject p_object, Language p_previousLanguage) {
        if(p_object == Game.m_controlMapperMenu.gameObject) return;
        List<Component> components = new List<Component>();

        components.AddRange(p_object.GetComponents<TextMeshProUGUI>());
        components.AddRange(p_object.GetComponents<TMP_Dropdown>());
        components.AddRange(p_object.GetComponents<AdaptativeSliderText>());

        foreach(Component component in components)
            UpdateComponentLanguage(component, p_previousLanguage);
    }

    public void UpdateObjectLanguage(GameObject p_object, Language p_previousLanguage) {
        if(p_object == Game.m_controlMapperMenu.gameObject) return;
        List<Component> components = new List<Component>();

        components.AddRange(p_object.GetComponentsInChildren<TextMeshProUGUI>(true));
        components.AddRange(p_object.GetComponentsInChildren<TMP_Dropdown>(true));
        components.AddRange(p_object.GetComponentsInChildren<AdaptativeSliderText>(true));

        foreach(Component component in components)
            UpdateComponentLanguage(component, p_previousLanguage);
    }

    public void UpdateComponentLanguage(Component p_component, Language p_previousLanguage) {
        if(p_component.tag == "No Translation") return;

        if(p_component is TextMeshProUGUI) {
            TextMeshProUGUI text = (TextMeshProUGUI) p_component;
            string updatedLine = GetLine(p_previousLanguage.FindKey(text.text));

            if(updatedLine.Length > 0)
                text.text = updatedLine;
        } else if(p_component is TMP_Dropdown) {
            TMP_Dropdown dropdown = (TMP_Dropdown) p_component;

            foreach(TMP_Dropdown.OptionData option in dropdown.options) {
                string updatedLine = GetLine(p_previousLanguage.FindKey(option.text));

                if(updatedLine.Length > 0)
                    option.text = updatedLine;
            }
        } else if(p_component is AdaptativeSliderText) 
            ((AdaptativeSliderText) p_component).UpdateSlider();
    }

    private void SetDefaults() {
        Language english = GetLanguage("English");
        Language current = GetCurrentLanguage();

        if(current != english) {
            foreach(Language.LanguageEntry entry in english.m_entries)
                if(!current.HasEntry(entry.m_key))
                    current.SetLine(entry.m_key, entry.m_line);
            
            current.Save();
        }
    }
}