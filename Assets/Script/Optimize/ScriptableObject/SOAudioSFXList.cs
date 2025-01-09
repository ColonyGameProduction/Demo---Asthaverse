
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



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
    public List<AudioSFXTypeGroup> audioSFX;
    public (AudioType audioType, AudioSFXData audioSFXData) GetAudioSFXData(AudioSFXName audioName)
    {
        foreach(AudioSFXTypeGroup audioSFXGroup in audioSFX)
        {
            // currAudioType = 
            foreach(AudioSFXData audioSFXData in audioSFXGroup.audioSFX)
            {
                if(audioName == audioSFXData.audioName)
                {
                    return (audioSFXGroup.audioType, audioSFXData);
                }
            }
        }

        return (AudioType.None, null);
    }
}
