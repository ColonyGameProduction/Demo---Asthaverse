using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public interface IAnimator
{
    public void ShootAnimation();
    public void WalkAnimation();

}
public class AnimationTestScript : MonoBehaviour
{
    [SerializeField]
    public Animator animator;

    public void WalkAnimation(Vector2 Movement)
    {
        Debug.Log(Movement);
        if (Movement.x == 0 && Movement.y > 0)
        {
            animator.SetBool("Move", true);
            animator.Play("Armature_Rifle_Sprint_Front");
        }
        else if (Movement.x < 0 && Movement.y == 0)
        {
            animator.SetBool("Move", true);
            animator.Play("Armature_Rifle_Run_Left");
        }
        else if (Movement.x > 0 && Movement.y == 0)
        {
            animator.SetBool("Move", true);
            animator.Play("Armature_Rifle_Run_Right");
        }
        else if(Movement.x == 0 && Movement.y < 0)
        {
            animator.SetBool("Move", true);
            animator.Play("Armature_Rifle_Run_Back");
        }
        else if(Movement == Vector2.zero)
        {
            animator.SetBool("Move", false);
        }
    }

    public void ShootAnimation()
    {

    }

}
