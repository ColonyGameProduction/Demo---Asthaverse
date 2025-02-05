using UnityEngine;
using TMPro;

public class GraphicsManager : MonoBehaviour
{
    [Header("Resolution")]
    [SerializeField] private TextMeshProUGUI _resolutionOption;

    [Header("Display Mode")]
    [SerializeField] private TextMeshProUGUI _displayModeOption;

    [Header("Graphic Quality")]
    [SerializeField] private TextMeshProUGUI _qualityOption;

    [Header("VSync")]
    [SerializeField] private TextMeshProUGUI _VSyncOption;

    [Header("Max Framerate")]
    [SerializeField] private TextMeshProUGUI _maxFrameOption;

    // Resolution
    private Resolution[] _resolutions;
    [ReadOnly(false), SerializeField] private int _currentResolutionIndex = 0;

    // Display Mode
    [ReadOnly(false), SerializeField] private int _currentDisplayModeIndex = 0;
    private readonly string[] _displayModes = { "Fullscreen", "Windowed", "Borderless" };
    private readonly Resolution[] _commonResolutions = new Resolution[] 
    {
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 1600, height = 900 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 2560, height = 1440 },
        new Resolution { width = 3840, height = 2160 }
    };

    //Graphic Quality
    [ReadOnly(false), SerializeField] private int _currentQualityIndex = 0;
    private readonly string[] _qualityLevels = { "Low", "Medium", "High", "Ultra" };

    // VSync
    [ReadOnly(false), SerializeField] private bool _isVSyncOn = false;

    // Max Framerate
    private int[] _framerateOptions = { 30, 60, 120, 144, 165, 240, -1 };
    [ReadOnly(false), SerializeField] private int _currentFramerateIndex = 6;

    #region const
    public const string RESOLUTION_OPTION_PREF = "ResolutionOption";
    public const string DISPLAYMODE_OPTION_PREF = "DisplayModeOption";
    public const string QUALITY_OPTION_PREF = "QualityOption";
    public const string VSYNC_OPTION_PREF = "VsyncOption";
    public const string MAXFRAME_OPTION_PREF = "MaxFrameOption";
    #endregion
    private void Start()
    {
        _resolutions = System.Array.FindAll(_commonResolutions, r => r.width <= Screen.currentResolution.width && r.height <= Screen.currentResolution.height);

        LoadPref();
    }

    private void LoadPref()
    {
        _currentResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_OPTION_PREF, _currentResolutionIndex);
        _currentDisplayModeIndex = PlayerPrefs.GetInt(DISPLAYMODE_OPTION_PREF, _currentDisplayModeIndex);
        _currentQualityIndex = PlayerPrefs.GetInt(QUALITY_OPTION_PREF, _currentQualityIndex);
        _isVSyncOn = PlayerPrefs.GetInt(VSYNC_OPTION_PREF, _isVSyncOn ? 1 : 0) == 1 ? true : false;
        _currentFramerateIndex = PlayerPrefs.GetInt(MAXFRAME_OPTION_PREF, _currentFramerateIndex);

        UpdateResolutionText();
        UpdateDisplayModeText();
        UpdateQualityText();
        UpdateVSyncText();
        UpdateMaxFramerateText();
    }

    public void ChangeResolution(int change)
    {
        _currentResolutionIndex = (_currentResolutionIndex + change + _resolutions.Length) % _resolutions.Length;
        Resolution res = _resolutions[_currentResolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);

        PlayerPrefs.SetInt(RESOLUTION_OPTION_PREF, _currentResolutionIndex);
        UpdateResolutionText();
    }

    public void ChangeDisplayMode(int change)
    {
        _currentDisplayModeIndex = (_currentDisplayModeIndex + change + _displayModes.Length) % _displayModes.Length;
        Screen.fullScreenMode = (FullScreenMode)_currentDisplayModeIndex;

        PlayerPrefs.SetInt(DISPLAYMODE_OPTION_PREF, _currentDisplayModeIndex);
        UpdateDisplayModeText();
    }

    public void ChangeQuality(int change)
    {
        _currentQualityIndex = (_currentQualityIndex + change + _qualityLevels.Length) % _qualityLevels.Length;
        QualitySettings.SetQualityLevel(_currentQualityIndex);

        PlayerPrefs.SetInt(QUALITY_OPTION_PREF, _currentQualityIndex);
        UpdateQualityText();
    }

    public void ToggleVSync()
    {
        _isVSyncOn = !_isVSyncOn;
        QualitySettings.vSyncCount = _isVSyncOn ? 1 : 0;

        PlayerPrefs.SetInt(VSYNC_OPTION_PREF, _isVSyncOn ? 1 : 0);
        UpdateVSyncText();
    }

    public void ChangeMaxFramerate(int change)
    {
        _currentFramerateIndex = (_currentFramerateIndex + change + _framerateOptions.Length) % _framerateOptions.Length;
        int framerate = _framerateOptions[_currentFramerateIndex];
        Application.targetFrameRate = framerate;

        PlayerPrefs.SetInt(MAXFRAME_OPTION_PREF, _currentFramerateIndex);
        UpdateMaxFramerateText();
    }

    private void UpdateResolutionText()
    {
        Resolution res = _resolutions[_currentResolutionIndex];
        _resolutionOption.text = $"{res.width} x {res.height}";
    }

    private void UpdateDisplayModeText()
    {
        _displayModeOption.text = _displayModes[_currentDisplayModeIndex];
    }

    private void UpdateQualityText()
    {
        _qualityOption.text = _qualityLevels[_currentQualityIndex];
    }

    private void UpdateVSyncText()
    {
        _VSyncOption.text = _isVSyncOn ? "On" : "Off";
    }

    private void UpdateMaxFramerateText()
    {
        int framerate = _framerateOptions[_currentFramerateIndex];
        _maxFrameOption.text = framerate == -1 ? "Unlimited" : framerate.ToString();
    }
}
