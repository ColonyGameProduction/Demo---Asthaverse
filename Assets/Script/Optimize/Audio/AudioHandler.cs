using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioHandler : MonoBehaviour
{
    [SerializeField] protected SOAudioSFXList _audioSFXList;
    protected Audio[] _audioArray;
    protected virtual int TotalAudioArray {get {return _audioSFXList.audioSFX.Count;}}

    protected virtual void Awake()
    {
        InitializeAudioSource();
    }
    protected virtual void InitializeAudioSource()
    {
        _audioArray = new Audio[TotalAudioArray];
        for(int i = 0; i < _audioSFXList.audioSFX.Count ; i++)
        {
            if(_audioSFXList.audioSFX[i].useSpatialBlend)continue;

            _audioArray[i].audioType = _audioSFXList.audioSFX[i].audioType;

            _audioArray[i].audioSource = gameObject.AddComponent<AudioSource>();
            AudioSource source = _audioArray[i].audioSource;

        }
    }

    protected virtual void PlaySFX(AudioSFXName name)
    {
        if(name == AudioSFXName.None) return;

        var audio = _audioSFXList.GetAudioSFXData(name);
        if(audio.audioType == AudioType.None) return;

        AudioSource audioSource = GetAudioSource(audio.audioType);
        if(audioSource == null) return;

        if(audioSource.clip != audio.audioSFXData.audioClip)
        {
            if(audioSource.isPlaying) audioSource.Stop();

            SetAudioClipToSource(audioSource, audio.audioSFXData);
        }
        
        if(!audioSource.isPlaying) audioSource.Play();
    }

    protected virtual void StopSFX(AudioType sfxType)
    {
        if(sfxType == AudioType.None) return;

        AudioSource audioSource = GetAudioSource(sfxType);
        if(audioSource == null) return;

        if(audioSource.isPlaying) audioSource.Stop();
    }

    protected virtual void PlaySFXOnce(AudioSFXName name)
    {
        var audio = _audioSFXList.GetAudioSFXData(name);
        if(audio.audioType == AudioType.None) return;

        AudioSource audioSource = GetAudioSource(audio.audioType);

        SetAudioClipToSource(audioSource, audio.audioSFXData);
            
        audioSource.Play();
    }
    protected virtual AudioSource GetAudioSource(AudioType audioType)
    {
        Audio audio = System.Array.Find(_audioArray, audio => audio.audioType == audioType);

        return audio.audioSource;
    }
    protected virtual void SetAudioClipToSource(AudioSource source, AudioData audioData)
    {
        if(source.clip == audioData.audioClip) return;

        source.clip = audioData.audioClip;
        source.outputAudioMixerGroup = audioData.audioMixerGroup;
        source.playOnAwake = audioData.playOnAwake;
        source.loop = audioData.loop;
        source.volume = audioData.volume;
        source.pitch = audioData.pitch;
    }
}
