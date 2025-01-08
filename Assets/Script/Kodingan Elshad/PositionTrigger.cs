using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTrigger : MonoBehaviour
{
    public QuestUIHandler QUH;
    public Quest thisQuest;
    public bool canProceedToNextQuest;

    private void Start()
    {
        QUH = QuestUIHandler.instance;
        thisQuest = GetComponent<Quest>();
        if (!thisQuest.questActivate)
        {
            gameObject.SetActive(false);
        }
        else
        {
            QUH.CreatingQuestUI(thisQuest.questName, thisQuest);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            thisQuest.ActivatingTheNextQuest(thisQuest);
            gameObject.SetActive(false);
        }
    }

    
}
