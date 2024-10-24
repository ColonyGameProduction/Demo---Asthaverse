using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadBehaviour : StateMachineBehaviour
{
    public string animationClipName;
    // public float time = 0.2f;
    private UseWeaponStateMachine _useWeaponStateMachine;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_useWeaponStateMachine == null) _useWeaponStateMachine = animator.gameObject.GetComponentInParent<UseWeaponStateMachine>();
        if(animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime != _useWeaponStateMachine.CurrAnimTime)
        {
            animator.Play(animationClipName, layerIndex, _useWeaponStateMachine.CurrAnimTime);
            // Debug.Log("lewat siuni sekali");
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _useWeaponStateMachine.CurrAnimTime = animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;
        // Debug.Log(_useWeaponStateMachine.CurrAnimTime + " s");
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    // override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {

    // }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
