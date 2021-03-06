﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFileBrowser : MonoBehaviour {

    public enum BrowserMode {
		LOAD, SAVE
    }

	[Tooltip("The UI Prefab to use with this file browser, must have the FileBrowserUI component")]
	public GameObject m_uiPrefab;

    public event Action<string> OnFileSelect;
    public event Action<string> OnFileBrowserClose;

	private static string LAST_OPENED_PATH = "";

	private Canvas m_canvas;
	private FileBrowserUI m_ui;
	private Menu m_menu;
	private BrowserMode m_mode;

	private Stack<string> m_backwardStack = new Stack<string>();
	private Stack<string> m_forwardStack = new Stack<string>();

	private string m_currentPath;
	private string[] m_extensions;

	void Setup() {
		OnFileBrowserClose = delegate { };
		OnFileSelect = delegate { };

		m_canvas = GetComponentInParent<Canvas>();
		m_menu = GetComponent<Menu>();

		if(transform.childCount > 0)
			Destroy(transform.GetChild(0).gameObject);
	}

	public void Open(Menu p_previousMenu) {
		Open(p_previousMenu, "", "");
	}

	public void Open(Menu p_previousMenu, string p_startPath = "", string p_defaultSaveName = "", params string[] p_extensions) {
        Setup();

		if(m_canvas == null) { Debug.LogError("This file browser has no canvas parent!"); return; }
        if(m_uiPrefab == null) { Debug.LogError("Please assign the UI prefab to this file browser!"); return; }

		if(p_previousMenu) m_menu.m_previousMenu = p_previousMenu;

		GameObject uiObject = Instantiate(m_uiPrefab, transform, false);
		m_ui = uiObject.GetComponent<FileBrowserUI>();

		if(m_ui == null) { 
			Debug.LogError("The UI Prefab does not have the FileBrowserUI component!"); 
			Destroy(uiObject);
			return; 
		}

		if(p_defaultSaveName.Length > 0) m_mode = BrowserMode.SAVE;
		else m_mode = BrowserMode.LOAD;

		m_extensions = p_extensions;

		m_ui.Setup(this, m_mode);
		SetupPath(p_startPath);
		LoadViewer();

		if(m_mode == BrowserMode.SAVE)
            m_ui.UpdateFileName(p_defaultSaveName + "." + m_extensions[0]);

		MenuHandler.Instance.OpenMenu(m_menu);
	}

    private void SetupPath(string p_startPath) {
        if(p_startPath == null || (p_startPath.Length > 0 && Directory.Exists(p_startPath)))
            m_currentPath = p_startPath;
		else if(LAST_OPENED_PATH == null || (LAST_OPENED_PATH.Length > 0 && Directory.Exists(LAST_OPENED_PATH)))
			m_currentPath = LAST_OPENED_PATH;
        else m_currentPath = Directory.GetCurrentDirectory();
    }

	private bool IsTopLevel() {
        return m_currentPath == null;
	}

	private bool IsParentTopLevel() {
		return IsTopLevel() ? true : Directory.GetParent(m_currentPath) == null;
	}

    private bool IsWindowsPlatform() {
        return (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.WindowsPlayer);
    }

    private bool IsMacOsPlatform() {
        return (Application.platform == RuntimePlatform.OSXEditor ||
                Application.platform == RuntimePlatform.OSXPlayer);
    }

	private bool IsLinuxPlatform() {
		return (Application.platform == RuntimePlatform.LinuxEditor ||
				Application.platform == RuntimePlatform.LinuxPlayer);
	}

    public bool CompatibleFileExtension(string p_file) {
        if(m_extensions.Length == 0) return true;

        foreach(string extension in m_extensions)
            if(p_file.EndsWith("." + extension)) 
				return true;

        return false;
    }

	public void GoUp() {
		string current = m_currentPath;

        if(!IsParentTopLevel())
            m_currentPath = Directory.GetParent(m_currentPath).FullName;
		else if(IsLinuxPlatform()) return;
		else m_currentPath = null;

        m_backwardStack.Push(current);

        LoadViewer();
	}

	public void GoBack() {
        if(m_backwardStack.Count == 0) return;

        m_forwardStack.Push(m_currentPath);

        string backPath = m_backwardStack.Pop();

        if(backPath == null || backPath.Length > 0) {
            m_currentPath = backPath;
            LoadViewer();
        }
	}

	public void GoForward() {
        if(m_forwardStack.Count == 0) return;

        m_backwardStack.Push(m_currentPath);

        string forwardPath = m_forwardStack.Pop();

        if(forwardPath == null || forwardPath.Length > 0) {
            m_currentPath = forwardPath;
            LoadViewer();
        }
	}

	public void GoTo(string p_path) {
		if(!String.IsNullOrEmpty(p_path) && Directory.Exists(p_path)) {
            m_backwardStack.Push(m_currentPath);
            m_currentPath = p_path;
            LoadViewer();
		}
	}

	public void LoadViewer() {
		m_ui.ToggleUp(!IsTopLevel());
		m_ui.ToggleBack(m_backwardStack.Count > 0);
		m_ui.ToggleForward(m_forwardStack.Count > 0);

        LAST_OPENED_PATH = m_currentPath;

		m_ui.UpdatePath(m_currentPath);
		m_ui.ClearViewer();
		LoadDirectories();
		LoadFiles();
	}

	public bool ValidateFileEditingInput(string p_file) {
        if(!CompatibleFileExtension(p_file)) {
			string extensions = "";

			foreach(string ext in m_extensions)
				extensions += ", ." + ext;

			extensions = extensions.Substring(2);
			m_ui.Error(Game.m_languages.FormatTexts(Game.m_languages.GetLine("File needs to have one of the following extensions: {0}"), extensions));

			return false;
        }

		return true;
	}

	public void SelectFile() {
		if(!CompatibleFileExtension(m_ui.GetFileName())) return;

		OnFileSelect(m_currentPath + "/" + m_ui.GetFileName());
		CloseBrowser();
	}

	public void CloseFileBrowser() {
		OnFileBrowserClose("");
		CloseBrowser();
	}

	private void CloseBrowser() {
		Destroy(m_ui.gameObject);

		if(m_menu.m_previousMenu is TabMenu)
			MenuHandler.Instance.OpenTabMenu((TabMenu) m_menu.m_previousMenu);
		else MenuHandler.Instance.OpenMenu(m_menu.m_previousMenu);
	}

	private void LoadDirectories() {
		string[] directories = new string[]{};

		if(IsParentTopLevel()) {
			if(IsTopLevel()) {
				if(IsWindowsPlatform()) {
					directories = Directory.GetLogicalDrives();
				} else if(IsMacOsPlatform()) {
					directories = Directory.GetDirectories("/Volumes");
				}
			} else if(IsLinuxPlatform()) {
				directories = Directory.GetDirectories("/");
			} else directories = Directory.GetDirectories(m_currentPath);
		} else directories = Directory.GetDirectories(m_currentPath);

        Array.Sort(directories, new AlphanumComparator());

        foreach(string directory in directories)
            if(Directory.Exists(directory))
                m_ui.AddDirectoryButton(directory);
	}

	private void LoadFiles() {
		if(m_currentPath == null) return;

		string[] files = Directory.GetFiles(m_currentPath);

        Array.Sort(files, new AlphanumComparator());

        foreach(string file in files)
            if(File.Exists(file) && CompatibleFileExtension(file))
                m_ui.AddFileButton(file);
	}

	public void OnDirectoryClick(string p_directory) {
		if(m_currentPath != null) m_backwardStack.Push(m_currentPath.Clone() as string);
		else m_backwardStack.Push(null);

		m_currentPath = p_directory;
		LoadViewer();
	}

	public void OnFileClick(string p_file) {
        m_ui.UpdateFileName(p_file);
	}
}
