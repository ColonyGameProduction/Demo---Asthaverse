using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : QuestParent
{
    public QuestUIHandler QUH;
    public Quest theNextQuest;
    public List<Quest> multiplyQuestAtOnce = new List<Quest>();
    public bool canProceedToNextQuest;
    public List<int> nextQuestID = new List<int>();
    public List<int> triggeringFailedQuest = new List<int>();
    bool onceItFalse = false;


    private void Start()
    {
        QUH = QuestUIHandler.instance;
    }

    public override void CompletedTheQuest()
    {
        base.CompletedTheQuest();

        questComplete = true;
    }

    public override void ActivatingTheNextQuest(Quest currQuest)
    {
        base.ActivatingTheNextQuest(currQuest);

        
        questComplete = true;


        if (multiplyQuestAtOnce.Count > 0)
        {
            for (int i = 0; i < multiplyQuestAtOnce.Count; i++)
            {
                if (multiplyQuestAtOnce[i].questComplete == true)
                {
                    if(onceItFalse)
                    {
                        canProceedToNextQuest = false;
                    }
                    else
                    {
                        canProceedToNextQuest = true;
                    }
                    QUH.RemovingQuestUI(currQuest.questName, currQuest);
                }
                else
                {
                    onceItFalse = true;
                }
            }
        }
        else
        {
            canProceedToNextQuest = true;
        }

        // Debug.Log(canProceedToNextQuest);

        if (canProceedToNextQuest)
        {
            onceItFalse = false;
            QuestHandler QH = QuestHandler.questHandler;
            QUH.RemovingQuestUI(currQuest.questName, currQuest);

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

                QUH.CreatingQuestUI(quest.questName, quest);

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


