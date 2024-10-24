using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorCharacterHelp : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private UseWeaponStateMachine _useWeaponStateMachine;
    private void Awake() 
    {
        animator = GetComponent<Animator>();
        _useWeaponStateMachine = GetComponentInParent<UseWeaponStateMachine>();
    }

    public void ReloadWeaponFinish()
    {
        _useWeaponStateMachine.ReloadAnimationFinished();
    }
}
