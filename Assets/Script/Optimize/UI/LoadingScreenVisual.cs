using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenVisual : MonoBehaviour
{
    public Image mainBackground;
    public Sprite[] backgroundImages;
    public GameObject[] descriptions;

    private int currentIndex = 0;

    void Awake()
    {
        SetRandomBackground();
        UpdateBackgroundAndDescription();
    }

    private void SetRandomBackground()
    {
        currentIndex = Random.Range(0, backgroundImages.Length);
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