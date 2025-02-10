using System;


public interface IPlayableUseWeaponDataNeeded
{
    event Action OnTurningOffScope;
    public void ToggleLeftArmPistolRig(bool isActivate);
    void TellToTurnOffScope();
    public PlayableCharacterIdentity GetPlayableCharacterIdentity{ get ;}
    public void SilentKilledEnemyAnimation();
    public PlayableMakeSFX GetPlayableMakeSFX {get ;}
}
