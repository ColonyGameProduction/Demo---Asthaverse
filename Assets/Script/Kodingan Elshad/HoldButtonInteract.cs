using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HoldButtonInteract : MonoBehaviour
{
    public List<int> nextQuestID = new List<int>();
    public int questID;

    public float curValue = 0;
    public float valueSpd = 5;
    public float maxValue = 10;

    public Quest thisQuest;

    public bool canProceedToNextQuest;
    public bool isComplete;
    public bool holdingButton;

    public GameObject holdingInterface;

    private void Start()
    {
        thisQuest = GetComponent<Quest>();

        if(!thisQuest.questActivate && thisQuest != null)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (holdingButton)
        {
            if (curValue < maxValue)
            {
                curValue += Time.deltaTime * valueSpd;
                holdingInterface.transform.GetChild(0).GetChild(1).GetComponent<Image>().fillAmount = curValue/maxValue;
            }
            else
            {
                if(!isComplete)
                {
                    holdingInterface.SetActive(false);
                    ActivatingNextQuest();
                }
                isComplete = true;
            }
        }
    }

    public void HoldInteraction(bool holdButton)
    {
        if(!isComplete)
        {
            curValue = 0;
            holdingButton = holdButton;
            if(holdButton)
            {
                holdingInterface.SetActive(true);
            }
            else
            {
                holdingInterface.SetActive(false);
            }
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

        for (int i = 0; i < nextQuestID.Count; i++)
        {
            if (canProceedToNextQuest)
            {
                QuestHandler QH = QuestHandler.questHandler;
                Quest quest = QH.questList[nextQuestID[i]];
                quest.questActivate = true;
                quest.gameObject.SetActive(true);
                if (nextQuestID.Count > 1)
                {
                    for (int j = 0; j < nextQuestID.Count; j++)
                    {
                        quest.multiplyQuestAtOnce.Add(QH.questList[nextQuestID[j]]);
                    }
                }
            }
        }
    }
}
