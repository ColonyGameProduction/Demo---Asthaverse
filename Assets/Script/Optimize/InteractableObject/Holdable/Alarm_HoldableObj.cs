

public class Alarm_HoldableObj : HoldableObj_IntObj
{
    public override void WhenQuestActivated()
    {
        if(_audioSource != null) _audioSource.Play();
    }
    protected override void WhenComplete()
    {
        base.WhenComplete();
        if(_audioSource != null) _audioSource.Stop();
        // Debug.Log("RINNGGGGGGGGGGGGGGGGGGGGG!!!!!");
    }
}
