
using System;
using UnityEngine;


public class PlayableMakeSFX : CharacterMakeSFX
{
    protected PlayableCharacterIdentity _playableCharaIdentity;
    private const string  AUDIOSFXNAME_FRONT_WHISTLETYPE = "S_Whistle_";
    public void PlayWhistleSFX()
    {
        int maxWhistleSFXCount = _audioSFXList.GetAudioSFXListCountPerType(AudioType.Whistle);  

        int chosenCount = 0;
        if(maxWhistleSFXCount > 0) chosenCount = UnityEngine.Random.Range(1, maxWhistleSFXCount + 1);

        // Debug.Log("Chosen whistle" + chosenCount);
        if(Enum.TryParse(AUDIOSFXNAME_FRONT_WHISTLETYPE + chosenCount.ToString(), out AudioSFXName result))
        {
            PlaySFXOnce(result);
        }
    }

    public void PlaySilentKillSFX(float idx)
    {
        if(idx == 0) PlaySFXOnce(AudioSFXName.SilentKill_1);
        else if(idx == 1) PlaySFXOnce(AudioSFXName.SilentKill_2);
    }

    public void PlaySwitchWeaponSFX()
    {
        if(_playableCharaIdentity.CurrWeaponIdx == 0)
        {
            PlaySFXOnce(AudioSFXName.SwitchWeap_RP);
        }
        else PlaySFXOnce(AudioSFXName.SwitchWeap_PR);
        
    }


    #region overide
    protected override void Awake()
    {
        base.Awake();
        _playableCharaIdentity = _charaIdentity as PlayableCharacterIdentity;
        
    }
    protected override void SetAudioClipToSource(AudioSource source, AudioData audioData)
    {
        base.SetAudioClipToSource(source, audioData);

        AudioSFXData data = audioData as AudioSFXData;

        source.spatialBlend = data.spatialBlend;
        source.minDistance = data.minDistance;
        source.maxDistance = data.maxDistance;
    }
    public override void PlayReloadSFX()
    {
        if(_playableCharaIdentity.CurrWeaponIdx == 0)
        {
            base.PlayReloadSFX();
        }
        else PlaySFXOnce(AudioSFXName.Reload_P);
    }
    public override void PlayShootSFX()
    {
        if(_playableCharaIdentity.CurrWeaponIdx == 0)
        {
            PlaySFXOnce(AudioSFXName.Shoot_R);
        }
        else PlaySFXOnce(AudioSFXName.Shoot_P);
    }
    
    #endregion

}
