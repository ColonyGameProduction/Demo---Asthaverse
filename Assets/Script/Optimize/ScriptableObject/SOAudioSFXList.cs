using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;
using System;

public enum AudioSFXName
{
    None, NormalWalk, Whistle
}
[Serializable]
public class AudioSFX
{
    public AudioSFXName audioName;
    public AudioClip audioClip;
    public AudioMixerGroup audioMixerGroup;
    public bool playOnAwake;
    public bool loop;
    [Range(0,1)]public float volume;
    [Range(-3,3)]public float pitch = 1;
    // public bool _needSpecificParent;
}
public class SOAudioSFXList : ScriptableObject
{
    #if UNITY_EDITOR
    [MenuItem("SO/SOAudioSFXList")]
    public static void QuickCreate()
    {
        SOAudioSFXList asset = CreateInstance<SOAudioSFXList>();
        string name =
            AssetDatabase.GenerateUniqueAssetPath("Assets/ScriptableObject/Sound//AudioSFXList.asset");
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
    #endif
    public List<AudioSFX> audioSFX;
}
