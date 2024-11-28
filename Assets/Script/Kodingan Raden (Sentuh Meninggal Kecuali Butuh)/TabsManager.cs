using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabsManager : MonoBehaviour
{
    public GameObject[] Tabs;
    public Image[] TabButtonImages;
    public TextMeshProUGUI[] TabButtonTexts;

    public GameObject mainMenuPanel;

    private float activeAlphaButton = 0.8f;
    private float inActiveAlpha = 0.5f;

    public void SwitchToTab(int TabID)
    {
        foreach (GameObject gameObject in Tabs)
        {
            gameObject.SetActive(false);
        }
        Tabs[TabID].SetActive(true);

        for (int i = 0; i < TabButtonImages.Length; i++)
        {
            SetButtonAlpha(TabButtonImages[i], TabButtonTexts[i], inActiveAlpha, inActiveAlpha);
        }
        SetButtonAlpha(TabButtonImages[TabID], TabButtonTexts[TabID], activeAlphaButton, 1f);
    }

    private void SetButtonAlpha(Image buttonImage, TextMeshProUGUI buttonText, float alphaValueButton, float alphaValueText)
    {
        if (buttonImage != null)
        {
            Color newImageColor = buttonImage.color;
            newImageColor.a = Mathf.Clamp01(alphaValueButton);
            buttonImage.color = newImageColor;
        }

        if (buttonText != null)
        {
            Color newTextColor = buttonText.color;
            newTextColor.a = Mathf.Clamp01(alphaValueText);
            buttonText.color = newTextColor;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
    }
}  
