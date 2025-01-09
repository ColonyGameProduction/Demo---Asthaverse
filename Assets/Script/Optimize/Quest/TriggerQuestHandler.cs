using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerQuestHandler : SoloQuestHandler, IUnsubscribeEvent
{
    protected IConnectToQuest _triggerObject;
    [SerializeField] private Collider _coll;
    [SerializeField] private GameObject _checkerInMap;
    private void Awake()
    {
        _triggerObject = GetComponent<IConnectToQuest>();
        _triggerObject.OnTriggerQuestComplete += QuestComplete;
    }

    public override void ActivateQuest()
    {
        _isActivated = true;
        _coll.enabled = true;
        if(_checkerInMap) _checkerInMap.SetActive(true);
    }
    public override void DeactivateQuest()
    {
        _coll.enabled = false;
        _isActivated = false;
        if(_checkerInMap) _checkerInMap.SetActive(false);
    }

    public void UnsubscribeEvent()
    {
        _triggerObject.OnTriggerQuestComplete -= QuestComplete;
    }
}
