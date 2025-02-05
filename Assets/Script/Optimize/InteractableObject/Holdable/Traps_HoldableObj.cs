
public class Traps_HoldableObj : HoldableObj_IntObj
{
    protected override void WhenComplete()
    {
        base.WhenComplete();
        if(_audioSource != null) _audioSource.Play();
    }
}
