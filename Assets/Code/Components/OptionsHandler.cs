using UnityEngine;
using UnityEngine.UI;

public class OptionsHandler : MonoBehaviour {

	[Tooltip("The dropdown handling the resolutions")]
	public Dropdown m_resolutionDropdown;

	// load these from db one day?
	[Tooltip("The fullscreen toggle")]
	public Toggle m_fullscreenToggle;

	[Tooltip("Whether or not the game is in fullscreen mode")]
	public bool m_fullscreen;

	[Tooltip("The slider handling the framerate")]
	public AdaptativeSliderText m_refreshRateSlider;

	[Tooltip("The game's frame rate")]
	[Range(1, 240)] public int m_framerate;

	public Toggle m_enemyHealthBars;
	public Dropdown m_qualityDropdown;

	private int m_qualityLevel;
	private Resolution m_resolution;
	private Resolution[] m_resolutions;

	void Start() {
		m_qualityLevel = QualitySettings.GetQualityLevel();
		m_framerate = Application.targetFrameRate;

		m_fullscreenToggle.isOn = Screen.fullScreen;
		m_refreshRateSlider.m_unlimited = -1;
		m_refreshRateSlider.m_value = m_framerate;
		m_enemyHealthBars.isOn = Camera.main.cullingMask == (Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Health Bars")));
		m_qualityDropdown.value = m_qualityLevel;

		PopulateResolutions();
	}

	private void PopulateResolutions() {
		m_resolutions = Screen.resolutions;

		for(int i = 0; i < m_resolutions.Length; i++) {
			Dropdown.OptionData data = new Dropdown.OptionData(ResolutionToString(m_resolutions[i]));
			m_resolutionDropdown.options.Add(data);

			if(Screen.currentResolution.Equals(m_resolutions[i])) {
				m_resolutionDropdown.value = i;
				m_resolution = m_resolutions[i];
			}
		}
	}

	public void ApplyOptions() {
		SetResolution();
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = m_framerate;
		ApplyResolution();
		QualitySettings.SetQualityLevel(m_qualityLevel, true);
	}

	public void SetResolution() { 
		m_resolution = m_resolutions[m_resolutionDropdown.value];
	}

	public void ApplyResolution() {
		if(m_resolution.width == m_resolutions[m_resolutions.Length - 1].width && !m_fullscreen) {
			SetFullscreen(true);
			m_fullscreenToggle.isOn = true; // TODO: check if this updates the actual value
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

	public string ResolutionToString(Resolution p_resolution) { 
		return p_resolution.width + "x" + p_resolution.height + "@" + p_resolution.refreshRate + "hz";
	}

	public void SetQualitySettings(int p_quality) { 
		m_qualityLevel = p_quality;
	}

	public void ToggleEnemyHealthBars(bool p_toggle) { 
		Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("Health Bars");
	}
}
