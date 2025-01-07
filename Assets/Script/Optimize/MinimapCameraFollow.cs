using UnityEngine;

public class MinimapCameraFollow : MonoBehaviour, IUnsubscribeEvent
{
    [SerializeField] private PlayableCharacterManager _playableCharaManager;
    private float cameraHeight;
    [SerializeField] private Transform targetToFollow;
    [SerializeField] private bool rotateWithTheTarget = true;

    private void Awake()
    {
        cameraHeight = transform.position.y;
        if(_playableCharaManager != null) _playableCharaManager.OnPlayerSwitch += ChangeTargetToFollow;
    }

    private void Update()
    {
        FollowTarget();
    }
    private void ChangeTargetToFollow(Transform target)
    {
        PlayableCharacterIdentity chara = target.gameObject.GetComponent<PlayableCharacterIdentity>();
        targetToFollow = chara.GetPlayableMovementStateMachine.CharaGameObject;
    }
    private void FollowTarget()
    {
        Vector3 targetPosition = targetToFollow.position;
        
        transform.position = new Vector3(targetPosition.x, targetPosition.y + cameraHeight, targetPosition.z);

        if (rotateWithTheTarget)
        {
            Quaternion targetRotation = targetToFollow.transform.rotation;

            transform.rotation = Quaternion.Euler(90f, targetRotation.eulerAngles.y, 0f);
        }
    }

    public void UnsubscribeEvent()
    {
        _playableCharaManager.OnPlayerSwitch -= ChangeTargetToFollow;
    }
}
