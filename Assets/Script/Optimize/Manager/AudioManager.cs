using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; private set;}
    [Header("TEST")]
    public bool isTest;
    public Vector3 pos;
    [SerializeField] private SOAudioSFXList _audioSFXList;
    [SerializeField] private SOAudioBGMList _audioBGMList;
    private Audio[] _audioArray;

    [Header("Start BGM of This Scene")]
    [SerializeField] private AudioBGMName _startudioBGMName;
    [SerializeField] private float _startBGMDuration = 1f, _stopBGMDuration = 0.5f;
    
    private int _leanTweenBGMID;
    public const string BGM_AUDIOSOURCE_NAME = "BGM";
    public static Action<AudioBGMName> OnChangeBGM;
    private void Awake() 
    {
        if(Instance != null)
        {
            OnChangeBGM?.Invoke(_startudioBGMName);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);

        InitializeAudioSource();
        
        OnChangeBGM += SetBGM;
    }

    private void InitializeAudioSource()
    {
        _audioArray = new Audio[_audioSFXList.audioSFX.Count + 1];

        for(int i = 0; i < _audioSFXList.audioSFX.Count ; i++)
        {
            if(_audioSFXList.audioSFX[i].useSpatialBlend)continue;

            _audioArray[i].audioName = _audioSFXList.audioSFX[i].audioName.ToString();

            _audioArray[i].audioSource = gameObject.AddComponent<AudioSource>();
            AudioSource source = _audioArray[i].audioSource;

            source.clip = _audioSFXList.audioSFX[i].audioClip;
            source.outputAudioMixerGroup = _audioSFXList.audioSFX[i].audioMixerGroup;
            source.playOnAwake = _audioSFXList.audioSFX[i].playOnAwake;
            source.loop = _audioSFXList.audioSFX[i].loop;
            source.volume = _audioSFXList.audioSFX[i].volume;
            source.pitch = _audioSFXList.audioSFX[i].pitch;
        }

        int idx = _audioSFXList.audioSFX.Count;
        _audioArray[idx].audioName = BGM_AUDIOSOURCE_NAME;

        _audioArray[idx].audioSource = gameObject.AddComponent<AudioSource>();
        AudioSource source2 = _audioArray[idx].audioSource;
        SetBGM(_startudioBGMName);
    }

    private void Update() 
    {
        if(isTest)
        {
            isTest = false;
            PlayAudioClip(AudioSFXName.Whistle, pos);
        }
    }

    #region Play Audio at AudioManager position
    public void PlayStopSFX(AudioSFXName name, bool playSound)
    {
        AudioSource audioSource = GetAudioSource(name.ToString());
        if(playSound)
        {
            if(!audioSource.isPlaying)audioSource.Play();
        }
        else
        {
            if(audioSource.isPlaying)audioSource.Stop();
        }
    }
    public void PlaySFXOnce(AudioSFXName name)
    {
        AudioSource audioSource = GetAudioSource(name.ToString());
        audioSource.Play();
    }

    private AudioSource GetAudioSource(string name)
    {
        Audio audio = System.Array.Find(_audioArray, audio => audio.audioName == name);
        return audio.audioSource;
    }
    #endregion

    #region BGM Method Helper
    private void SetBGM(AudioBGMName name)
    {
        AudioSource audioSource = GetAudioSource(BGM_AUDIOSOURCE_NAME);

        if(audioSource.isPlaying && name != AudioBGMName.None)
        {
            StopBGM(name);
            return;
        }

        AudioBGMData bgmData = _audioBGMList.GetAudioBGMData(name);
        if(bgmData == null) return;

        audioSource.clip = bgmData.audioClip;
        audioSource.outputAudioMixerGroup = bgmData.audioMixerGroup;
        audioSource.playOnAwake = bgmData.playOnAwake;
        audioSource.loop = bgmData.loop;
        audioSource.volume = bgmData.volume;
        audioSource.pitch = bgmData.pitch;

        PlayBGM();
    }
    private void PlayBGM()
    {
        AudioSource audioSource = GetAudioSource(BGM_AUDIOSOURCE_NAME);
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
        AudioSource audioSource = GetAudioSource(BGM_AUDIOSOURCE_NAME);
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
        AudioSFXData audio = _audioSFXList.audioSFX.Find(audio => audio.audioName == name);
        return audio;
    }
    #endregion
}
