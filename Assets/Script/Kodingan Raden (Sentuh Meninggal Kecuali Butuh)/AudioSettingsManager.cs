using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Master")]
    public TextMeshProUGUI masterVolumeText;
    public Slider masterSlider;

    // Master Volume
    public float masterCurrentVolume = 0.5f;

    private float maxValue = 0.0f;
    private float minValue = 1.0f;

    private void Start()
    {
        masterCurrentVolume = 0.5f;
        UpdateMasterVolume(masterCurrentVolume);
    }

    public void ChangeMasterVolume(float delta)
    {
        masterCurrentVolume = Mathf.Clamp(masterCurrentVolume + delta, minValue, maxValue);
        UpdateMasterVolume(masterCurrentVolume);
    }

    public void UpdateMasterVolume(float value)
    {
        masterVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        masterSlider.value = value;
    }
}
