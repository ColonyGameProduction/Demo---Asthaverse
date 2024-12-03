using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorCharacterHelp : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterIdentity _characterIdentity;
    [SerializeField] private PlayableCharacterIdentity _playableCharaIdentity;
    [SerializeField] private UseWeaponStateMachine _useWeaponStateMachine;
    [Header("Foot")]
    [SerializeField][Range(0, 1f)] private float _distanceToGround;
    [SerializeField] private LayerMask _placeToWalkLayer;
    private bool wasDeath;
    private void Awake() 
    {
        _animator = GetComponent<Animator>();
        _useWeaponStateMachine = GetComponentInParent<UseWeaponStateMachine>();
        _characterIdentity = GetComponentInParent<CharacterIdentity>();
        _playableCharaIdentity = _characterIdentity as PlayableCharacterIdentity;
    }

    public void ReloadWeaponFinish()
    {
        _useWeaponStateMachine.ReloadAnimationFinished();
    }
    public void AfterDeathAnim()
    {
        wasDeath = true;
        _characterIdentity.AfterFinishDeathAnimation();
    }
    public void StartKnockOutAnim()
    {
        if(wasDeath)
        {
            wasDeath = false;
            _playableCharaIdentity.AfterFinishKnockOutAnimation();
        }
    }
    public void OnAnimatorIK(int layerIndex) 
    {
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
        _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);

        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
        _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
        

        Ray rayLeftFoot = new Ray(_animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
        if(Physics.Raycast(rayLeftFoot, out RaycastHit hitLeft, _distanceToGround + 1f, _placeToWalkLayer))
        {
            if(hitLeft.transform.CompareTag("Ground"))
            {
                Vector3 newFootPos = hitLeft.point;
                newFootPos.y += _distanceToGround;
                _animator.SetIKPosition(AvatarIKGoal.LeftFoot, newFootPos);
                
                Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hitLeft.normal);
                _animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(forward,hitLeft.normal)); 
            }
        }

        Ray rayRightFoot = new Ray(_animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
        if(Physics.Raycast(rayRightFoot, out RaycastHit hitRight, _distanceToGround + 1f, _placeToWalkLayer))
        {
            if(hitRight.transform.CompareTag("Ground"))
            {
                Vector3 newFootPos = hitRight.point;
                newFootPos.y += _distanceToGround;
                _animator.SetIKPosition(AvatarIKGoal.RightFoot, newFootPos);
                
                Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hitRight.normal);
                _animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(forward,hitRight.normal));
            }
        }
    }
}
