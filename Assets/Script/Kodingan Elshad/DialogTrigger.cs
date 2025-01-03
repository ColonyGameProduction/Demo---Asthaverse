using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogTrigger : MonoBehaviour
{
    public List<int> nextQuestID = new List<int>();
    public List<int> triggeringFailedQuest = new List<int>();


    public DialogCutsceneSO dialog;
    public InGameUIHandler inGameUIHandler;
    public Quest dialouge;
    bool isActivate;

    private void Start()
    {
        dialouge = GetComponent<Quest>();
        if(!dialouge.questActivate)
        {
            gameObject.SetActive(false);
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
                    inGameUIHandler.nextQuestID = nextQuestID;
                    inGameUIHandler.triggeringFailedQuest = triggeringFailedQuest;
                    inGameUIHandler.DialogPlay();
                    isActivate = true;
                }
            }
        }
    }

}
