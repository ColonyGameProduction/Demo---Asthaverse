using UnityEngine;
using UnityEngine.UI;

public class HoldButtonInteract : MonoBehaviour
{
    public QuestUIHandler QUH;
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
        QUH = QuestUIHandler.instance;
        thisQuest = GetComponent<Quest>();

        if(!thisQuest.questActivate && thisQuest != null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            QUH.CreatingQuestUI(thisQuest.questName, thisQuest);
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
                    thisQuest.ActivatingTheNextQuest(thisQuest);
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
}
