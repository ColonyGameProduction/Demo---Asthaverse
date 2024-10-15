using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueCharacter
{
    public DialogueCharacterName charaName;
    public string charaFullName;
    public Color charaDialogueColor;
}
public class SODialogueCharacterList : ScriptableObject
{
    #if UNITY_EDITOR
    [MenuItem("SO/SODialogueCharacterList")]
    public static void QuickCreate()
    {
        SODialogueCharacterList asset = CreateInstance<SODialogueCharacterList>();
        string name =
            AssetDatabase.GenerateUniqueAssetPath("Assets/Scriptable Objects/Cutscene//DialogueCharacterList.asset");
        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
    #endif

    public List<DialogueCharacter> CharacterList;
}
