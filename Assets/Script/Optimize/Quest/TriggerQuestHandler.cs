using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerQuestHandler : SoloQuestHandler
{
    protected IConnectToQuest _triggerObject;
    [SerializeField] private Collider _coll;
    private void Awake()
    {
        _triggerObject = GetComponent<IConnectToQuest>();
        _triggerObject.OnTriggerQuestComplete += QuestComplete;
    }

    public override void ActivateQuest()
    {
        _isActivated = true;
        _coll.enabled = true;
    }
    public override void DeactivateQuest()
    {
        _coll.enabled = false;
        _isActivated = false;
    }
}
