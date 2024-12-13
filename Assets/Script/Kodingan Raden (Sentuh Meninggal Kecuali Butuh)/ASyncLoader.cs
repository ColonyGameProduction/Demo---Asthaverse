using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ASyncLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pressAnyKeyPrompt; // UI untuk "Press Any Key" prompt
    [SerializeField] private Image logoFillImage; // Image yang akan diubah fillAmount-nya (logo berwarna)

    private bool isReadyToProceed = false; // Apakah pemain sudah menekan tombol

    public void LoadLevelButton(string levelToLoad)
    {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);
        pressAnyKeyPrompt.SetActive(false); // Awalnya disembunyikan
        StartCoroutine(LoadLevelASync(levelToLoad));
    }

    IEnumerator LoadLevelASync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        loadOperation.allowSceneActivation = false; // Jangan langsung pindah ke scene

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);

            // Update fillAmount logo berdasarkan progress
            logoFillImage.fillAmount = progressValue;

            // Jika sudah selesai loading tetapi menunggu pemain
            if (loadOperation.progress >= 0.9f)
            {
                pressAnyKeyPrompt.SetActive(true); // Tampilkan prompt "Press Any Key"
                if (Input.anyKeyDown) // Tunggu pemain menekan tombol apa saja
                {
                    isReadyToProceed = true;
                }
            }

            // Jika pemain sudah siap, aktifkan scene
            if (isReadyToProceed)
            {
                loadOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}