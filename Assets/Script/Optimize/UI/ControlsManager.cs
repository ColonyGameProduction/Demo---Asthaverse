using System;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class ControlsManager : MonoBehaviour
{
    [Header("Sensitivity")]
    [SerializeField] private TextMeshProUGUI _sensPresentageText;
    [SerializeField] private Slider _sensSlider;
    [SerializeField] private Button _sensLeftButton;
    [SerializeField] private Button _sensRightButton;

    [Header("Aim")]
    [SerializeField] private TextMeshProUGUI _aimOption;

    [Header("Sprint")]
    [SerializeField] private TextMeshProUGUI _sprintOption;

    [Header("Crouch")]
    [SerializeField] private TextMeshProUGUI _crouchOption;

    [SerializeField] private float _stepSize = 0.01f;

    [ReadOnly(false), SerializeField] private float _sensCurrentPresentage = 0.5f;
    private bool _isSensChangeFromSlider = false;

    //Aim
    [ReadOnly(false), SerializeField] private int _currentAimIndex = 0;
    [ReadOnly(false), SerializeField] private readonly string[] _aimOptions = { "Hold", "Toggle"};

    //Sprint
    [ReadOnly(false), SerializeField] private int _currentSprintIndex = 0;
    [ReadOnly(false), SerializeField] private readonly string[] _sprintOptions = { "Hold", "Toggle" };

    //Crouch
    [ReadOnly(false), SerializeField] private int _currentCrouchIndex = 0;
    [ReadOnly(false), SerializeField] private readonly string[] _crouchOptions = { "Hold", "Toggle" };

    #region event
    public Action<float> OnSensValueChange;
    public Action<bool> OnAimModeChange, OnSprintModeChange, OnCrouchModeChange;
    #endregion

    #region const
    public const string SENS_VALUE_PREF = "SensValue";
    public const string AIM_MODE_PREF = "AimMode";
    public const string SPRINT_MODE_PREF = "SprintMode";
    public const string CROUCH_MODE_PREF = "CrouchMode";
    #endregion

    private void Start()
    {
        LoadPref();
        

        _sensLeftButton.onClick.AddListener(() => ChangeSensPresentageButton(-_stepSize));
        _sensRightButton.onClick.AddListener(() => ChangeSensPresentageButton(_stepSize));
        _sensSlider.onValueChanged.AddListener(ChangeSensPresentageSlider);
    }

    private void LoadPref()
    {
        _sensCurrentPresentage = PlayerPrefs.GetFloat(SENS_VALUE_PREF, _sensCurrentPresentage);
        _currentAimIndex = PlayerPrefs.GetInt(AIM_MODE_PREF, _currentAimIndex);
        _currentSprintIndex = PlayerPrefs.GetInt(SPRINT_MODE_PREF, _currentSprintIndex);
        _currentCrouchIndex = PlayerPrefs.GetInt(CROUCH_MODE_PREF, _currentCrouchIndex);

        UpdateSensPresentage(_sensCurrentPresentage);
        UpdateAimText();
        UpdateSprintText();
        UpdateCrouchText();
    }

    public void ChangeSensPresentageButton(float delta)
    {
        _sensCurrentPresentage = Mathf.Clamp(_sensCurrentPresentage + delta, 0f, 1f);

        _isSensChangeFromSlider = false;
        UpdateSensPresentage(_sensCurrentPresentage);

        PlayerPrefs.SetFloat(SENS_VALUE_PREF, _sensCurrentPresentage);
    }
    public void ChangeSensPresentageSlider(float value)
    {
        if(_isSensChangeFromSlider)UpdateSensPresentage(value);
        else _isSensChangeFromSlider = true;

        PlayerPrefs.SetFloat(SENS_VALUE_PREF, value);
    }

    private void UpdateSensPresentage(float value)
    {
        if(_isSensChangeFromSlider) _sensCurrentPresentage = value;
        else _sensSlider.value = value;
        
        _sensPresentageText.text = Mathf.RoundToInt(value * 100) + "%";
        
        // value change
        // ------------
        
        OnSensValueChange?.Invoke(value);
        
    }

    public void ChangeAim(int change)
    {
        _currentAimIndex = (_currentAimIndex + change + _aimOptions.Length) % _aimOptions.Length;

        // value change
        PlayerPrefs.SetInt(AIM_MODE_PREF, _currentAimIndex);

        UpdateAimText();
    }

    private void UpdateAimText()
    {
        _aimOption.text = _aimOptions[_currentAimIndex];

        OnAimModeChange?.Invoke(_currentAimIndex == 0 ? true : false);
    }

    public void ChangeSprint(int change)
    {
        _currentSprintIndex = (_currentSprintIndex + change + _sprintOptions.Length) % _sprintOptions.Length;

        // value change
        PlayerPrefs.SetInt(SPRINT_MODE_PREF, _currentSprintIndex);
        
        UpdateSprintText();
    }

    private void UpdateSprintText()
    {
        _sprintOption.text = _sprintOptions[_currentSprintIndex];

        OnSprintModeChange?.Invoke(_currentSprintIndex == 0 ? true : false);
    }

    public void ChangeCrouch(int change)
    {
        _currentCrouchIndex = (_currentCrouchIndex + change + _crouchOptions.Length) % _crouchOptions.Length;

        // value change
        PlayerPrefs.SetInt(CROUCH_MODE_PREF, _currentCrouchIndex);

        UpdateCrouchText();
    }

    private void UpdateCrouchText()
    {
        _crouchOption.text = _crouchOptions[_currentCrouchIndex];

        OnCrouchModeChange?.Invoke(_currentCrouchIndex == 0 ? true : false);
    }
}
