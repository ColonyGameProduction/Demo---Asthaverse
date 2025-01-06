using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayableMakeSFX : MonoBehaviour
{
    // [Header("Foot sound")]
    // private Transform _footSoundParent;
    // public staticv
    [SerializeField]private SOAudioSFXList _playerSFXList;
    private Audio[] _audioArray;
    private void Awake() 
    {
        _audioArray = new Audio[_playerSFXList.audioSFX.Count];
        for(int i = 0; i < _audioArray.Length ; i++)
        {
            _audioArray[i].audioName = _playerSFXList.audioSFX[i].audioName.ToString();

            _audioArray[i].audioSource = gameObject.AddComponent<AudioSource>();
            AudioSource source = _audioArray[i].audioSource;
            source.clip = _playerSFXList.audioSFX[i].audioClip;
            source.outputAudioMixerGroup = _playerSFXList.audioSFX[i].audioMixerGroup;
            source.playOnAwake = _playerSFXList.audioSFX[i].playOnAwake;
            source.loop = _playerSFXList.audioSFX[i].loop;
            source.volume = _playerSFXList.audioSFX[i].volume;
            source.pitch = _playerSFXList.audioSFX[i].pitch;

            source.spatialBlend = _playerSFXList.audioSFX[i].spatialBlend;
            source.minDistance = _playerSFXList.audioSFX[i].minDistance;
            source.maxDistance = _playerSFXList.audioSFX[i].maxDistance;

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

    private AudioSource GetAudioSource(AudioSFXName name)
    {
        Audio audio = System.Array.Find(_audioArray, audio => audio.audioName == name.ToString());
        return audio.audioSource;
    }
}
