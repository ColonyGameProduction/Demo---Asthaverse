
public class SpecialSnipingMakeSFX : CharacterMakeSFX
{
    public override void PlayReloadSFX()
    {
        PlaySFXOnce(AudioSFXName.Reload_Sniper_Only);
    }
    public override void PlayShootSFX()
    {
        PlaySFXOnce(AudioSFXName.Shoot_Sniper_Only);
    }
}
