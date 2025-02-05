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

    private void Awake() 
    {
        video.Prepare();
    }
    private void Start()
    {
        SMM = SceneManagementManager.Instance;
        video.loopPointReached += Video_loopPointReached;
        
    }
    private void Update() 
    {
        if(video.isPrepared) video.Play();
    }

    private void Video_loopPointReached(VideoPlayer source)
    {
        SMM.GoToScene(SceneName);   
        _hasFinish = true;
    }
}
 