using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AudioManager : AudioHandler
{
    public static AudioManager Instance {get; private set;}

    [Space(1)]
    [SerializeField] private SOAudioBGMList _audioBGMList;

    [Header("Start BGM of This Scene")]
    [SerializeField] private AudioBGMName _startudioBGMName;
    private AudioBGMName _currBGMName;
    [SerializeField] private float _startBGMDuration = 1f, _stopBGMDuration = 0.5f;
    private float _currMaxVol = 0;
    
    private int _leanTweenBGMID;
    public static Action<AudioBGMName> OnChangeBGM;

    #region Getter Setter Variable
    // protected override int TotalAudioList {get {return _audioSFXList.audioSFX.Count + 1;}}
    #endregion

    protected override void Awake()
    {
        if(Instance != null)
        {
            OnChangeBGM?.Invoke(_startudioBGMName);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);

        base.Awake();

        OnChangeBGM += SetBGM;
    }



    #region BGM Method Helper
    private void SetBGM(AudioBGMName name)
    {
        // Debug.Log("SetBGM" + name);
        AudioSource audioSource = GetAudioSource(AudioType.BGM);

        if(audioSource.isPlaying && name != AudioBGMName.None && name != _currBGMName)
        {
            StopBGM(name);
            return;
        }

        AudioBGMData bgmData = _audioBGMList.GetAudioBGMData(name);
        if(bgmData == null) return;

        _currBGMName = name;
        SetAudioClipToSource(audioSource, bgmData);

        PlayBGM();
    }
    private void PlayBGM()
    {

        AudioSource audioSource = GetAudioSource(AudioType.BGM);
        float finalVolume = audioSource.volume;
        // Debug.Log("PlayBGM" + audioSource.clip + " " + finalVolume);

        audioSource.volume = 0;
        audioSource.Play();

        LeanTween.cancel(_leanTweenBGMID);
        _leanTweenBGMID = LeanTween.value(0, finalVolume, _startBGMDuration).setIgnoreTimeScale(true).setOnUpdate((float value) => {
            audioSource.volume = value;
        }).id;
    }
    private void StopBGM(AudioBGMName startAnotherBGM)
    {
        // Debug.Log("StopBGM" + startAnotherBGM);
        AudioSource audioSource = GetAudioSource(AudioType.BGM);
        float startVolume = audioSource.volume;
        
        LeanTween.cancel(_leanTweenBGMID);
        _leanTweenBGMID = LeanTween.value(startVolume, 0, _stopBGMDuration).setIgnoreTimeScale(true).setOnUpdate((float value) => {
            audioSource.volume = value;
        }).setOnComplete(
            ()=>
            {
                audioSource.Stop();
                if(startAnotherBGM != AudioBGMName.None) SetBGM(startAnotherBGM);
            }
        ).id;
    }
    public void ChangeBGMVolumeWhenPause(bool isPause)
    {
        AudioSource audioSource = GetAudioSource(AudioType.BGM);

        float vol = 0;
        if(isPause)
        {
            _currMaxVol = audioSource.volume;
            vol = _currMaxVol * 0.5f;
        }
        else vol = _currMaxVol;

        audioSource.volume = vol;

        if(isPause) StopLoopAudioSourceWhenPause();
    }

    #endregion

    #region Play AudioClip outside AudioManager position
    public void PlayAudioClip(AudioSFXName name, Vector3 position)
    {
        AudioSFXData audio = GetAudioSFXData(name);
        PlayClipAtPoint(audio, position);
    }
    private void PlayClipAtPoint(AudioSFXData audioData, Vector3 position)
    {
        GameObject gameObject = new GameObject("One shot audio");
        gameObject.transform.position = position;
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));

        audioSource.clip = audioData.audioClip;
        audioSource.outputAudioMixerGroup = audioData.audioMixerGroup;
        audioSource.volume = audioData.volume;
        audioSource.pitch = audioData.pitch;

        audioSource.spatialBlend = audioData.spatialBlend;
        audioSource.minDistance = audioData.minDistance;
        audioSource.maxDistance = audioData.maxDistance;

        audioSource.Play();
        Destroy(gameObject, audioData.audioClip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }
    private AudioSFXData GetAudioSFXData(AudioSFXName name)
    {
        var audio = _audioSFXList.GetAudioSFXData(name);

        return audio.audioSFXData;
    }
    #endregion

    #region override
    protected override void InitializeAudioSource()
    {
        for(int i = 0; i < _audioSFXList.audioSFX.Count ; i++)
        {
            if(_audioSFXList.audioSFX[i].useSpatialBlend)continue;

            Audio newAudio = new Audio();
            newAudio.audioType = _audioSFXList.audioSFX[i].audioType;
            newAudio.audioSource = gameObject.AddComponent<AudioSource>();

            _audioList.Add(newAudio);
        }

        Audio newAudioBGM = new Audio();
        newAudioBGM.audioType = AudioType.BGM;
        newAudioBGM.audioSource = gameObject.AddComponent<AudioSource>();

        _audioList.Add(newAudioBGM);
        SetBGM(_startudioBGMName);
    }
    protected override void StopLoopAudioSourceWhenPause()
    {
        foreach(Audio audio in _audioList)
        {
            if(audio.audioType == AudioType.BGM)
            {
                Debug.Log("AAAA");
                continue;
            }
            Debug.Log("AAAA hrsnya ga masuk");
            if(audio.audioSource.loop) audio.audioSource.Stop();
        }
    }
    #endregion

    #region PlaySFX
    public void PlayNighVision(bool isNightVisionOn)
    {
        if(isNightVisionOn) PlaySFXOnce(AudioSFXName.NightVision_On);
        else PlaySFXOnce(AudioSFXName.NightVision_Off);
    }
    public void PlayUIClick()
    {
        PlaySFXOnce(AudioSFXName.UI_Click);
    }
    #endregion
}
