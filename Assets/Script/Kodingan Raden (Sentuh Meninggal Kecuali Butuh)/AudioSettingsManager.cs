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

    [Header("Dialogue")]
    public TextMeshProUGUI dialogueVolumeText;
    public Slider dialogueSlider;
    public Button dialogueLeftButton;  // Button to decrease volume
    public Button dialogueRightButton; // Button to increase volume

    // Volume Settings
    private float stepSize = 0.05f; // How much the volume changes per button press

    private float masterCurrentVolume = 0.5f;
    private float musicCurrentVolume = 0.5f;
    private float soundEffectCurrentVolume = 0.5f;
    private float dialogueCurrentVolume = 0.5f;

    private void Start()
    {
        // Initialize slider value
        masterSlider.value = masterCurrentVolume;
        UpdateMasterVolume(masterCurrentVolume);

        musicSlider.value = musicCurrentVolume;
        UpdateMusicVolume(musicCurrentVolume);

        soundEffectSlider.value = soundEffectCurrentVolume;
        UpdateSoundEffectVolume(soundEffectCurrentVolume);

        dialogueSlider.value = dialogueCurrentVolume;
        UpdateDialogueVolume(dialogueCurrentVolume);

        // Add listeners to buttons
        masterLeftButton.onClick.AddListener(() => ChangeMasterVolume(-stepSize)); // Decrease volume
        masterRightButton.onClick.AddListener(() => ChangeMasterVolume(stepSize)); // Increase volume

        musicLeftButton.onClick.AddListener(() => ChangeMusicVolume(-stepSize)); // Decrease volume
        musicRightButton.onClick.AddListener(() => ChangeMusicVolume(stepSize)); // Increase volume

        soundEffectLeftButton.onClick.AddListener(() => ChangeSoundEffectVolume(-stepSize)); // Decrease volume
        soundEffectRightButton.onClick.AddListener(() => ChangeSoundEffectVolume(stepSize)); // Increase volume

        dialogueLeftButton.onClick.AddListener(() => ChangeDialogueVolume(-stepSize)); // Decrease volume
        dialogueRightButton.onClick.AddListener(() => ChangeDialogueVolume(stepSize)); // Increase volume
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

    public void ChangeDialogueVolume(float delta)
    {
        // Adjust volume within 0-1 range
        dialogueCurrentVolume = Mathf.Clamp(dialogueCurrentVolume + delta, 0f, 1f);

        // Update slider and volume UI
        dialogueSlider.value = dialogueCurrentVolume;
        UpdateDialogueVolume(dialogueCurrentVolume);
    }

    public void UpdateDialogueVolume(float value)
    {
        // Update volume and UI text
        dialogueVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        AudioListener.volume = value; // Update system volume
    }
}