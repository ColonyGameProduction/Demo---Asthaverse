using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ASyncLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pressAnyKeyPrompt;
    [SerializeField] private Image logoFillImage;
    [SerializeField] private GameObject mainMenuContainer;
    [SerializeField] private GameObject settingsMenuContainer;
    [SerializeField] private float startButtonDelay = 2.0f;
    [SerializeField] private float exitButtonDelay = 2.0f;
    [SerializeField] private float settingsButtonDelay = 2.0f;

    private bool isReadyToProceed = false;

    public void LoadLevelButton(string levelToLoad)
    {
        StartCoroutine(DelayStartButtonAnimation(levelToLoad));
    }

    public void ExitButton()
    {
        StartCoroutine(DelayExitButtonAnimation());
    }

    public void SettingsButton()
    {
        StartCoroutine(DelaySettingsButtonAnimation());
    }

    private IEnumerator DelayStartButtonAnimation(string levelToLoad)
    {
        yield return new WaitForSeconds(startButtonDelay);

        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);
        pressAnyKeyPrompt.SetActive(false);
        StartCoroutine(LoadLevelASync(levelToLoad));
    }

    private IEnumerator DelayExitButtonAnimation()
    {
        yield return new WaitForSeconds(exitButtonDelay);

        Application.Quit();

        Debug.Log("Application Quit!");
    }

    private IEnumerator DelaySettingsButtonAnimation()
    {
        yield return new WaitForSeconds(settingsButtonDelay);

        settingsMenuContainer.SetActive(true);
        mainMenuContainer.SetActive(false);
    }

    private IEnumerator LoadLevelASync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        loadOperation.allowSceneActivation = false;

        float displayedProgress = 0f;

        while (!loadOperation.isDone)
        {
            float targetProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);

            displayedProgress = Mathf.Lerp(displayedProgress, targetProgress, Time.deltaTime * 2f); // -> nilai 2f ini bisa di ganti kalo mau lebih cepet ato lebih lambat kecepetan progressnya

            logoFillImage.fillAmount = displayedProgress;

            if (loadOperation.progress >= 0.9f)
            {
                pressAnyKeyPrompt.SetActive(true);

                if (Input.anyKeyDown)
                {
                    isReadyToProceed = true;
                }
            }

            if (isReadyToProceed)
            {
                loadOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}