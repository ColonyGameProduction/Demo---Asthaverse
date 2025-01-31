
using UnityEngine;

public class TriggerQuestHandler : SoloQuestHandler, IUnsubscribeEvent
{
    protected IConnectToQuest _triggerObject;
    [SerializeField] private Collider[] _colls;
    private void Awake()
    {
        _triggerObject = GetComponent<IConnectToQuest>();
        _triggerObject.OnTriggerQuestComplete += QuestComplete;
    }

    public override void ActivateQuest()
    {
        base.ActivateQuest();
        ToggleCollider(true);
        
    }
    public override void DeactivateQuest()
    {
        ToggleCollider(false);
        base.DeactivateQuest();
    }

    public void UnsubscribeEvent()
    {
        _triggerObject.OnTriggerQuestComplete -= QuestComplete;
    }
    public virtual void ToggleCollider(bool isActivate)
    {
        foreach(Collider coll in _colls)
        {
            coll.enabled = isActivate;
        }
    }
}
