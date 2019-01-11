using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OptionsMenuHandler : MonoBehaviour {

	[Tooltip("The dropdown handling the resolutions")]
	public Dropdown m_resolutionDropdown;

	[Tooltip("The fullscreen toggle")]
	public Toggle m_fullscreenToggle;

	[Tooltip("The toggle for vsync")]
	public Toggle m_vsyncToggle;

	[Tooltip("The slider handling the framerate")]
	public AdaptativeSliderText m_framerateSlider;

	[Tooltip("The dropdown handling the quality settings")]
	public Dropdown m_qualityDropdown;

	[Tooltip("The canvas containing all the health bars")]
	public Canvas m_hpBarsCanvas;

	[HideInInspector] public List<Extruder> m_extruders;

	private int m_framerate;
	private bool m_fullscreen;
	private bool m_vsync;
	private int m_qualityLevel;
	private int m_shadows;
	private int m_originalShadows = -1;
	private Resolution m_resolution;
	private Resolution[] m_resolutions;

	private OptionsMenuHandler() { }

	public static OptionsMenuHandler Instance { get; private set; }

	void Start() {
		Instance = this;

		m_extruders = new List<Extruder>();
		m_qualityLevel = QualitySettings.GetQualityLevel();

		m_fullscreenToggle.isOn = Screen.fullScreen;
		m_vsyncToggle.isOn = QualitySettings.vSyncCount > 0;
		m_framerateSlider.m_unlimited = -1;
		m_qualityDropdown.value = m_qualityLevel;

		PopulateResolutions();
	}

	////////////////////////
	/*  Video Settings  */
	////////////////////////

	private void PopulateResolutions() {
		m_resolutions = Screen.resolutions;
		List<Resolution> availableResolutions = new List<Resolution>();

		for(int i = 0; i < m_resolutions.Length; i++) {
			if(Screen.currentResolution.refreshRate != m_resolutions[i].refreshRate) continue;

			Dropdown.OptionData data = new Dropdown.OptionData(ResolutionToString(m_resolutions[i]));
			m_resolutionDropdown.options.Add(data);

			availableResolutions.Add(m_resolutions[i]);

			if(!Screen.fullScreen) { 
				if(Screen.width == m_resolutions[i].width && Screen.height == m_resolutions[i].height) {
					m_resolutionDropdown.value = i;
					m_resolution = m_resolutions[i];
				}
			} else if(Screen.currentResolution.Equals(m_resolutions[i])) {
				m_resolutionDropdown.value = i;
				m_resolution = m_resolutions[i];
			}
		}

		m_resolutions = new Resolution[availableResolutions.Count];

		for(int i = 0; i < availableResolutions.Count; i++) 
			m_resolutions[i] = availableResolutions[i];
	}

	private IEnumerator UpdateResolution() { 
		yield return new WaitForSeconds(1f);

		m_fullscreenToggle.isOn = Screen.fullScreen;
		m_resolutionDropdown.ClearOptions();
		PopulateResolutions();
	}

	public void ApplyOptions() {
		SetResolution();
		ApplyVSync();
		ApplyFramerate();

		ApplyResolution();
		QualitySettings.SetQualityLevel(m_qualityLevel, true);

		if((m_originalShadows >= 1 && m_shadows == 0) || 
			(m_originalShadows == 0 && m_shadows >= 1)) {
			m_originalShadows = m_shadows;
			ReExtrusion();
		}

		StartCoroutine(UpdateResolution());
	}

	public void SetResolution() { 
		m_resolution = m_resolutions[m_resolutionDropdown.value];
	}

	public void ApplyResolution() {
		if(m_resolution.width == m_resolutions[m_resolutions.Length - 1].width && !m_fullscreen) {
			SetFullscreen(true);
			m_fullscreenToggle.isOn = true; // TODO: check if this updates the actual value
		} else if(m_resolution.width != m_resolutions[m_resolutions.Length - 1].width && m_fullscreen) { 
			SetFullscreen(false);
			m_fullscreenToggle.isOn = false;
		}

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

	public void SetFullscreen(bool p_fullscreen) { 
		m_fullscreen = p_fullscreen;
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

	////////////////////////
	/*  Audio Settings  */
	////////////////////////

	public void SetMasterVolume(int p_volume) { 
		AudioListener.volume = (float) p_volume / 100f;
	}

	/////////////////////////////
	/*  Gameplay Settings  */
	////////////////////////////

	public void SetEnemyHealthBars(bool p_toggle) { 
		m_hpBarsCanvas.gameObject.SetActive(p_toggle);
	}

	////////////////////////////////
	/*  Key Bindings Settings  */
	////////////////////////////////
}
