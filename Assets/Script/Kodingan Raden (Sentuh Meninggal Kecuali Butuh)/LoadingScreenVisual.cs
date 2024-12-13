using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenVisual : MonoBehaviour
{
    public Image mainBackground; // Gambar Main Background
    public Sprite[] backgroundImages; // Array gambar background
    public GameObject[] descriptions; // Array deskripsi sesuai dengan gambar

    private int currentIndex = 0; // Index gambar aktif
    public float switchInterval = 5f; // Interval waktu antar background (dalam detik)

    void Start()
    {
        UpdateBackgroundAndDescription(); // Set gambar dan deskripsi awal
        InvokeRepeating(nameof(ChangeBackground), switchInterval, switchInterval); // Mulai penggantian otomatis
    }

    private void ChangeBackground()
    {
        // Matikan deskripsi sebelumnya
        descriptions[currentIndex].SetActive(false);

        // Update ke gambar berikutnya
        currentIndex = (currentIndex + 1) % backgroundImages.Length;

        // Set gambar baru
        mainBackground.sprite = backgroundImages[currentIndex];

        // Aktifkan deskripsi baru
        descriptions[currentIndex].SetActive(true);
    }

    private void UpdateBackgroundAndDescription()
    {
        // Set gambar awal
        mainBackground.sprite = backgroundImages[currentIndex];

        // Aktifkan hanya deskripsi yang sesuai
        for (int i = 0; i < descriptions.Length; i++)
        {
            descriptions[i].SetActive(i == currentIndex);
        }
    }
}
