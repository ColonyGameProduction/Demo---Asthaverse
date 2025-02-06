

using UnityEngine;

public class Alarm_HoldableObj : HoldableObj_IntObj
{
    [SerializeField] protected AudioSource _startQuestAudioSource;
    public override void WhenQuestActivated()
    {
        ToggleInteractableObjAudio(_startQuestAudioSource, true);
    }
    protected override void WhenComplete()
    {
        base.WhenComplete();
        ToggleInteractableObjAudio(_startQuestAudioSource, false);
    }
}
