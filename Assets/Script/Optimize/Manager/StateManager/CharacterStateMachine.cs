using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterStateMachine : BaseStateMachine
{
    protected Animator _animator;
    protected CharacterMakeSFX _charaMakeSFX;
    public Animator CharaAnimator {get {return _animator;}}
    public CharacterMakeSFX GetCharaMakeSFX {get{return _charaMakeSFX;}}

    protected bool _isAIInput = true;
    public bool IsAIInput {get {return _isAIInput;}}

    protected override void Awake() 
    {
        base.Awake();
        if(_animator == null)_animator = GetComponentInChildren<Animator>();
        _charaMakeSFX = GetComponentInChildren<CharacterMakeSFX>();
    }
}
