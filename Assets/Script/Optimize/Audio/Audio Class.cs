using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public struct Audio
{
    public string audioName;
    public AudioSource audioSource;
}

public enum AudioSFXName
{
    None, NormalWalk, Whistle, FireGun,
    BrokenItem, Alarm, 
    DoorRotate, DoorSlide
}
public enum AudioBGMName
{
    None, MainMenu,
    Team1_Ambience, Team1_Battle, 
    Team2_Ambience, Team2_Battle
}
[Serializable]
public class AudioSFXData : AudioData
{
    public AudioSFXName audioName;

    [Header("Spatial Blend")]
    [Range(0,1)]public float spatialBlend;
    public float minDistance = 1;
    public float maxDistance;
    // public bool _needSpecificParent;

    [Tooltip("For Audio Manager checker Only - true this if you want to play the clip at other place so there's spatial blend")]
    public bool useSpatialBlend = true;
}
[Serializable]
public class AudioBGMData : AudioData
{
    public AudioBGMName audioName;
}
public abstract class AudioData
{
    public AudioClip audioClip;
    public AudioMixerGroup audioMixerGroup;
    public bool playOnAwake;
    public bool loop;
    [Range(0,1)]public float volume;
    [Range(-3,3)]public float pitch = 1;

}
