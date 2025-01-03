using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTrigger : MonoBehaviour
{
    public List<int> nextQuestID = new List<int>();
    public List<int> triggeringFailedQuest = new List<int>();
    public Quest thisQuest;
    public bool canProceedToNextQuest;

    private void Start()
    {
        thisQuest = GetComponent<Quest>();
        if(!thisQuest.questActivate)
        {
            gameObject.SetActive(false);
        }    
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            ActivatingNextQuest();
        }
    }

    public void ActivatingNextQuest()
    {
        thisQuest.questComplete = true;

        if (thisQuest.multiplyQuestAtOnce.Count > 0)
        {
            for (int i = 0; i < thisQuest.multiplyQuestAtOnce.Count; i++)
            {
                if (thisQuest.multiplyQuestAtOnce[i].questComplete == true)
                {
                    canProceedToNextQuest = true;
                }
                else
                {
                    canProceedToNextQuest = false;
                    break;
                }
            }
        }
        else
        {
            canProceedToNextQuest = true;
        }

        Debug.Log(canProceedToNextQuest);

        if (canProceedToNextQuest)
        {
            QuestHandler QH = QuestHandler.questHandler;

            if (triggeringFailedQuest.Count > 0)
            {
                for (int j = 0; j < triggeringFailedQuest.Count; j++)
                {
                    QH.questList[triggeringFailedQuest[j]].questActivate = false;
                    QH.questList[triggeringFailedQuest[j]].gameObject.SetActive(false);
                }
            }
            for (int i = 0; i < nextQuestID.Count; i++)
            {
                Quest quest = QH.questList[nextQuestID[i]];
                quest.questActivate = true;
                quest.gameObject.SetActive(true);

                if (nextQuestID.Count > 1)
                {
                    for (int j = 0; j < nextQuestID.Count; j++)
                    {
                        if (!quest.isOptional)
                        {
                            if (!QH.questList[nextQuestID[j]].isOptional)
                            {
                                quest.multiplyQuestAtOnce.Add(QH.questList[nextQuestID[j]]);
                            }
                        }
                        else
                        {
                            if (QH.questList[nextQuestID[j]].isOptional)
                            {
                                quest.multiplyQuestAtOnce.Add(QH.questList[nextQuestID[j]]);
                            }
                        }
                    }
                }
            }
        }
    }
}
