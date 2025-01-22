using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public struct Audio
{
    public AudioType audioType;
    public AudioSource audioSource;
}

public enum AudioType
{
    None, BGM, Movement, Whistle, SwitchWeapon, Reload, NightVision, UI, Pain, PickupAmmo, BulletHits
}
public enum AudioSFXName
{
    None, NormalWalk, Whistle, FireGun,
    Walk_Dirt, Run_Dirt, Crouch_Dirt,
    SwitchWeapon_Start, SwitchWeapon_End,
    Reload_Rifle, Reload_Pistol,
    NightVision_On, NightVision_Off,
    BrokenItem, Alarm, 
    DoorRotate, DoorSlide,
    UI_Click, Pain, PickupAmmo,
    BulletHits_Head, BulletHits_Body,
    BulletHits_Window, BulletHits_Ground,
    BulletHits_Wood,

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

[Serializable]
public class AudioSFXTypeGroup
{
    public AudioType audioType;

    [Tooltip("For Audio Manager checker Only - true this if you want to play the clip at other place so there's spatial blend")]
    public bool useSpatialBlend = true;
    public List<AudioSFXData> audioSFX;
    
}
