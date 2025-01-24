using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMakeSFX : AudioHandler
{
    [Header("Walk SFX")]
    protected bool _isOnDirt = true;
    public bool IsOnDirt 
    {
        set
        {
            _isOnDirt = value;
        }
    }

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
}
