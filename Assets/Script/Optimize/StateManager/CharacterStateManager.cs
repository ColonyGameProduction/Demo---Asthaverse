using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterStateManager : BaseStateManager
{
    [SerializeField]protected Animator _animator;
    public Animator CharaAnimator {get {return _animator;}}


    protected virtual void Awake() 
    {
        _animator = GetComponent<Animator>();
    }
}
