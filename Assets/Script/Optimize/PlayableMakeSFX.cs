using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayableMakeSFX : AudioHandler
{
    [Header("Walk SFX")]
    private bool _isOnDirt = true;
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
        AudioSFXName audioName = _isOnDirt ? AudioSFXName.Run_Dirt : AudioSFXName.None;
        PlaySFX(audioName);
    }
    public void StopMovementTypeSFX()
    {
        StopSFX(AudioType.Movement);
    }

    public void PlayWhistleSFX()
    {
        PlaySFXOnce(AudioSFXName.Whistle);
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
