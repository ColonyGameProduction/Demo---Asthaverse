using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogTrigger : MonoBehaviour
{
    QuestUIHandler QUH;
    public DialogCutsceneSO dialog;
    public InGameUIHandler inGameUIHandler;
    public Quest dialouge;
    bool isActivate;

    private void Start()
    {
        QUH = QuestUIHandler.instance;
        dialouge = GetComponent<Quest>();
        if(!dialouge.questActivate)
        {
            gameObject.SetActive(false);
        }
        else
        {
            QUH.CreatingQuestUI(dialouge.questName, dialouge);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(other.gameObject.GetComponentInParent<PlayerAction>().enabled)
            {
                if (!isActivate)
                {
                    inGameUIHandler.dialogCutscene = dialog;
                    inGameUIHandler.dialougeQuest = dialouge;
                    inGameUIHandler.nextQuestID = dialouge.nextQuestID;
                    inGameUIHandler.triggeringFailedQuest = dialouge.triggeringFailedQuest;
                    inGameUIHandler.DialogPlay();
                    isActivate = true;
                }
            }
        }
    }

}
