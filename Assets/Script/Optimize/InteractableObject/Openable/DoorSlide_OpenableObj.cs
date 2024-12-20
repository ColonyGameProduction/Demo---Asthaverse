using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSlide_OpenableObj : Door_OpenableObj
{
    [SerializeField] private Vector3 _closeDoorPosition;
    [SerializeField] private Vector3 _openDoorPosition;
    protected override void ToggleOpenClose(Transform chara)
    {
        if(_isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
        base.ToggleOpenClose(chara);
    }
    protected void OpenDoor()
    {
        LeanTween.cancel(_openObj);
        LeanTween.moveLocal(_openObj, _openDoorPosition, _animationOpenTimer);
    }
    protected override void CloseDoor()
    {
        LeanTween.cancel(_openObj);
        LeanTween.moveLocal(_openObj, _closeDoorPosition, _animationOpenTimer);
    }
}
