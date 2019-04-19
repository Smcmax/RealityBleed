using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OptionsMenuHandler : MonoBehaviour {

	[Tooltip("The dropdown handling the resolutions")]
	public Dropdown m_resolutionDropdown;

	[Tooltip("The dropdown handling the language selection")]
	public Dropdown m_languageDropdown;

	[Tooltip("The toggle for vsync")]
	public Toggle m_vsyncToggle;

	[Tooltip("The slider handling the framerate")]
	public AdaptativeSliderText m_framerateSlider;

	[Tooltip("The dropdown handling the quality settings")]
	public Dropdown m_qualityDropdown;

	[Tooltip("The canvas containing all the health bars")]
	public Canvas m_hpBarsCanvas;

	[HideInInspector] public List<Extruder> m_extruders;
	[HideInInspector] public List<GameLight> m_lights;

	private int m_framerate;
	private bool m_fullscreen;
	private bool m_vsync;
	private int m_qualityLevel;
	private int m_shadows;
	private int m_originalShadows = -1;
	private Resolution m_resolution;
	private Resolution[] m_resolutions;
	private Language[] m_languages;

	private OptionsMenuHandler() { }

	public static OptionsMenuHandler Instance { get; private set; }

	void Start() {
		Instance = this;

		m_extruders = new List<Extruder>();
		m_lights = new List<GameLight>();
		m_qualityLevel = QualitySettings.GetQualityLevel();
		m_fullscreen = Screen.fullScreen;
		m_framerateSlider.m_unlimited = -1;
		m_qualityDropdown.value = m_qualityLevel;

		PopulateResolutions(false);
		CheckForResolutionIssues();

		PopulateLanguages();
	}

	void OnEnable() {
		StartCoroutine(UpdateResolution());
		StartCoroutine(UpdateLanguage());
	}

	////////////////////////
	/*   Video Settings   */
	////////////////////////

	private void PopulateResolutions(bool p_forceCurrentRefreshRate) {
		m_resolutions = Screen.resolutions;
		List<Resolution> resolutions = new List<Resolution>(m_resolutions);
		List<Resolution> availableResolutions = new List<Resolution>();

		resolutions.Sort(delegate(Resolution p_x, Resolution p_y) {
			if(p_x.width > p_y.width) return 1;
			else if(p_x.width < p_y.width) return -1;
			else if(p_x.height > p_y.height) return 1;
			else if(p_x.height < p_y.height) return -1;

			return 0;
		});

		for(int i = 0; i < resolutions.Count; i++) {
			if(!p_forceCurrentRefreshRate && Screen.currentResolution.refreshRate != m_resolutions[i].refreshRate) continue;

			Resolution res = resolutions[i];

			if(p_forceCurrentRefreshRate) {
				res = new Resolution();

				res.width = resolutions[i].width;
				res.height = resolutions[i].height;
				res.refreshRate = Screen.currentResolution.refreshRate;
			}

			Dropdown.OptionData data = new Dropdown.OptionData(ResolutionToString(res));
			m_resolutionDropdown.options.Add(data);

			availableResolutions.Add(res);
			Debug.Log("Added res: " + ResolutionToString(res) + "@" + res.refreshRate + "hz");

			if(!Screen.fullScreen) { 
				if(Screen.width == res.width && Screen.height == res.height) {
					m_resolutionDropdown.value = i;
					m_resolution = res;
				}
			} else if(Screen.currentResolution.Equals(res)) {
				m_resolutionDropdown.value = i;
				m_resolution = res;
			}
		}

		if(availableResolutions.Count == 0 && !p_forceCurrentRefreshRate) {
			PopulateResolutions(true);
			return;
		}

		m_resolutions = new Resolution[availableResolutions.Count];

		for(int i = 0; i < availableResolutions.Count; i++) 
			m_resolutions[i] = availableResolutions[i];
	}

	private void CheckForResolutionIssues() {
		if(Screen.currentResolution.width <= 1 || Screen.width <= 1 ||
		   Screen.currentResolution.height <= 1 || Screen.height <= 1 ||
		   Screen.currentResolution.refreshRate == 0) {
			   Debug.Log("Resolution isn't properly set, fixing...");
			   ApplyResolution(m_resolution.width,
								m_resolution.height,
								FullScreenMode.Windowed,
								m_resolution.refreshRate);
			   StartCoroutine(SizeBackUp());
		   }
	}

	private IEnumerator SizeBackUp() {
		yield return new WaitForSecondsRealtime(0.5f);

		ApplyResolution();

		Debug.Log("Resolution fixed!");
	}

	private IEnumerator UpdateResolution() { 
		yield return new WaitForSecondsRealtime(1f);

		m_resolutionDropdown.ClearOptions();
		PopulateResolutions(false);
		CheckForResolutionIssues();
	}

	private IEnumerator UpdateLanguage() {
        yield return new WaitForSecondsRealtime(1f);

        m_languageDropdown.ClearOptions();
        PopulateLanguages();
	}

	private void PopulateLanguages() {
		m_languages = Game.m_languages.m_languages.ToArray();

		for(int i = 0; i < m_languages.Length; i++) {
            Dropdown.OptionData data = new Dropdown.OptionData(m_languages[i].m_name);
            m_languageDropdown.options.Add(data);

			if(m_languages[i].m_name == Game.m_languages.GetCurrentLanguage().m_name)
				m_languageDropdown.value = i;
		}
	}

	public void ApplyOptions() {
		SetResolution();
		ApplyVSync();
		ApplyFramerate();

		ApplyResolution();
		QualitySettings.SetQualityLevel(m_qualityLevel, true);

		if(m_originalShadows >= 2 && m_shadows < 2)
			ChangeLightModes(false);
		else if(m_originalShadows < 2 && m_shadows >= 2)
			ChangeLightModes(true);

		if(m_originalShadows != m_shadows) {
			m_originalShadows = m_shadows;
			ReExtrusion();
		}

		StartCoroutine(UpdateResolution());
	}

	public void SetResolution() { 
		m_resolution = m_resolutions[m_resolutionDropdown.value];
	}

	public void ApplyResolution() {
		if(m_resolution.width == m_resolutions[m_resolutions.Length - 1].width && !m_fullscreen)
			m_fullscreen = true;
		else if(m_resolution.width != m_resolutions[m_resolutions.Length - 1].width && m_fullscreen)
			m_fullscreen = false;

		ApplyResolution(m_resolution.width,
						   m_resolution.height,
						   m_fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed,
						   m_resolution.refreshRate);
	}

	public void ApplyResolution(int p_width, int p_height, FullScreenMode p_fullscreen, int p_refreshRate) {
		Screen.SetResolution(p_width,
								 p_height,
								 p_fullscreen,
								 p_refreshRate);
	}

	public void SetLanguage() {
		Game.m_options.Get("Language").Save(m_languages[m_languageDropdown.value].m_name);
		Game.m_languages.UpdateUILanguage();
	}

	public void SetVSync(bool p_vsync) { 
		m_vsync = p_vsync;
	}

	public void ApplyVSync() { 
		QualitySettings.vSyncCount = m_vsync ? 1 : 0;
	}

	public void SetFramerate(int p_framerate) {
		m_framerate = p_framerate;

		if(m_framerateSlider.m_value != p_framerate)
			m_framerateSlider.m_value = p_framerate;
	}

	public void ApplyFramerate() { 
		Application.targetFrameRate = m_framerate;
	}

	public string ResolutionToString(Resolution p_resolution) { 
		return p_resolution.width + "x" + p_resolution.height;
	}

	public void SetQualitySettings(int p_quality) { 
		m_qualityLevel = p_quality;
	}

	public void SetShadows(int p_shadows) { 
		m_shadows = p_shadows;

		if(m_originalShadows == -1) m_originalShadows = p_shadows;
	}

	public void AddGameLight(GameLight p_light) {
		m_lights.Add(p_light);
	}

	private void ChangeLightModes(bool p_quality) { 
		foreach(GameLight light in m_lights)
			light.SwitchQualityModes(p_quality);
	}

	public void AddExtruder(Extruder p_extruder) {
		m_extruders.Add(p_extruder);
	}

	private void ReExtrusion() {
		m_extruders.RemoveAll(e => !e);

		foreach(Extruder extruder in m_extruders) {
			if(extruder.m_isProjectileExtrusion || !extruder.gameObject.activeSelf) continue;

			if(extruder.m_extrusions.Count > 0) {
				foreach(GameObject ext in extruder.m_extrusions)
					Destroy(ext);

				extruder.m_extrusions.Clear();
			}

			extruder.Extrude();
		}
	}

	public void SetBrightness(int p_brightness) { 
		RenderSettings.ambientLight = new Color(p_brightness / 100f, p_brightness / 100f, p_brightness / 100f, 1);
	}

	////////////////////////
	/*   Audio Settings   */
	////////////////////////

	public void SetMasterVolume(int p_volume) { 
		AudioListener.volume = (float) p_volume / 100f;
	}

	/////////////////////////////
	/*   Gameplay Settings     */
	/////////////////////////////

	public void SetEnemyHealthBars(bool p_toggle) { 
		m_hpBarsCanvas.gameObject.SetActive(p_toggle);
	}

	////////////////////////////
	/*   Controls Settings    */
	////////////////////////////
}
