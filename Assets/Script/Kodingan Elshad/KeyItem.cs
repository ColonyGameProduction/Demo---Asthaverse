using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : MonoBehaviour
{
    public QuestUIHandler QUH;
    public Quest thisQuest;
    private void Start()
    {
        QUH = QuestUIHandler.instance;
        thisQuest = GetComponent<Quest>();

        if (!thisQuest.questActivate && thisQuest != null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            QUH.CreatingQuestUI(thisQuest.questName, thisQuest);
        }
    }

    public void KeyItemInteract()
    {
        if (thisQuest != null && thisQuest.questComplete != true)
        {
            Debug.Log("Quest Complete");
            thisQuest.questComplete = true;
            thisQuest.ActivatingTheNextQuest(thisQuest);
        }
    }

    
}
