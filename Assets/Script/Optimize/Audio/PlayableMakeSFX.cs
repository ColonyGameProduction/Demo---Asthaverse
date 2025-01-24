using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayableMakeSFX : CharacterMakeSFX
{
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
