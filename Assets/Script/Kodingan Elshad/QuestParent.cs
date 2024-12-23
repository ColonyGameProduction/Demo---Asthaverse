using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestParent : MonoBehaviour
{
    public string questName, questDescription;
    public bool questActivate, questComplete;

    public virtual void QuestIsntStarting()
    {

    }

    public virtual void StartingTheQuest()
    {

    }

    public virtual void CompletedTheQuest()
    {

    }

    public virtual void ActivatingTheNextQuest()
    {

    }

}
