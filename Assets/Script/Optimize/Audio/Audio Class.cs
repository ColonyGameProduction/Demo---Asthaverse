using System;
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
    None = 0, BGM = 1, Movement = 2, Whistle = 3, SwitchWeapon = 4, Reload = 5, Shoot = 11, NightVision = 6, SilentKill = 12, UI = 7, Pain = 8, PickupAmmo = 9, BulletHits = 10, BrokeThrowItem = 11,
}
public enum AudioSFXName
{
    None = 0,  
    Walk_Dirt = 1, Run_Dirt = 2, Crouch_Dirt = 3,
    S_Whistle_1 = 4, S_Whistle_2 = 5, S_Whistle_3 = 6, S_Whistle_4 = 7,
    Pain_Grunts_1 = 8, Pain_Grunts_2 = 9, Pain_Grunts_3 = 10, Pain_Grunts_4 = 11, Pain_Grunts_5 = 12, Pain_Grunts_6 = 13, Pain_Grunts_7 = 14, Pain_Grunts_8 = 15,
    SwitchWeap_RP = 16, SwitchWeap_PR = 17,
    Reload_R_Stand = 18, Reload_R_Crouch = 24, Reload_P = 19,
    SilentKill_1 = 20, SilentKill_2 = 21,
    Shoot_R = 22, Shoot_P = 23,
    BrokeThrowItem_GlassBottle = 30,
    SwitchWeapon_Start, SwitchWeapon_End,
    Reload_Rifle, Reload_Pistol,
    NightVision_On = 40, NightVision_Off = 41, Alarm = 39, 
    DoorRotate, DoorSlide,
    UI_Click = 45, Pain, PickupAmmo,
    BulletHits_Head = 50, BulletHits_Body = 51,
    BulletHits_Window_1 = 52, BulletHits_Window_2 = 53, BulletHits_Window_3 = 54,
    BulletHits_Ground = 55,
    BulletHits_Wood = 56,
    Shoot_R_EnemyOnly = 24,

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
