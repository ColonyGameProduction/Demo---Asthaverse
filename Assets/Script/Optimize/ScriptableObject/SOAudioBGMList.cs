
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SOAudioBGMList : ScriptableObject
{
    #if UNITY_EDITOR
    [MenuItem("SO/SOAudioBGMList")]
    public static void QuickCreate()
    {
        SOAudioBGMList asset = CreateInstance<SOAudioBGMList>();
        string name =
            AssetDatabase.GenerateUniqueAssetPath("Assets/ScriptableObject/Sound//AudioBGMList.asset");
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
    #endif
    public List<AudioBGMData> audioBGM;

    public AudioBGMData GetAudioBGMData(AudioBGMName name)
    {
        foreach(AudioBGMData data in audioBGM)
        {
            if(data.audioName == name) return data;
        }   
        return null;
    }
}
