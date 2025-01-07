using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Dialog", menuName = "ScriptableObject/DialogSO")]
public class DialogCutsceneSO : ScriptableObject
{
    public DialogCutsceneTitle title;
    public List<CutsceneMouthSO> character = new List<CutsceneMouthSO>();
    public List<string> dialogSentence = new List<string>();
    public List<int> charID = new List<int>();

}
