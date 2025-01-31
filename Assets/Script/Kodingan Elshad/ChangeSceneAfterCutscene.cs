using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ChangeSceneAfterCutscene : MonoBehaviour
{
    public SceneManagementManager SMM;
    public string SceneName;
    public VideoPlayer video;
    public bool _hasFinish = false;

    private void Start()
    {
        SMM = SceneManagementManager.Instance;
        video.loopPointReached += Video_loopPointReached;
    }

    private void Video_loopPointReached(VideoPlayer source)
    {
        SMM.GoToScene(SceneName);   
        _hasFinish = true;
    }
}
 