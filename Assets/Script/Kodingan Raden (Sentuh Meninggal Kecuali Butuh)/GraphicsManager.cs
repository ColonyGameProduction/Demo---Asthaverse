using UnityEngine;
using TMPro;

public class GraphicsManager : MonoBehaviour
{
    [Header("Resolution")]
    [SerializeField] private TextMeshProUGUI resolutionOption;

    [Header("Display Mode")]
    [SerializeField] private TextMeshProUGUI displayModeOption;

    [Header("Graphic Quality")]
    [SerializeField] private TextMeshProUGUI qualityOption;

    [Header("VSync")]
    [SerializeField] private TextMeshProUGUI VSyncOption;

    [Header("Max Framerate")]
    [SerializeField] private TextMeshProUGUI maxFrameOption;

    // Resolution
    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;

    // Display Mode
    private int currentDisplayModeIndex = 0;
    private readonly string[] displayModes = { "Fullscreen", "Windowed", "Borderless" };
    private readonly Resolution[] commonResolutions = new Resolution[] 
    {
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 1600, height = 900 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 2560, height = 1440 },
        new Resolution { width = 3840, height = 2160 }
    };

    //Graphic Quality
    private int currentQualityIndex = 0;
    private readonly string[] qualityLevels = { "Low", "Medium", "High", "Ultra" };

    // VSync
    private bool isVSyncOn = true;

    // Max Framerate
    private int[] framerateOptions = { 30, 60, 120, 144, 165, 240, -1 };
    private int currentFramerateIndex = 6;

    private void Start()
    {
        resolutions = System.Array.FindAll(commonResolutions, r => r.width <= Screen.currentResolution.width && r.height <= Screen.currentResolution.height);
        UpdateResolutionText();
        UpdateDisplayModeText();
        UpdateQualityText();
        UpdateVSyncText();
    }

    public void ChangeResolution(int change)
    {
        currentResolutionIndex = (currentResolutionIndex + change + resolutions.Length) % resolutions.Length;
        Resolution res = resolutions[currentResolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);

        UpdateResolutionText();
    }

    public void ChangeDisplayMode(int change)
    {
        currentDisplayModeIndex = (currentDisplayModeIndex + change + displayModes.Length) % displayModes.Length;
        Screen.fullScreenMode = (FullScreenMode)currentDisplayModeIndex;

        UpdateDisplayModeText();
    }

    public void ChangeQuality(int change)
    {
        currentQualityIndex = (currentQualityIndex + change + qualityLevels.Length) % qualityLevels.Length;
        QualitySettings.SetQualityLevel(currentQualityIndex);

        UpdateQualityText();
    }

    public void ToggleVSync()
    {
        isVSyncOn = !isVSyncOn;
        QualitySettings.vSyncCount = isVSyncOn ? 1 : 0;
        UpdateVSyncText();
    }

    public void ChangeMaxFramerate(int change)
    {
        currentFramerateIndex = (currentFramerateIndex + change + framerateOptions.Length) % framerateOptions.Length;
        int framerate = framerateOptions[currentFramerateIndex];
        Application.targetFrameRate = framerate;

        UpdateMaxFramerateText();
    }

    private void UpdateResolutionText()
    {
        Resolution res = resolutions[currentResolutionIndex];
        resolutionOption.text = $"{res.width} x {res.height}";
    }

    private void UpdateDisplayModeText()
    {
        displayModeOption.text = displayModes[currentDisplayModeIndex];
    }

    private void UpdateQualityText()
    {
        qualityOption.text = qualityLevels[currentQualityIndex];
    }

    private void UpdateVSyncText()
    {
        VSyncOption.text = isVSyncOn ? "On" : "Off";
    }

    private void UpdateMaxFramerateText()
    {
        int framerate = framerateOptions[currentFramerateIndex];
        maxFrameOption.text = framerate == -1 ? "Unlimited" : framerate.ToString();
    }
}
