using System;

using UnityEngine;

public class HoldableObj_IntObj : InteractableObject, IUnsubscribeEvent, IConnectToQuest
{
    [SerializeField] protected float _totalValueToDoSomething;
    [SerializeField] protected float _valueFillSpeed;
    [ReadOnly(false), SerializeField] protected float _value = 0;
    [ReadOnly(false), SerializeField] protected bool _isBeingInteracted;
    [ReadOnly(false), SerializeField] protected bool _isComplete;

    protected PlayableCharacterIdentity _currCharaInteracting;
    [SerializeField] protected AudioSource _completedAudioSource;
    public override bool CanInteract {get{ return !_isComplete;}}
    public Action<float> OnValueChange;
    public event Action OnTriggerQuestComplete;
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
        characterIdentity.DoNormalInteractionAnimation();
        HoldInteractUI.OnStartHoldInteracting?.Invoke(this);

        base.Interact(characterIdentity);
        _isBeingInteracted = true;
    }
    public virtual void WhenQuestActivated()
    {
        
    }
    protected virtual void WhenComplete()
    {
        OnTriggerQuestComplete?.Invoke();
    }
    
    protected virtual void ResetInteraction()
    {
        ToggleInteractableObjAudio(_interactSFXAudioSource, false);

        _currCharaInteracting.OnCancelInteractionButton -= ResetInteraction;
        _currCharaInteracting.IsHoldingInteraction = false;
        _currCharaInteracting.StopNormalInteractionAnimation();
        HoldInteractUI.OnStopHoldInteracting?.Invoke(this);
        _currCharaInteracting = null;

        if(_isComplete)
        {
            ToggleInteractableObjAudio(_completedAudioSource, true);
            return;
        }
        _isBeingInteracted = false;

        
    }

    public void UnsubscribeEvent()
    {

        if(_currCharaInteracting) _currCharaInteracting.OnCancelInteractionButton -= ResetInteraction;
    }
}
