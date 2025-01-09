using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AudioManager : AudioHandler
{
    public static AudioManager Instance {get; private set;}
    [Header("TEST")]
    public bool isTest;
    public Vector3 pos;

    [Space(1)]
    [SerializeField] private SOAudioBGMList _audioBGMList;

    [Header("Start BGM of This Scene")]
    [SerializeField] private AudioBGMName _startudioBGMName;
    [SerializeField] private float _startBGMDuration = 1f, _stopBGMDuration = 0.5f;
    private float _currMaxVol = 0;
    
    private int _leanTweenBGMID;
    public static Action<AudioBGMName> OnChangeBGM;

    #region Getter Setter Variable
    protected override int TotalAudioArray {get {return _audioSFXList.audioSFX.Count + 1;}}
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

    private void Update() 
    {
        if(isTest)
        {
            isTest = false;
            PlayAudioClip(AudioSFXName.Whistle, pos);
        }
    }


    #region BGM Method Helper
    private void SetBGM(AudioBGMName name)
    {
        AudioSource audioSource = GetAudioSource(AudioType.BGM);

        if(audioSource.isPlaying && name != AudioBGMName.None)
        {
            StopBGM(name);
            return;
        }

        AudioBGMData bgmData = _audioBGMList.GetAudioBGMData(name);
        if(bgmData == null) return;

        SetAudioClipToSource(audioSource, bgmData);

        PlayBGM();
    }
    private void PlayBGM()
    {
        AudioSource audioSource = GetAudioSource(AudioType.BGM);
        float finalVolume = audioSource.volume;

        audioSource.volume = 0;
        audioSource.Play();

        LeanTween.cancel(_leanTweenBGMID);
        _leanTweenBGMID = LeanTween.value(0, finalVolume, _startBGMDuration).setIgnoreTimeScale(true).setOnUpdate((float value) => {
            audioSource.volume = value;
        }).id;
    }
    private void StopBGM(AudioBGMName startAnotherBGM)
    {
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
        if(isPause) vol = audioSource.volume * 0.5f;
        else vol = _currMaxVol;

        audioSource.volume = vol;
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
        base.InitializeAudioSource();

        int idx = _audioSFXList.audioSFX.Count;
        _audioArray[idx].audioType = AudioType.BGM;

        _audioArray[idx].audioSource = gameObject.AddComponent<AudioSource>();
        AudioSource source2 = _audioArray[idx].audioSource;
        SetBGM(_startudioBGMName);
    }
    #endregion
}
