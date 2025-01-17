using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagementManager : MonoBehaviour
{
    public static SceneManagementManager Instance {get; private set;}
    private FadeBGUIHandler _fadeUIHandler;
    public const string LOADING_SCREEN_SCENE_NAME = "Loading Screen";
    public const string SCENE_TO_LOADNAME_PREF = "SceneToLoad";
    private void Awake() 
    {
        Instance = this;
    }
    private void Start() 
    {
        _fadeUIHandler = FadeBGUIHandler.Instance;
    }
    public void SaveLoadSceneName(string sceneToLoad)
    {
        PlayerPrefs.SetString(SCENE_TO_LOADNAME_PREF, sceneToLoad);
    }
    public void GoToOtherScene()
    {
        IUnsubscribeEvent[] unsubs = FindObjectsOfType<MonoBehaviour>().OfType<IUnsubscribeEvent>().ToArray();
        foreach(IUnsubscribeEvent unsub in unsubs)
        {
            unsub.UnsubscribeEvent();
        }

        // Debug.Log("Halo??");
        Time.timeScale = 1f;
        _fadeUIHandler.ShowBGAtEnd(LoadLoadingScreen);
        
    }
    public void RestartScene()
    {
        SaveLoadSceneName(SceneManager.GetActiveScene().name);
        GoToOtherScene();
    }
    private void LoadLoadingScreen()
    {
        SceneManager.LoadScene(LOADING_SCREEN_SCENE_NAME);
    }
    public void ExitGame()
    {
        _fadeUIHandler.ShowBGAtEnd(QuitGame);
    }
    private void QuitGame()
    {
        Application.Quit();

        Debug.Log("Application Quit!");
    }
    public void GoToScene(string sceneName)
    {
        SaveLoadSceneName(sceneName);
        GoToOtherScene();
    }
    
}
