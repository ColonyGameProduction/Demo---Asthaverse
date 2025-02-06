
using Unity.VisualScripting;
using UnityEngine;

public class PickableObj_IntObj : InteractableObject
{
    [ReadOnly(false), SerializeField] protected bool _canInteract = true;
    protected Rigidbody _rb;
    [SerializeField]protected bool _isThrowAble = true;
    protected bool _isBeingThrown;
    protected PlayableCharacterIdentity _playerWhoHeldItem;
    [SerializeField] protected ParticleSystem _sparkleParticle;
    public Rigidbody RB {get{return _rb;}}
    public bool IsThrowAble {get{return _isThrowAble;}}
    public bool IsBeingThrown {get{return _isBeingThrown;} set{_isBeingThrown = value;}}
    public override bool CanInteract {get{ return _canInteract;}}
    protected virtual void Awake()  
    {
        _rb = GetComponent<Rigidbody>();
        _sparkleParticle = GetComponentInChildren<ParticleSystem>();
        StartSparkleParticle();
    }
    public override void Interact(PlayableCharacterIdentity characterIdentity)
    {
        _playerWhoHeldItem = characterIdentity;
        IsBeingThrown = false;
        characterIdentity.GetPlayableInteraction.PickUpObject(this);

        base.Interact(characterIdentity);
        KeybindUIHandler.OnChangeKeybind?.Invoke(KeybindUIType.PickUp);
    }
    private void OnCollisionEnter(Collision other) 
    {
        if(!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Weapon"))
        {
            // Debug.Log("I make sound using" + other.gameObject);
            if(IsBeingThrown)
            {
                // Debug.Log("I make sound");
                _canInteract = false;
                IsBeingThrown = false;
                WorldSoundManager.Instance.MakeSound(WorldSoundName.Bottle, transform.position, _playerWhoHeldItem.GetFOVMachine.CharaEnemyMask);

                AudioManager.Instance.PlayAudioClip(AudioSFXName.BrokeThrowItem_GlassBottle, transform.position);

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

    public void StartSparkleParticle()
    {
        _sparkleParticle.Play();
    }
    public void StopSparkleParticle()
    {
        _sparkleParticle.Stop();
    }
}
