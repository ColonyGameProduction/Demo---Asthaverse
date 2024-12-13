using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenVisual : MonoBehaviour
{
    public Image mainBackground;
    public Sprite[] backgroundImages;
    public GameObject[] descriptions;

    private int currentIndex = 0;
    public float switchInterval = 5f;

    void Start()
    {
        UpdateBackgroundAndDescription();
        InvokeRepeating(nameof(ChangeBackground), switchInterval, switchInterval);
    }

    private void ChangeBackground()
    {
        descriptions[currentIndex].SetActive(false);

        currentIndex = (currentIndex + 1) % backgroundImages.Length;

        mainBackground.sprite = backgroundImages[currentIndex];

        descriptions[currentIndex].SetActive(true);
    }

    private void UpdateBackgroundAndDescription()
    {
        mainBackground.sprite = backgroundImages[currentIndex];

        for (int i = 0; i < descriptions.Length; i++)
        {
            descriptions[i].SetActive(i == currentIndex);
        }
    }
}
