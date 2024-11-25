using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; private set;}
    [Header("TEST")]
    public bool isTest;
    public Vector3 pos;
    [SerializeField]private SOAudioSFXList _audioSFXList;
    private List<Audio> _audioList = new List<Audio>();
    private void Awake() 
    {
        Instance = this;
        for(int i = 0; i < _audioSFXList.audioSFX.Count ; i++)
        {
            if(_audioSFXList.audioSFX[i].useSpatialBlend)continue;

            // _audioList[i].audioName = _audioSFXList.audioSFX[i].audioName;

            AudioSource source = gameObject.AddComponent<AudioSource>();
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

    public void PlayStopSFX(AudioSFXName name, bool playSound)
    {
        AudioSource audioSource = GetAudioSource(name);
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
        AudioSource audioSource = GetAudioSource(name);
        audioSource.Play();
    }

    public void PlayAudioClip(AudioSFXName name, Vector3 position)
    {
        AudioSFX audio = GetAudioSFXData(name);
        PlayClipAtPoint(audio, position);
    }
    private void PlayClipAtPoint(AudioSFX audioData, Vector3 position)
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
        Object.Destroy(gameObject, audioData.audioClip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }

    private AudioSource GetAudioSource(AudioSFXName name)
    {
        Audio audio = _audioList.Find(audio => audio.audioName == name);
        return audio.audioSource;
    }
    private AudioSFX GetAudioSFXData(AudioSFXName name)
    {
        AudioSFX audio = _audioSFXList.audioSFX.Find(audio => audio.audioName == name);
        return audio;
    }
}
