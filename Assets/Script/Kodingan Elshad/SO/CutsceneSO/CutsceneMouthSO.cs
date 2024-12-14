using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[CreateAssetMenu (fileName = "MouthAnimation", menuName = "ScriptableObject/MouthAnimationSO")]
public class CutsceneMouthSO : ScriptableObject
{
    public string Name;
    public Sprite OpenMouth;
    public Sprite CloseMouth;
    public VideoClip MouthAnimation;

    public Sprite cropFace;
}
