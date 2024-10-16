using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterStateMachine : BaseStateMachine
{
    [SerializeField]protected Animator _animator;
    public Animator CharaAnimator {get {return _animator;}}

    protected bool _isInputPlayer;
    public bool IsInputPlayer {get {return _isInputPlayer;}}

    protected override void Awake() 
    {
        base.Awake();
        if(_animator == null)_animator = GetComponent<Animator>();
    }
}
