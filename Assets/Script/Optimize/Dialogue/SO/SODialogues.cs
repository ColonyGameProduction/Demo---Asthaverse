using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Dialogue_Lines
{
    public DialogueType dialogueType;
    public DialogueCharacterName charaName;
    public float delayTypeText;
    public float delayTypeBetweenLines;

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
    public DialogueTitle title;
    public DialogueSubTitle_Episode1 subtitle_Ep1;

    [Space(1)]
    [Header("Check this if you want the sub to be close straight away after finish")]
    public bool isCloseAfterFinish;
    public string VA_AudioName;
    public List<Dialogue_Lines> dialogue_Lines;
}
