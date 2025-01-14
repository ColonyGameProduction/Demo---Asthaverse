using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogCutsceneSceneSOList", menuName = "ScriptableObject/DialogCutsceneSceneSOList")]
public class DialogCutsceneSceneSOList : ScriptableObject
{
    public int currIdx;
    public List<DialogCutsceneSO> dialogCutsceneSOList;
    public DialogCutsceneSO GetLatestDialogScene()
    {
        if(currIdx < dialogCutsceneSOList.Count)
        {
            int oldIdx = currIdx;
            currIdx++;
            return dialogCutsceneSOList[oldIdx];
        }
        return null;
    }   
}
