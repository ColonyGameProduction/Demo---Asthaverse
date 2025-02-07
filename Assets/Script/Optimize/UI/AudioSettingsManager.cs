using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class AudioSettingsManager : MonoBehaviour
{   
    private AudioManager _am;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("Output")]
    [SerializeField] private TextMeshProUGUI _outputOption;
    private readonly string[] _outputOptions = { "Default", "Headphones"};

    [Header("Master")]
    [SerializeField] private TextMeshProUGUI _masterVolumeText;
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Button _masterLeftButton;  // Button to decrease volume
    [SerializeField] private Button _masterRightButton; // Button to increase volume

    [Header("Music")]
    [SerializeField] private TextMeshProUGUI _musicVolumeText;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Button _musicLeftButton;  // Button to decrease volume
    [SerializeField] private Button _musicRightButton; // Button to increase volume

    [Header("Sound Effect")]
    [SerializeField] private TextMeshProUGUI _soundEffectVolumeText;
    [SerializeField] private Slider _soundEffectSlider;
    [SerializeField] private Button _soundEffectLeftButton;  // Button to decrease volume
    [SerializeField] private Button _soundEffectRightButton; // Button to increase volume

    [Header("Dialogue")]
    [SerializeField] private TextMeshProUGUI _dialogueVolumeText;
    [SerializeField] private Slider _dialogueSlider;
    [SerializeField] private Button _dialogueLeftButton;  // Button to decrease volume
    [SerializeField] private Button _dialogueRightButton; // Button to increase volume

    // Volume Settings
    [SerializeField] private float _stepSize = 0.05f; // How much the volume changes per button press

    [ReadOnly(false), SerializeField] private int _currentOutputIndex = 0;
    [ReadOnly(false), SerializeField] private float _masterCurrentVolume = 0.5f;
    private bool _isMasterVolChangeFromSlider = false;
    [ReadOnly(false), SerializeField] private float _musicCurrentVolume = 0.5f;
    private bool _isMusicVolChangeFromSlider = false;
    [ReadOnly(false), SerializeField] private float _soundEffectCurrentVolume = 0.5f;
    private bool _isSFXVolChangeFromSlider = false;
    [ReadOnly(false), SerializeField] private float _dialogueCurrentVolume = 0.5f;
    private bool _isDialogueVolChangeFromSlider = false;

    #region const
    public const string OUTPUT_OPTION_PREF = "OutputOption";
    public const string MASTER_VALUE_PREF = "MasterVolume";
    public const string MUSIC_VALUE_PREF = "MusicVolume";
    public const string SFX_VALUE_PREF = "SFXVolume";
    public const string DIALOGUE_VALUE_PREF = "DialogueVolume";

    public const string MIXER_MASTER = "Master";
    public const string MIXER_MUSIC = "Music";
    public const string MIXER_SFX = "SFX";
    public const string MIXER_DIALOGUE = "Dialogue";
    #endregion
    private void Start()
    {
        LoadPref();
        _am = AudioManager.Instance;

        // Add listeners to buttons
        _masterLeftButton.onClick.AddListener(() => ChangeMasterVolumeButton(-_stepSize)); // Decrease volume
        _masterRightButton.onClick.AddListener(() => ChangeMasterVolumeButton(_stepSize)); // Increase volume
        _masterSlider.onValueChanged.AddListener(ChangeMasterVolumeSlider);

        _musicLeftButton.onClick.AddListener(() => ChangeMusicVolumeButton(-_stepSize)); // Decrease volume
        _musicRightButton.onClick.AddListener(() => ChangeMusicVolumeButton(_stepSize)); // Increase volume
        _musicSlider.onValueChanged.AddListener(ChangeMusicVolumeSlider);

        _soundEffectLeftButton.onClick.AddListener(() => ChangeSoundEffectVolumeButton(-_stepSize)); // Decrease volume
        _soundEffectRightButton.onClick.AddListener(() => ChangeSoundEffectVolumeButton(_stepSize)); // Increase volume
        _soundEffectSlider.onValueChanged.AddListener(ChangeSoundEffectVolumeSlider);

        _dialogueLeftButton.onClick.AddListener(() => ChangeDialogueVolumeButton(-_stepSize)); // Decrease volume
        _dialogueRightButton.onClick.AddListener(() => ChangeDialogueVolumeButton(_stepSize)); // Increase volume
        _dialogueSlider.onValueChanged.AddListener(ChangeDialogueVolumeSlider);
    }

    private void LoadPref()
    {
        _currentOutputIndex = PlayerPrefs.GetInt(OUTPUT_OPTION_PREF, _currentOutputIndex);
        _masterCurrentVolume = PlayerPrefs.GetFloat(MASTER_VALUE_PREF, _masterCurrentVolume);
        _musicCurrentVolume = PlayerPrefs.GetFloat(MUSIC_VALUE_PREF, _musicCurrentVolume);
        _soundEffectCurrentVolume = PlayerPrefs.GetFloat(SFX_VALUE_PREF, _soundEffectCurrentVolume);
        _dialogueCurrentVolume = PlayerPrefs.GetFloat(DIALOGUE_VALUE_PREF, _dialogueCurrentVolume);

        UpdateOutputText();
        UpdateMasterVolume(_masterCurrentVolume);
        UpdateMusicVolume(_musicCurrentVolume);
        UpdateSoundEffectVolume(_soundEffectCurrentVolume);
        UpdateDialogueVolume(_dialogueCurrentVolume);

        _isMasterVolChangeFromSlider = true;
        _isMusicVolChangeFromSlider = true;
        _isSFXVolChangeFromSlider = true;
        _isDialogueVolChangeFromSlider = true;
    }

    public void ChangeOutput(int change)
    {
        _currentOutputIndex = (_currentOutputIndex + change + _outputOptions.Length) % _outputOptions.Length;
        //What to do in unity settings

        PlayerPrefs.SetInt(OUTPUT_OPTION_PREF, _currentOutputIndex);
        UpdateOutputText();
    }

    private void UpdateOutputText()
    {
        _outputOption.text = _outputOptions[_currentOutputIndex];
    }

    public void ChangeMasterVolumeButton(float delta)
    {
        // Adjust volume within 0-1 range
        _masterCurrentVolume = Mathf.Clamp(_masterCurrentVolume + delta, 0f, 1f);

        _isMasterVolChangeFromSlider = false;
        UpdateMasterVolume(_masterCurrentVolume);

        PlayerPrefs.SetFloat(MASTER_VALUE_PREF, _masterCurrentVolume);
    }

    public void ChangeMasterVolumeSlider(float value)
    {
        if(_isMasterVolChangeFromSlider) UpdateMasterVolume(value);
        else _isMasterVolChangeFromSlider = true;

        
        PlayerPrefs.SetFloat(MASTER_VALUE_PREF, value);
    }

    public void UpdateMasterVolume(float value)
    {
        if(_isMasterVolChangeFromSlider) _masterCurrentVolume = value;
        else _masterSlider.value = value;

        // Update volume and UI text
        if(_am != null) _am.PlayUIClick();
        _masterVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        SetAudioMixerFloat(MIXER_MASTER, _masterCurrentVolume);
    }

    public void ChangeMusicVolumeButton(float delta)
    {
        // Adjust volume within 0-1 range
        _musicCurrentVolume = Mathf.Clamp(_musicCurrentVolume + delta, 0f, 1f);

        _isMusicVolChangeFromSlider = false;
        UpdateMusicVolume(_musicCurrentVolume);

        PlayerPrefs.SetFloat(MUSIC_VALUE_PREF, _musicCurrentVolume);
    }

    public void ChangeMusicVolumeSlider(float value)
    {
        if(_isMusicVolChangeFromSlider) UpdateMusicVolume(value);
        else _isMusicVolChangeFromSlider = true;

        
        PlayerPrefs.SetFloat(MUSIC_VALUE_PREF, value);
    }

    public void UpdateMusicVolume(float value)
    {
        if(_isMusicVolChangeFromSlider) _musicCurrentVolume = value;
        else _musicSlider.value = value;

        // Update volume and UI text
        if(_am != null) _am.PlayUIClick();
        _musicVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        SetAudioMixerFloat(MIXER_MUSIC, _musicCurrentVolume);
    }

    public void ChangeSoundEffectVolumeButton(float delta)
    {
        // Adjust volume within 0-1 range
        _soundEffectCurrentVolume = Mathf.Clamp(_soundEffectCurrentVolume + delta, 0f, 1f);

        _isSFXVolChangeFromSlider = false;
        UpdateSoundEffectVolume(_soundEffectCurrentVolume);

        PlayerPrefs.SetFloat(SFX_VALUE_PREF, _soundEffectCurrentVolume);
    }

    public void ChangeSoundEffectVolumeSlider(float value)
    {
        if(_isSFXVolChangeFromSlider) UpdateSoundEffectVolume(value);
        else _isSFXVolChangeFromSlider = true;

        
        PlayerPrefs.SetFloat(SFX_VALUE_PREF, value);
    }

    public void UpdateSoundEffectVolume(float value)
    {
        if(_isSFXVolChangeFromSlider) _soundEffectCurrentVolume = value;
        else _soundEffectSlider.value = value;

        // Update volume and UI text
        if(_am != null) _am.PlayUIClick();
        _soundEffectVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        SetAudioMixerFloat(MIXER_SFX, _soundEffectCurrentVolume);
    }

    public void ChangeDialogueVolumeButton(float delta)
    {
        // Adjust volume within 0-1 range
        _dialogueCurrentVolume = Mathf.Clamp(_dialogueCurrentVolume + delta, 0f, 1f);

        _isDialogueVolChangeFromSlider = false;
        UpdateDialogueVolume(_dialogueCurrentVolume);

        PlayerPrefs.SetFloat(DIALOGUE_VALUE_PREF, _dialogueCurrentVolume);
    }

    public void ChangeDialogueVolumeSlider(float value)
    {
        if(_isDialogueVolChangeFromSlider) UpdateDialogueVolume(value);
        else _isDialogueVolChangeFromSlider = true;

        
        PlayerPrefs.SetFloat(DIALOGUE_VALUE_PREF, value);
    }

    public void UpdateDialogueVolume(float value)
    {
        if(_isDialogueVolChangeFromSlider) _dialogueCurrentVolume = value;
        else _dialogueSlider.value = value;

        // Update volume and UI text
        if(_am != null) _am.PlayUIClick();
        _dialogueVolumeText.text = Mathf.RoundToInt(value * 100) + "%";
        SetAudioMixerFloat(MIXER_DIALOGUE, _dialogueCurrentVolume);
    }

    private void SetAudioMixerFloat(string parameter, float vol)
    {
        float currVol = Mathf.Clamp(vol, 0.0001f, 1f);

        if(parameter == MIXER_DIALOGUE)
        {
            _audioMixer.SetFloat(parameter, Mathf.Log10(currVol) * 20 + 1);
        }
        else if(parameter == MIXER_MUSIC)
        {
            _audioMixer.SetFloat(parameter, Mathf.Log10(currVol) * 20 - 8);
        }
        else
        {
            _audioMixer.SetFloat(parameter, Mathf.Log10(currVol) * 20);
        }
        
    }
}