using UnityEngine;
using UnityEngine.UI;

public class BackgroundChanger : MonoBehaviour
{
    public Sprite[] backgroundImages; // Array to store background images
    public float changeInterval = 1f; // Time interval between changes

    private Image backgroundImage;
    private int currentIndex = 0;

    void Start()
    {
        // Get the Image component
        backgroundImage = GetComponent<Image>();

        if (backgroundImages.Length > 0)
        {
            // Start the cycle
            InvokeRepeating(nameof(ChangeBackground), 0f, changeInterval);
        }
        else
        {
            // Debug.LogError("No images assigned to the BackgroundChanger script!");
        }
    }

    void ChangeBackground()
    {
        // Change to the next image in the array
        currentIndex = (currentIndex + 1) % backgroundImages.Length;
        backgroundImage.sprite = backgroundImages[currentIndex];
    }
}

