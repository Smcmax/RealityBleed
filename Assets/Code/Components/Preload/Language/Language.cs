using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Rewired.UI.ControlMapper;

[Serializable]
public class Language {

    public string m_name;
    public bool m_accentLanguage;
    public List<LanguageEntry> m_entries;
    // TODO: figure out how to get rewired language? or force it

    private string m_file;

    public bool HasEntry(string p_key) {
        return m_entries.Exists(le => le.m_key == p_key);
    }

    public string GetLine(string p_key) {
        return m_entries.Find(le => le.m_key == p_key).m_line;
    }

    public string FindKey(string p_line) {
        LanguageEntry entry = m_entries.Find(le => le.m_line == p_line);

        if (entry != null && HasEntry(entry.m_key)) return entry.m_key;
        else return "";
    }

    public void SetLine(string p_key, string p_line) {
        if (HasEntry(p_key))
            m_entries.Find(le => le.m_key == p_key).m_line = p_line;
        else m_entries.Add(new LanguageEntry(p_key, p_line));
    }

    public void Save() {
        StreamWriter writer = new StreamWriter(m_file, false);

        writer.Write(JsonUtility.ToJson(this));
        writer.Close();
    }

    public static Language Load(string p_path) {
        StreamReader reader = new StreamReader(p_path);
        Language language = JsonUtility.FromJson<Language>(reader.ReadToEnd());
        language.m_file = p_path;

        reader.Close();
        return language;
    }

    [Serializable]
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