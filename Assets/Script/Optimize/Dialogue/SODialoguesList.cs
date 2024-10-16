using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SODialoguesList : ScriptableObject
{
    #if UNITY_EDITOR
    [MenuItem("SO/SODialogues")]
    public static void QuickCreate()
    {
        SODialogues asset = CreateInstance<SODialogues>();
        string name =
            AssetDatabase.GenerateUniqueAssetPath("Assets/Scriptable Objects/Cutscene//Dialogues.asset");
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
    #endif

    
}

