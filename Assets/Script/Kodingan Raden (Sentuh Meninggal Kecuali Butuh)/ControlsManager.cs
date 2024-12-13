using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsManager : MonoBehaviour
{
    [Header("Sensitivity")]
    public TextMeshProUGUI sensPresentageText;
    public Slider sensSlider;
    public Button sensLeftButton;
    public Button sensRightButton;

    [Header("Aim")]
    public TextMeshProUGUI aimOption;

    [Header("Sprint")]
    public TextMeshProUGUI sprintOption;

    [Header("Crouch")]
    public TextMeshProUGUI crouchOption;

    private float stepSize = 0.01f;

    private float sensCurrentPresentage = 0.5f;

    //Aim
    private int currentAimIndex = 0;
    private readonly string[] aimOptions = { "Hold", "Toggle"};

    //Sprint
    private int currentSprintIndex = 0;
    private readonly string[] sprintOptions = { "Hold", "Toggle" };

    //Crouch
    private int currentCrouchIndex = 0;
    private readonly string[] crouchOptions = { "Hold", "Toggle" };

    private void Start()
    {
        sensSlider.value = sensCurrentPresentage;
        UpdateSensPresentage(sensCurrentPresentage);

        sensLeftButton.onClick.AddListener(() => ChangeSensPresentage(-stepSize));
        sensRightButton.onClick.AddListener(() => ChangeSensPresentage(stepSize));

        UpdateAimText();
        UpdateSprintText();
    }

    public void ChangeSensPresentage(float delta)
    {
        sensCurrentPresentage = Mathf.Clamp(sensCurrentPresentage + delta, 0f, 1f);

        sensSlider.value = sensCurrentPresentage;
        UpdateSensPresentage(sensCurrentPresentage);
    }

    public void UpdateSensPresentage(float value)
    {
        sensPresentageText.text = Mathf.RoundToInt(value * 100) + "%";

        // value change
        // ------------
    }

    public void ChangeAim(int change)
    {
        currentAimIndex = (currentAimIndex + change + aimOptions.Length) % aimOptions.Length;

        // value change
        // ------------

        UpdateAimText();
    }

    private void UpdateAimText()
    {
        aimOption.text = aimOptions[currentAimIndex];
    }

    public void ChangeSprint(int change)
    {
        currentSprintIndex = (currentSprintIndex + change + sprintOptions.Length) % sprintOptions.Length;

        // value change
        // ------------

        UpdateSprintText();
    }

    private void UpdateSprintText()
    {
        sprintOption.text = sprintOptions[currentSprintIndex];
    }

    public void ChangeCrouch(int change)
    {
        currentCrouchIndex = (currentCrouchIndex + change + crouchOptions.Length) % crouchOptions.Length;

        // value change
        // ------------

        UpdateCrouchText();
    }

    private void UpdateCrouchText()
    {
        crouchOption.text = crouchOptions[currentCrouchIndex];
    }
}
