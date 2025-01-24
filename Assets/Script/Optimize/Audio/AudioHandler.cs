using System.Collections.Generic;
using UnityEngine;

public abstract class AudioHandler : MonoBehaviour
{
    [SerializeField] protected SOAudioSFXList _audioSFXList;
    protected List<Audio> _audioList = new List<Audio>();
    // protected virtual int TotalAudioList {get {return _audioSFXList.audioSFX.Count;}}

    protected virtual void Awake()
    {
        InitializeAudioSource();
    }
    protected virtual void InitializeAudioSource()
    {
        for(int i = 0; i < _audioSFXList.audioSFX.Count ; i++)
        {
            Audio newAudio = new Audio();
            newAudio.audioType = _audioSFXList.audioSFX[i].audioType;
            newAudio.audioSource = gameObject.AddComponent<AudioSource>();
            
            _audioList.Add(newAudio);
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

            ApplyAudioClipToSource(audioSource, audio.audioSFXData);
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

        ApplyAudioClipToSource(audioSource, audio.audioSFXData);
            
        audioSource.Play();
    }
    protected virtual AudioSource GetAudioSource(AudioType audioType)
    {
        Audio audio = _audioList.Find(audio => audio.audioType == audioType);

        return audio.audioSource;
    }
    protected void ApplyAudioClipToSource(AudioSource source, AudioData audioData)
    {
        if(source.clip == audioData.audioClip) return;

        SetAudioClipToSource(source, audioData);
    }
    protected virtual void SetAudioClipToSource(AudioSource source, AudioData audioData)
    {
        source.clip = audioData.audioClip;
        source.outputAudioMixerGroup = audioData.audioMixerGroup;
        source.playOnAwake = audioData.playOnAwake;
        source.loop = audioData.loop;
        source.volume = audioData.volume;
        source.pitch = audioData.pitch;
    }

    protected virtual void StopLoopAudioSourceWhenPause()
    {
        foreach(Audio audio in _audioList)
        {
            if(audio.audioSource.loop) audio.audioSource.Stop();
        }
    }
}
