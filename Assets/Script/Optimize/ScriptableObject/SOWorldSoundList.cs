using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum WorldSoundName
{
    None, Walk, Whistle, Bottle
}
[Serializable]
public class WorldSound
{
    public WorldSoundName Name;
    public float SoundRange;
}
public class SOWorldSoundList : ScriptableObject
{
    #if UNITY_EDITOR
    [MenuItem("SO/SOWorldSoundList")]
    public static void QuickCreate()
    {
        SOWorldSoundList asset = CreateInstance<SOWorldSoundList>();
        string name =
            AssetDatabase.GenerateUniqueAssetPath("Assets/ScriptableObject/Sound//WorldSoundList.asset");
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
    #endif
    public List<WorldSound> worldSounds;
}
