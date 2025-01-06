using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public static Action<AudioBGMName> OnChangeBGM;
    private void Awake() 
    {
        if(Instance != null)
        {
            // OnChangeBGM?.Invoke();
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(Instance);

        InitializeAudioSource();
        
        for(int i = 0; i < _audioBGMList.audioBGM.Count ; i++)
        {
            _audioArray[i].audioName = _audioBGMList.audioBGM[i].audioName.ToString();

            _audioArray[i].audioSource = gameObject.AddComponent<AudioSource>();
            AudioSource source = _audioArray[i].audioSource;

            source.clip = _audioBGMList.audioBGM[i].audioClip;
            source.outputAudioMixerGroup = _audioBGMList.audioBGM[i].audioMixerGroup;
            source.playOnAwake = _audioBGMList.audioBGM[i].playOnAwake;
            source.loop = _audioBGMList.audioBGM[i].loop;
            source.volume = _audioBGMList.audioBGM[i].volume;
            source.pitch = _audioBGMList.audioBGM[i].pitch;
        }
    }

    private void InitializeAudioSource()
    {
        _audioArray = new Audio[_audioSFXList.audioSFX.Count + _audioBGMList.audioBGM.Count];

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
    private void PlayBGM(AudioBGMName name)
    {

    }
    private void StopBGM()
    {

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
        // Object.Destroy(gameObject, audioData.audioClip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }
    private AudioSFXData GetAudioSFXData(AudioSFXName name)
    {
        AudioSFXData audio = _audioSFXList.audioSFX.Find(audio => audio.audioName == name);
        return audio;
    }
    #endregion
}
