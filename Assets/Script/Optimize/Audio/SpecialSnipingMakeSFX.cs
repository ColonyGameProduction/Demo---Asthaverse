
public class SpecialSnipingMakeSFX : CharacterMakeSFX
{
    public override void PlayReloadSFX()
    {
        PlaySFXOnce(AudioSFXName.Reload_R_Stand);
    }
    public override void PlayShootSFX()
    {
        PlaySFXOnce(AudioSFXName.Shoot_R);
    }
}
