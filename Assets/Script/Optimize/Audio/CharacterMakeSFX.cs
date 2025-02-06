using System;
using UnityEngine;

public class CharacterMakeSFX : AudioHandler, IUnsubscribeEvent
{
    [Header("Manager variable")]
    protected GameManager _gm;
    protected CharacterIdentity _charaIdentity;

    private const string  AUDIOSFXNAME_FRONT_PAINGRUNTTYPE = "Pain_Grunts_";
    [Header("Walk SFX")]
    protected bool _isOnDirt = true;
    public bool IsOnDirt 
    {
        set
        {
            _isOnDirt = value;
        }
    }
    protected override void Awake()
    {
        base.Awake();
        _charaIdentity = GetComponentInParent<CharacterIdentity>();
        
    }
    protected virtual void Start() 
    {
        _gm = GameManager.instance;
        _gm.OnPlayerPause += GameManager_OnPlayerPause;
    }

    #region MovementSFX
    public void PlayWalkSFX()
    {
        AudioSFXName audioName = _isOnDirt ? AudioSFXName.Walk_Dirt : AudioSFXName.None;
        PlaySFX(audioName);
    }
    public void PlayRunSFX()
    {
        AudioSFXName audioName = _isOnDirt ? AudioSFXName.Run_Dirt : AudioSFXName.None;
        PlaySFX(audioName);
    }
    public void PlayCrouchSFX()
    {
        AudioSFXName audioName = _isOnDirt ? AudioSFXName.Crouch_Dirt : AudioSFXName.None;
        PlaySFX(audioName);
    }
    public void StopMovementTypeSFX()
    {
        StopSFX(AudioType.Movement);
    }
    #endregion

    #region Health SFX
    public void PlayPainGruntSFX()
    {
        int maxWhistleSFXCount = _audioSFXList.GetAudioSFXListCountPerType(AudioType.Pain);  

        int chosenCount = 0;
        if(maxWhistleSFXCount > 0) chosenCount = UnityEngine.Random.Range(1, maxWhistleSFXCount + 1);

        Debug.Log("Chosen whistle" + chosenCount);
        if(Enum.TryParse(AUDIOSFXNAME_FRONT_PAINGRUNTTYPE + chosenCount.ToString(), out AudioSFXName result))
        {
            PlaySFXOnce(result);
        }
    }
    #endregion

    #region UseWeapon SFX
    public virtual void PlayReloadSFX()
    {
        AudioSFXName audioName = _charaIdentity.GetMovementStateMachine.IsCrouching ? AudioSFXName.Reload_R_Crouch : AudioSFXName.Reload_R_Stand;

        PlaySFXOnce(audioName);
    }
    public virtual void PlayShootSFX()
    {
        PlaySFXOnce(AudioSFXName.Shoot_R_EnemyOnly);
    }
    #endregion

    #region overide
    protected override void SetAudioClipToSource(AudioSource source, AudioData audioData)
    {
        base.SetAudioClipToSource(source, audioData);

        AudioSFXData data = audioData as AudioSFXData;

        source.spatialBlend = data.spatialBlend;
        source.minDistance = data.minDistance;
        source.maxDistance = data.maxDistance;
    }
    #endregion

    protected void GameManager_OnPlayerPause(bool isPause)
    {
        //mungkin perlu dipertimbangkan kalo isplaying aja kali penandanya
        if(isPause) StopLoopAudioSourceWhenPause();
        TogglePauseAllAudioSource(isPause);
    }
    protected void TogglePauseAllAudioSource(bool isPause)
    {
        foreach(Audio audio in _audioList)
        {
            if(isPause)
            {
                if(audio.audioSource.isPlaying)
                {
                    audio.audioSource.Pause();
                    audio.isPause = true;
                }
            }
            else
            {
                if(audio.isPause)
                {
                    audio.audioSource.UnPause();
                    audio.isPause = false;
                }
            }
        }
    }

    public void UnsubscribeEvent()
    {
        _gm.OnPlayerPause -= GameManager_OnPlayerPause;
    }
}
