using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObj_IntObj : InteractableObject
{
    protected bool _canInteract = true;
    protected Rigidbody _rb;
    [SerializeField]protected bool _isThrowAble = true;
    protected bool _isBeingThrown;
    protected PlayableCharacterIdentity _playerWhoHeldItem;
    public Rigidbody RB {get{return _rb;}}
    public bool IsThrowAble {get{return _isThrowAble;}}
    public bool IsBeingThrown {get{return _isBeingThrown;} set{_isBeingThrown = value;}}
    public override bool CanInteract {get{ return _canInteract;}}
    private void Awake()  
    {
        _rb = GetComponent<Rigidbody>();
    }
    public override void Interact(PlayableCharacterIdentity characterIdentity)
    {
        _playerWhoHeldItem = characterIdentity;
        IsBeingThrown = false;
        characterIdentity.GetPlayableInteraction.PickUpObject(this);
    }
    private void OnCollisionEnter(Collision other) 
    {
        if(!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Weapon"))
        {
            Debug.Log("I make sound using" + other.gameObject);
            if(IsBeingThrown)
            {
                Debug.Log("I make sound");
                IsBeingThrown = false;
                WorldSoundManager.Instance.MakeSound(WorldSoundName.Whistle, _playerWhoHeldItem.transform.position, _playerWhoHeldItem.GetFOVMachine.CharaEnemyMask);
                AudioManager.Instance.PlayAudioClip(AudioSFXName.Whistle, transform.position);
                _playerWhoHeldItem = null;
                Destroy(this.gameObject);
                // Debug.Log(gameObject + "Object destroy");
            }
        }
        else
        {
            // IsBeingThrown = false;
        }
        
    }
}
