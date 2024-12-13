using UnityEngine;
using TMPro;

public class TabsManager : MonoBehaviour
{
    public GameObject[] Tabs;
    public GameObject[] TabButtonHighlights;
    public TextMeshProUGUI[] TabButtonTexts;

    public GameObject mainMenuPanel;

    private float activeAlphaText = 1f;
    private float inActiveAlphaText = 0.3f;

    public void SwitchToTab(int TabID)
    {
        foreach (GameObject gameObject in Tabs)
        {
            gameObject.SetActive(false);
        }
        Tabs[TabID].SetActive(true);

        for (int i = 0; i < TabButtonHighlights.Length; i++)
        {
            TabButtonHighlights[i].SetActive(false);
            SetTextAlpha(TabButtonTexts[i], inActiveAlphaText);
        }

        TabButtonHighlights[TabID].SetActive(true);
        SetTextAlpha(TabButtonTexts[TabID], activeAlphaText);
    }

    private void SetTextAlpha(TextMeshProUGUI buttonText, float alphaValueText)
    {
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