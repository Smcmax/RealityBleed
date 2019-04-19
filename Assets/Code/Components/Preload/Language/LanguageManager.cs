using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LanguageManager : MonoBehaviour {

    [Tooltip("Standard font")]
    public Font m_defaultFont;

    [Tooltip("The list of loaded languages")]
    public List<Language> m_languages;

    void Awake() {
        Game.m_options.LoadOptionString("Language", "English");
    }

    public Language GetCurrentLanguage() {
        string currentLanguage = Game.m_options.Get("Language").GetString();
        Language language = m_languages.Find(l => l.m_name == currentLanguage);

        if(language == null) language = m_languages.Find(l => l.m_name == "English");

        return language;
    }

    public string GetLine(string p_key) {
        Language current = GetCurrentLanguage();
        
        if(!current.HasEntry(p_key)) {
            if(m_languages.Find(l => l.m_name == "English").HasEntry(p_key))
                SetDefaults();
            else return "";
        }

        return current.GetLine(p_key);
    }

    public void UpdateUILanguage() {
        Language language = GetCurrentLanguage();

        if(language.m_rewiredLanguageData)
            Game.m_controlMapper.language = language.m_rewiredLanguageData;

        if(MenuHandler.Instance && MenuHandler.Instance.m_openedMenus.Count > 0)
            foreach(Menu menu in MenuHandler.Instance.m_openedMenus)
                menu.UpdateMenuLanguage();
    }

    public void UpdateMenuLanguage(Menu p_menu, Language p_previousLanguage) {
        if(p_menu == Game.m_controlMapperMenu) return;
        List<Component> components = new List<Component>();

        components.AddRange(p_menu.GetComponentsInChildren<Text>());
        components.AddRange(p_menu.GetComponentsInChildren<Dropdown>());

        foreach(Component component in components)
            UpdateComponentLanguage(component, p_previousLanguage);
    }

    private void UpdateComponentLanguage(Component p_component, Language p_previousLanguage) {
        Language current = GetCurrentLanguage();

        if(p_component is Text) {
            Text text = (Text) p_component;
            string updatedLine = GetLine(p_previousLanguage.FindKey(text.text));

            if(updatedLine.Length > 0)
                text.text = updatedLine;

            if(current.m_forcedFont)
                text.font = current.m_forcedFont;
            else text.font = m_defaultFont;
        } else if(p_component is Dropdown) {
            Dropdown dropdown = (Dropdown) p_component;

            foreach(Dropdown.OptionData option in dropdown.options) {
                string updatedLine = GetLine(p_previousLanguage.FindKey(option.text));

                if(updatedLine.Length > 0)
                    option.text = updatedLine;
            }
        } else if(p_component is TextualSliderText) {
            TextualSliderText tst = (TextualSliderText) p_component;

            foreach(ValueTextPair pair in tst.m_valueTextPairs) {
                string updatedLine = GetLine(p_previousLanguage.FindKey(pair.m_text));

                if(updatedLine.Length > 0)
                    pair.m_text = updatedLine;
            }
        } else if(p_component is AdaptativeSliderText) {
            AdaptativeSliderText ast = (AdaptativeSliderText) p_component;
            string updatedLine = GetLine(p_previousLanguage.FindKey(ast.m_prefixText));

            if(updatedLine.Length > 0)
                ast.m_prefixText = updatedLine;

            updatedLine = GetLine(p_previousLanguage.FindKey(ast.m_suffixText));

            if(updatedLine.Length > 0)
                ast.m_suffixText = updatedLine;
        }
    }

    private void SetDefaults() {
        Language english = m_languages.Find(l => l.m_name == "English");
        Language current = GetCurrentLanguage();

        if(current != english)
            foreach(Language.LanguageEntry entry in english.m_entries)
                if(!current.HasEntry(entry.m_key))
                    current.SetLine(entry.m_key, entry.m_line);
    }
}