using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : QuestParent
{
    public Quest theNextQuest;
    public List<Quest> multiplyQuestAtOnce = new List<Quest>();

    public override void CompletedTheQuest()
    {
        base.CompletedTheQuest();

        questComplete = true;
    }

    public override void ActivatingTheNextQuest()
    {
        base.ActivatingTheNextQuest();

        theNextQuest.questActivate = true;
    }

}
