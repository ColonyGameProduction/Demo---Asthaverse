
using UnityEngine;

public class DoorRotate_OpenableObj : Door_OpenableObj
{
    protected bool _isCharaInfront;
    [SerializeField] private Vector3 _closeDoorRotatePos;
    [SerializeField] private Vector3 _openDoorRotatePos_Front, _openDoorRotatePos_Behind;

    protected override void ToggleOpenClose(Transform chara)
    {
        if(_isOpen)
        {
            CloseDoor();
        }
        else
        {
            Vector3 direction = transform.position - chara.position;
            float dot = Vector3.Dot(transform.forward, direction.normalized); 

            OpenDoor(dot);
        }
        base.ToggleOpenClose(chara);
    }
    private void OpenDoor(float dot)
    {
        if(dot > 0)
        {
            _isCharaInfront = true;
        }
        else
        {
            _isCharaInfront = false;
        }

        if(_isCharaInfront)
        {
            LeanTween.cancel(_openObj);
            LeanTween.rotate(_openObj, -_openDoorRotatePos_Front, _animationOpenTimer);
        }
        else
        {
            LeanTween.cancel(_openObj);
            LeanTween.rotate(_openObj, _openDoorRotatePos_Behind, _animationOpenTimer);
        }
    }
    protected override void CloseDoor()
    {
        LeanTween.cancel(_openObj);
        LeanTween.rotate(_openObj, _closeDoorRotatePos, _animationOpenTimer);
    }

}
