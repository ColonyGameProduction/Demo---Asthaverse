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
    bool _isFirsTime = true;
    public bool _isEndingSceneFirst = false;
    [SerializeField] private ThankFinishUI _thankFinishUI;

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
        if(video.isPrepared && _isFirsTime)
        {
            _isFirsTime = false;
            video.Play();
        }
    }

    private void Video_loopPointReached(VideoPlayer source)
    {
        if(_isEndingSceneFirst)
        {
            _thankFinishUI.ShowThanks(()=>SMM.GoToScene(SceneName));
        }
        else
        {
            SMM.GoToScene(SceneName);   
        }
        _hasFinish = true;
        
    }
}
 