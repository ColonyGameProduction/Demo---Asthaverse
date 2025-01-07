using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    public const string SCENE_TO_LOADNAME_PREF = "SceneToLoad";
    [SerializeField] private Image _logoFillImage;
    [SerializeField] private GameObject _pressAnyKeyPrompt;
    private FadeBGUIHandler _fadeUIHandler;
    private bool _isReadyToProceed = false;

    private void Start() 
    {
        _fadeUIHandler = FadeBGUIHandler.Instance;
        _fadeUIHandler.HideBGStart(StartLoadNextLevel);
    }
    private void StartLoadNextLevel()
    {
        string sceneName = PlayerPrefs.GetString(SCENE_TO_LOADNAME_PREF);
        StartCoroutine(LoadLevelASync(sceneName));
    }
    private IEnumerator LoadLevelASync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        loadOperation.allowSceneActivation = false;

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);

            _logoFillImage.fillAmount = progressValue;

            if (loadOperation.progress >= 0.9f)
            {
                _pressAnyKeyPrompt.SetActive(true);
                if (Input.anyKeyDown)
                {
                    _isReadyToProceed = true;
                }
            }

            if (_isReadyToProceed)
            {
                loadOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
