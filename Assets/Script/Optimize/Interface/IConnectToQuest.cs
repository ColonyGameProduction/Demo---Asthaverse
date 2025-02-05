using System;


public interface IConnectToQuest
{
    void WhenQuestActivated();
    event Action OnTriggerQuestComplete;
}

