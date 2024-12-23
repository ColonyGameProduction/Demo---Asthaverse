using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableObj_IntObj : InteractableObject
{
    [SerializeField] protected float _totalValueToDoSomething;
    [SerializeField] protected float _valueFillSpeed;
    [ReadOnly(false), SerializeField] protected float _value = 0;
    [ReadOnly(false), SerializeField] protected bool _isBeingInteracted;
    [ReadOnly(false), SerializeField] protected bool _isComplete;

    protected PlayableCharacterIdentity _currCharaInteracting;
    public override bool CanInteract {get{ return !_isComplete;}}
    public Action<float> OnValueChange;
    protected virtual void Update() 
    {
        if(_isComplete) return;
        if(_isBeingInteracted)
        {
            if(_value < _totalValueToDoSomething)
            {
                _value += Time.deltaTime * _valueFillSpeed;
                OnValueChange?.Invoke(_value/_totalValueToDoSomething);
            }
            else
            {
                _isComplete = true; // Do something
                ResetInteraction();
                WhenComplete();
            }
        }
    }
    public override void Interact(PlayableCharacterIdentity characterIdentity)
    {
        _value = 0;

        _currCharaInteracting = characterIdentity;
        _currCharaInteracting.OnCancelInteractionButton += ResetInteraction;
        _currCharaInteracting.IsHoldingInteraction = true;
        HoldInteractUI.OnStartHoldInteracting?.Invoke(this);

        _isBeingInteracted = true;
    }

    protected virtual void WhenComplete()
    {
        Debug.Log("This thing is finish");
    }
    
    protected virtual void ResetInteraction()
    {
        _currCharaInteracting.OnCancelInteractionButton -= ResetInteraction;
        _currCharaInteracting.IsHoldingInteraction = false;
        HoldInteractUI.OnStopHoldInteracting?.Invoke(this);
        _currCharaInteracting = null;

        if(_isComplete) return;
        _isBeingInteracted = false;

        
    }

}
