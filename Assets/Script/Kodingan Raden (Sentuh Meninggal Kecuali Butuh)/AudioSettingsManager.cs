using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Master")]
    public TextMeshProUGUI masterVolumeText;
    public Slider masterSlider;
    public Button masterLeftButton;  // Button to decrease volume
    public Button masterRightButton; // Button to increase volume

    [Header("Music")]
    public TextMeshProUGUI musicVolumeText;
    public Slider musicSlider;
    public Button musicLeftButton;  // Button to decrease volume
    public Button musicRightButton; // Button to increase volume

    [Header("Sound Effect")]
    public TextMeshProUGUI soundEffectVolumeText;
    public Slider soundEffectSlider;
    public Button soundEffectLeftButton;  // Button to decrease volume
    public Button soundEffectRightButton; // Button to increase volume

    [Header("Subtitle")]
    public TextMeshProUGUI subtitleVolumeText;
    public Slider subtitleSlider;
    public Button subtitleLeftButton;  // Button to decrease volume
    public Button subtitleRightButton; // Button to increase volume

    // Volume Settings
    private float stepSize = 0.05f; // How much the volume changes per button press

    private float masterCurrentVolume = 0.5f;
    private float musicCurrentVolume = 0.5f;
    private float soundEffectCurrentVolume = 0.5f;
    private float subtitleCurrentVolume = 0.5f;

    private void Start()
    {
        // Initialize slider value
        masterSlider.value = masterCurrentVolume;
        UpdateMasterVolume(masterCurrentVolume);

        musicSlider.value = musicCurrentVolume;
        UpdateMusicVolume(musicCurrentVolume);

        soundEffectSlider.value = soundEffectCurrentVolume;
        UpdateSoundEffectVolume(soundEffectCurrentVolume);

        subtitleSlider.value = subtitleCurrentVolume;
        UpdateSubtitleVolume(subtitleCurrentVolume);

        // Add listeners to buttons
        masterLeftButton.onClick.AddListener(() => ChangeMasterVolume(-stepSize)); // Decrease volume
        masterRightButton.onClick.AddListener(() => ChangeMasterVolume(stepSize)); // Increase volume

        musicLeftButton.onClick.AddListener(() => ChangeMusicVolume(-stepSize)); // Decrease volume
        musicRightButton.onClick.AddListener(() => ChangeMusicVolume(stepSize)); // Increase volume

        soundEffectLeftButton.onClick.AddListener(() => ChangeSoundEffectVolume(-stepSize)); // Decrease volume
        soundEffectRightButton.onClick.AddListener(() => ChangeSoundEffectVolume(stepSize)); // Increase volume

        subtitleLeftButton.onClick.AddListener(() => ChangeSubtitleVolume(-stepSize)); // Decrease volume
        subtitleRightButton.onClick.AddListener(() => ChangeSubtitleVolume(stepSize)); // Increase volume
    }

    public void ChangeMasterVolume(float delta)
    {
        // Adjust volume within 0-1 range
        masterCurrentVolume = Mathf.Clamp(masterCurrentVolume + delta, 0f, 1f);

        // Update slider and volume UI
        masterSlider.value = masterCurrentVolume;
        UpdateMasterVolume(masterCurrentVolume);
    }

    public void UpdateMasterVolume(float value)
    {
        // Update volume and UI text
        masterVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        AudioListener.volume = value; // Update system volume
    }

    public void ChangeMusicVolume(float delta)
    {
        // Adjust volume within 0-1 range
        musicCurrentVolume = Mathf.Clamp(musicCurrentVolume + delta, 0f, 1f);

        // Update slider and volume UI
        musicSlider.value = musicCurrentVolume;
        UpdateMusicVolume(musicCurrentVolume);
    }

    public void UpdateMusicVolume(float value)
    {
        // Update volume and UI text
        musicVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        AudioListener.volume = value; // Update system volume
    }

    public void ChangeSoundEffectVolume(float delta)
    {
        // Adjust volume within 0-1 range
        soundEffectCurrentVolume = Mathf.Clamp(soundEffectCurrentVolume + delta, 0f, 1f);

        // Update slider and volume UI
        soundEffectSlider.value = soundEffectCurrentVolume;
        UpdateSoundEffectVolume(soundEffectCurrentVolume);
    }

    public void UpdateSoundEffectVolume(float value)
    {
        // Update volume and UI text
        soundEffectVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        AudioListener.volume = value; // Update system volume
    }

    public void ChangeSubtitleVolume(float delta)
    {
        // Adjust volume within 0-1 range
        subtitleCurrentVolume = Mathf.Clamp(subtitleCurrentVolume + delta, 0f, 1f);

        // Update slider and volume UI
        subtitleSlider.value = subtitleCurrentVolume;
        UpdateSubtitleVolume(subtitleCurrentVolume);
    }

    public void UpdateSubtitleVolume(float value)
    {
        // Update volume and UI text
        subtitleVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        AudioListener.volume = value; // Update system volume
    }
}