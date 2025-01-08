using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogSOList", menuName = "ScriptableObject/DialogSOList")]
public class DialogCutsceneSOList : ScriptableObject
{
    public List<DialogCutsceneSO> dialogCutsceneSOList;
    public DialogCutsceneSO SearchDialogCutScene(DialogCutsceneTitle title)
    {
        foreach(DialogCutsceneSO dialog in dialogCutsceneSOList)
        {
            if(dialog.title == title) return dialog;
        }
        return null;
    }
}
