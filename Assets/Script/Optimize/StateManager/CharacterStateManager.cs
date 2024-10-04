using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterStateManager : BaseStateManager
{
    [SerializeField]protected Animator animator;
    private void Awake() 
    {
        animator = GetComponent<Animator>();
    }
}
