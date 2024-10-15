using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class dialogue_Lines
{
    public DialogueType dialogueType;
    public DialogueCharacterName charaName;

    public string dialogueLine;
}
public class SODialogues : ScriptableObject
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
    // public DialogueSubTitle_Episode1
    [Tooltip("Check this if you want the sub to be close straight away after finish")]
    public bool isCloseAfterFinish;
    public string VA_AudioName;
    public List<dialogue_Lines> dialogue_Lines;
}
