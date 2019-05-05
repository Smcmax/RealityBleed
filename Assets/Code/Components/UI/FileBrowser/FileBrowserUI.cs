using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FileBrowserUI : MonoBehaviour {

	[Tooltip("The button prefab used to show directories in the UI")]
	public GameObject m_directoryButtonPrefab;

	[Tooltip("The button prefab used to show files in the UI")]
	public GameObject m_fileButtonPrefab;

	[Tooltip("The window that pops up whenever an error occurs")]
	public GameObject m_errorWindow;

	private CFileBrowser m_fileBrowser;
	private TMP_InputField m_pathField;
	private TMP_InputField m_fileField;
    private Button m_upButton;
	private Button m_backButton;
	private Button m_forwardButton;
	private Button m_refreshButton;
	private Button m_openSaveButton;
	private Button m_cancelButton;
	private GameObject m_viewerParent;

	public void Setup(CFileBrowser p_fileBrowser, CFileBrowser.BrowserMode m_mode) {
		m_fileBrowser = p_fileBrowser;

        m_pathField = transform.Find("Header").Find("PathField").GetComponent<TMP_InputField>();
        m_fileField = transform.Find("Footer").Find("FileNameField").GetComponent<TMP_InputField>();
        m_upButton = transform.Find("Header").Find("Up").GetComponent<Button>();
        m_backButton = transform.Find("Header").Find("Back").GetComponent<Button>();
        m_forwardButton = transform.Find("Header").Find("Forward").GetComponent<Button>();
        m_refreshButton = transform.Find("Header").Find("Refresh").GetComponent<Button>();
        m_openSaveButton = transform.Find("Footer").Find("OpenSave").GetComponent<Button>();
        m_cancelButton = transform.Find("Footer").Find("Cancel").GetComponent<Button>();
		m_viewerParent = transform.Find("Viewer").Find("Scroll View").Find("Viewport").Find("ViewerContent").gameObject;

		Game.m_languages.UpdateObjectLanguage(gameObject, Game.m_languages.GetLanguage("English"));

		if(m_mode == CFileBrowser.BrowserMode.SAVE)
			m_openSaveButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Game.m_languages.GetLine("Save");
		else m_openSaveButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Game.m_languages.GetLine("Open");

		SetupListeners();
	}

	private void SetupListeners() {
		m_upButton.onClick.AddListener(m_fileBrowser.GoUp);
        m_backButton.onClick.AddListener(m_fileBrowser.GoBack);
        m_forwardButton.onClick.AddListener(m_fileBrowser.GoForward);
        m_refreshButton.onClick.AddListener(m_fileBrowser.LoadViewer);
		m_openSaveButton.onClick.AddListener(m_fileBrowser.SelectFile);
		m_cancelButton.onClick.AddListener(m_fileBrowser.CloseFileBrowser);

		m_pathField.onEndEdit.AddListener(delegate { m_fileBrowser.GoTo(m_pathField.text); });
		m_fileField.onEndEdit.AddListener(delegate { 
			if(!m_fileBrowser.ValidateFileEditingInput(m_fileField.text)) 
				m_fileField.text = ""; 
		});
	}

	public void ToggleUp(bool p_on) {
		m_upButton.interactable = p_on;
		m_upButton.GetComponent<Image>().color = new Color(1, 1, 1, p_on ? 1 : 0.5f);
	}

	public void ToggleBack(bool p_on) {
        m_backButton.interactable = p_on;
        m_backButton.GetComponent<Image>().color = new Color(1, 1, 1, p_on ? 1 : 0.5f);
	}

    public void ToggleForward(bool p_on) {
        m_forwardButton.interactable = p_on;
        m_forwardButton.GetComponent<Image>().color = new Color(1, 1, 1, p_on ? 1 : 0.5f);
    }

	public void UpdatePath(string p_path) {
		m_pathField.text = p_path;
	}

	public string GetFileName() {
		return m_fileField.text;
	}

	public void UpdateFileName(string p_name) {
        m_fileField.text = p_name;
	}

	public void Error(string p_error) {
		GameObject errorWindow = Instantiate(m_errorWindow, transform.parent, false);
		errorWindow.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = p_error;

		Button closeButton = errorWindow.transform.Find("Close").GetComponent<Button>();
		closeButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = Game.m_languages.GetLine("Close");
		closeButton.onClick.AddListener(delegate { Destroy(errorWindow); });
	}

	public void AddDirectoryButton(string p_directory) {
		GameObject directoryObj = Instantiate(m_directoryButtonPrefab, m_viewerParent.transform, false);

		directoryObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = new DirectoryInfo(p_directory).Name;
        directoryObj.GetComponent<Button>().onClick.AddListener(delegate { m_fileBrowser.OnDirectoryClick(p_directory); });
	}

	public void AddFileButton(string p_file) {
        GameObject fileObj = Instantiate(m_fileButtonPrefab, m_viewerParent.transform, false);

        fileObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Path.GetFileName(p_file);
		fileObj.AddComponent<FileButton>().Setup(Path.GetFileName(p_file), m_fileBrowser);
	}

	public void ClearViewer() {
        if(m_viewerParent.transform.childCount > 0)
            foreach(Transform child in m_viewerParent.transform)
                Destroy(child.gameObject);

		ScrollRect rect = m_viewerParent.transform.GetComponentInParent<ScrollRect>();

		if(rect != null) {
			rect.verticalNormalizedPosition = 1;
			rect.horizontalNormalizedPosition = 0;
		}
	}

	public class FileButton : ClickHandler {
		private string m_path;
		private CFileBrowser m_browser;

		public void Setup(string p_path, CFileBrowser p_browser) {
			m_path = p_path;
			m_browser = p_browser;
		}

		protected override void OnLeftSingleClick(GameObject p_clicked, Player p_clicker) {
			m_browser.OnFileClick(m_path);
		}

		protected override void OnLeftDoubleClick(GameObject p_clicked, Player p_clicker) {
			m_browser.OnFileClick(m_path);
			m_browser.SelectFile();
		}
	}
}
