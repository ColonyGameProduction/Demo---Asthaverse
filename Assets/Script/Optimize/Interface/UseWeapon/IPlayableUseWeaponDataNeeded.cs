using System;


public interface IPlayableUseWeaponDataNeeded
{
    event Action OnTurningOffScope;
    void TellToTurnOffScope();
    public PlayableCharacterIdentity GetPlayableCharacterIdentity{ get ;}
    public void SilentKilledEnemyAnimation();
    public PlayableMakeSFX GetPlayableMakeSFX {get ;}
}
