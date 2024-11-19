using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;
using System;
using UnityEditor.EditorTools;

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

    [Header("Spatial Blend")]
    [Range(0,1)]public float spatialBlend;
    public float minDistance = 1;
    public float maxDistance;
    // public bool _needSpecificParent;

    [Tooltip("For Audio Manager Only, true this if you want to create it in audiomanager and no need spacial blend - for example like BGM")]
    public bool useSpatialBlend = true;
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
