using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterStateMachine : BaseStateMachine
{
    [SerializeField]protected Animator _animator;
    public Animator CharaAnimator {get {return _animator;}}


    protected virtual void Awake() 
    {
        if(_animator == null)_animator = GetComponent<Animator>();
    }
}
