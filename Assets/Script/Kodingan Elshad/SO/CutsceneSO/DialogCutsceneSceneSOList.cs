using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogCutsceneSceneSOList", menuName = "ScriptableObject/DialogCutsceneSceneSOList")]
public class DialogCutsceneSceneSOList : ScriptableObject
{
    public List<DialogCutsceneSO> dialogCutsceneSOList;
    public DialogCutsceneSO GetLatestDialogScene(int currIdx)
    {
        if(currIdx < dialogCutsceneSOList.Count)
        {
            return dialogCutsceneSOList[currIdx];
        }
        return null;
    }   
}
