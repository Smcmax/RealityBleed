using UnityEngine;
using System.Collections.Generic;
using Rewired.UI.ControlMapper;

[CreateAssetMenu(menuName = "Language")]
public class Language : ScriptableObject {
    
    [Tooltip("The language's name, this is also the displayed name")]
    public string m_name;

    [Tooltip("The font to forcibly use, for languages with accents")]
    public Font m_forcedFont;

    [Tooltip("The entries allowing translation from English")]
    public List<LanguageEntry> m_entries;

    [Tooltip("The language data used for Rewired")]
    public LanguageData m_rewiredLanguageData;

    public bool HasEntry(string p_key) {
        return m_entries.Exists(le => le.m_key == p_key);
    }

    public string GetLine(string p_key) {
        return m_entries.Find(le => le.m_key == p_key).m_line;
    }

    public string FindKey(string p_line) {
        LanguageEntry entry = m_entries.Find(le => le.m_line == p_line);

        if(entry != null && HasEntry(entry.m_key)) return entry.m_key;
        else return "";
    }

    public void SetLine(string p_key, string p_line) {
        if(HasEntry(p_key))
            m_entries.Find(le => le.m_key == p_key).m_line = p_line;
        else m_entries.Add(new LanguageEntry(p_key, p_line));
    }

    [System.Serializable]
    public class LanguageEntry {
        [Tooltip("The key used to retrieve this line, usually is the English equivalent")]
        public string m_key;

        [Tooltip("The translated line (from English)")]
        public string m_line;

        public LanguageEntry(string p_key, string p_line) {
            m_key = p_key;
            m_line = p_line;
        }
    }
}