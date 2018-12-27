using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OptionsHandler : MonoBehaviour {

	[Tooltip("The dropdown handling the resolutions")]
	public Dropdown m_resolutionDropdown;

	[Tooltip("The fullscreen toggle")]
	public Toggle m_fullscreenToggle;

	[Tooltip("Whether or not the game is in fullscreen mode")]
	public bool m_fullscreen;

	[Tooltip("The slider handling the framerate")]
	public AdaptativeSliderText m_refreshRateSlider;

	public Toggle m_enemyHealthBars;
	public Dropdown m_qualityDropdown;

	private int m_framerate;
	private int m_qualityLevel;
	private Resolution m_resolution;
	private Resolution[] m_resolutions;

	void Start() {
		QualitySettings.vSyncCount = 0;
		m_qualityLevel = QualitySettings.GetQualityLevel();
		//m_framerate = Application.targetFrameRate;

		m_fullscreenToggle.isOn = Screen.fullScreen;
		m_refreshRateSlider.m_unlimited = -1;
		m_refreshRateSlider.m_value = Screen.currentResolution.refreshRate;
		//m_enemyHealthBars.isOn = true;
		m_qualityDropdown.value = m_qualityLevel;

		PopulateResolutions();
	}

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
					Debug.Log("wh " + Screen.width + ", " + Screen.height + " ir " + ResolutionToString(m_resolutions[i]));
					m_resolutionDropdown.value = i;
					m_resolution = m_resolutions[i];
				}
			} else if(Screen.currentResolution.Equals(m_resolutions[i])) {
				Debug.Log("cr " + ResolutionToString(Screen.currentResolution) + " ir " + ResolutionToString(m_resolutions[i]));
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
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = m_framerate;

		ApplyResolution();
		QualitySettings.SetQualityLevel(m_qualityLevel, true);
		
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

	public void SetFramerate(float p_framerate) {
		m_framerate = (int) p_framerate;
	}

	public void ApplyFramerate() { 
		Application.targetFrameRate = m_framerate;
	}

	public string ResolutionToString(Resolution p_resolution) { 
		return p_resolution.width + "x" + p_resolution.height + "@" + p_resolution.refreshRate + "hz";
	}

	public void SetQualitySettings(int p_quality) { 
		m_qualityLevel = p_quality;
	}

	public void SetEnemyHealthBars(bool p_toggle) { 
		int layerBit = 1 << LayerMask.NameToLayer("Health Bars");
		bool currentlySet = (Camera.main.cullingMask & layerBit) != 0;

		if(currentlySet != p_toggle) Camera.main.cullingMask ^= layerBit;
	}
}
