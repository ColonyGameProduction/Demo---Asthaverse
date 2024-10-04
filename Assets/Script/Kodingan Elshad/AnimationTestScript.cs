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
        animator.SetFloat("Horizontal", Movement.x);
        animator.SetFloat("Vertical", Movement.y);
    }

    public void ShootAnimation()
    {

    }

}
