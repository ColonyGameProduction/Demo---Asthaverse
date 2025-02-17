
using System.Collections.Generic;
using UnityEngine;

public class PlayableInteraction : MonoBehaviour
{
    [SerializeField] private GameManager _gm;
    [SerializeField] private List<IInteractable> _interactablesList = new List<IInteractable>();
    [SerializeField] private List<ISilentKillAble> _silentKillAbleList = new List<ISilentKillAble>();
    [SerializeField] private IInteractable _currInteractable, _currInteractableRaycast;
    [SerializeField] private ISilentKillAble _currSilentKillAble;
    [SerializeField] private LayerMask _interactableLayerMask;
    [SerializeField] private float _interactDistances = 7.5f;

    [Space(1)]
    [Header("TakeItem")]
    [SerializeField]protected Transform _pickableObjPoint;
    [SerializeField]protected float _throwForceMultiplier = 20f;
    protected PickableObj_IntObj _heldObject, _tempHeldObj;
    protected bool _isAnimatingPickUpItemInteraction_PickUp, _isAnimatingPickUpItemInteraction_Throw;
    private int _leanTweenID;



    private IInteractable _thisObjInteractable;
    private PlayableCharacterIdentity _playableCharacterIdentity;
    private Transform _originInteract, _directionInteract;
    protected Camera _mainCamera;


    public IInteractable CurrInteractable{get{return _currInteractable;}}
    public ISilentKillAble CurrSilentKillAble{get{return _currSilentKillAble;}}
    public bool IsHeldingObject {get{return _heldObject != null? true : false;}}
    public bool IsAnimatingPickingUpItem {get {return _isAnimatingPickUpItemInteraction_PickUp;}}
    public bool IsAnimatingThrowingItem {get {return _isAnimatingPickUpItemInteraction_Throw;} set {_isAnimatingPickUpItemInteraction_Throw = value;}}
    private void Awake() 
    {
        _thisObjInteractable = GetComponentInParent<IInteractable>();
        _playableCharacterIdentity = GetComponentInParent<PlayableCharacterIdentity>();
        _mainCamera = Camera.main;
        _originInteract = _mainCamera.transform;
        _directionInteract = _mainCamera.transform;
    }

    private void Start() 
    {
        _gm = GameManager.instance;
    }
    private void Update() 
    {
        if(!_gm.IsGamePlaying()) return;
        if(!_playableCharacterIdentity.IsPlayerInput)return;
        _currInteractable = GetClosestInteractables();
        SearchRaycastInteractable();
        if(_playableCharacterIdentity.IsHoldingInteraction)
        {
            KeybindUIHandler.OnHideInteractKeybind();
        }
        else
        {
            if(_currInteractable != null)
            {
                KeybindUIHandler.OnShowInteractKeybind(InteractObjType.Revive);
            }
            else
            {
                if(_currInteractableRaycast != null)
                {  
                    KeybindUIHandler.OnShowInteractKeybind(_currInteractableRaycast.InteractObjType);   
                    
                    
                }
                else
                {
                    KeybindUIHandler.OnHideInteractKeybind();
                }
            }
        }

        _currSilentKillAble = GetClosestSilentkillable();
        KeybindUIHandler.OnShowSilentTakeDownKeybind(_currSilentKillAble == null ? false : true);

    }
    #region  Make Sound

    #endregion
    #region  Get Interactable Through OnTrigger
    public void Interact() //interact revive ama item samakan
    {
        if(_currInteractable != null)
        {
            if(_interactablesList.Contains(_currInteractable))
            {
                _interactablesList.Remove(_currInteractable);
            }
            _currInteractable.Interact(_playableCharacterIdentity);
        }
        else
        {
            if(_currInteractableRaycast != null)
            {
                if(_currInteractableRaycast == _thisObjInteractable) return;
                _currInteractableRaycast.Interact(_playableCharacterIdentity);
            }
            else
            {
                if(IsHeldingObject) RemoveHeldObject();
            }
        }

        
    }
    public void SilentKill()
    {
        // Debug.Log("Duarr");
        if(_currSilentKillAble != null)
        {
            // Debug.Log("Duarrss");
            if(_silentKillAbleList.Contains(_currSilentKillAble))
            {
                _silentKillAbleList.Remove(_currSilentKillAble);
            }
            _currSilentKillAble.GotSilentKill(_playableCharacterIdentity);
        }
    }
    private IInteractable GetClosestInteractables()
    {
        if(_interactablesList.Count == 0)return null;
        float closestDistance = Mathf.Infinity;
        // float smallestDotProduct = -1;
        IInteractable chosenInteractable = null;
        IInteractable removeInteractable = null;
        foreach(IInteractable interactable in _interactablesList)
        {
            if(interactable == null || !interactable.CanInteract)
            {
                if(!interactable.CanInteract) removeInteractable = interactable;
                continue;
            }
            float InteractableToPlayerDistance = Vector3.Distance(interactable.GetInteractableTransform.position, _thisObjInteractable.GetInteractableTransform.position);
            

            if(closestDistance > InteractableToPlayerDistance)
            {
                closestDistance = InteractableToPlayerDistance;

                chosenInteractable = interactable;
            }
        }

        if(removeInteractable != null)
        {
            _interactablesList.Remove(removeInteractable);
            removeInteractable = null;
        }
        
        return chosenInteractable;
    }
    private ISilentKillAble GetClosestSilentkillable()
    {
        if(_silentKillAbleList.Count == 0 || _playableCharacterIdentity.IsSilentKilling || _playableCharacterIdentity.GetFriendAIStateMachine.GotDetectedbyEnemy)return null;
        float closestDistance = Mathf.Infinity;
        // float smallestDotProduct = -1;
        ISilentKillAble chosen = null;

        foreach(ISilentKillAble interactable in _silentKillAbleList)
        {
            if(interactable == null)continue;
            float InteractableToPlayerDistance = Vector3.Distance(interactable.GetSilentKillAbleTransform.position, _thisObjInteractable.GetInteractableTransform.position);
            
            if(closestDistance > InteractableToPlayerDistance)
            {
                closestDistance = InteractableToPlayerDistance;

                chosen = interactable;
            }
        }
        return chosen;
    }
    private void SearchRaycastInteractable()
    {
        if(Physics.Raycast(_originInteract.position, _directionInteract.forward.normalized, out RaycastHit hit, _interactDistances, _interactableLayerMask))
        {
            IInteractable temp = hit.collider.GetComponent<IInteractable>();
            IInteractable interactable =  temp != null ? temp : hit.collider.GetComponentInParent<IInteractable>();
            
            if(interactable != null && !_interactablesList.Contains(interactable) && interactable.CanInteract)
            {
                // if(interactable == _thisObjInteractable) return;
                // interactable.Interact(_playableCharacterIdentity);

                _currInteractableRaycast = interactable;
            }
        }
        else
        {
            _currInteractableRaycast = null;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.CompareTag("Interactable"))
        {
            IInteractable temp = other.gameObject.GetComponent<IInteractable>();
            IInteractable interactable =  temp != null ? temp : other.gameObject.GetComponentInParent<IInteractable>();

            // if(interactable != null)Debug.Log(interactable.InteractableTransform.name + " in1");
            if(interactable != null && !_interactablesList.Contains(interactable) && interactable.CanInteract)
            {
                if(interactable == _thisObjInteractable) return;
                // Debug.Log(interactable.InteractableTransform.name + " in");
                _interactablesList.Add(interactable);
            }

            
        }
        if(other.gameObject.CompareTag("Enemy"))
        {
            ISilentKillAble temp = other.gameObject.GetComponent<ISilentKillAble>();
            ISilentKillAble silentKillAble =  temp != null ? temp : other.gameObject.GetComponentInParent<ISilentKillAble>();

            EnemyIdentity enemyIdentity = other.gameObject.GetComponent<EnemyIdentity>();
            enemyIdentity = enemyIdentity != null ? enemyIdentity : other.gameObject.GetComponentInParent<EnemyIdentity>();

            if(enemyIdentity != null && enemyIdentity.IsDead) return;

            // if(silentKillAble != null)Debug.Log(silentKillAble.SilentKillAbleTransform.name + " in1");
            if(silentKillAble != null && !_silentKillAbleList.Contains(silentKillAble) && silentKillAble.CanBeKill)
            {
                _silentKillAbleList.Add(silentKillAble);
                // Debug.Log(silentKillAble.SilentKillAbleTransform.name + " in2");
            }
        }
        
    }
    private void OnTriggerStay(Collider other) 
    {
        if(other.gameObject.CompareTag("Interactable"))
        {
            IInteractable temp = other.gameObject.GetComponent<IInteractable>();
            IInteractable interactable =  temp != null ? temp : other.gameObject.GetComponentInParent<IInteractable>();

            // if(interactable != null)Debug.Log(interactable.InteractableTransform.name + " out1");
            if(interactable != null && _interactablesList.Contains(interactable))
            {
                // Debug.Log(interactable.InteractableTransform.name + " out");
                if(!interactable.CanInteract)_interactablesList.Remove(interactable);
            }
            else if(interactable != null && !_interactablesList.Contains(interactable) && interactable.CanInteract)
            {
                if(interactable == _thisObjInteractable) return;
                // Debug.Log(interactable.InteractableTransform.name + " in");
                _interactablesList.Add(interactable);
            }

            
        }
        if(other.gameObject.CompareTag("Enemy"))
        {
            ISilentKillAble temp = other.gameObject.GetComponent<ISilentKillAble>();
            ISilentKillAble silentKillAble =  temp != null ? temp : other.gameObject.GetComponentInParent<ISilentKillAble>();
            if(silentKillAble != null && _silentKillAbleList.Contains(silentKillAble))
            {
                if(!silentKillAble.CanBeKill)_silentKillAbleList.Remove(silentKillAble);
            }
            else
            {
                EnemyIdentity enemyIdentity = other.gameObject.GetComponent<EnemyIdentity>();
                enemyIdentity = enemyIdentity != null ? enemyIdentity : other.gameObject.GetComponentInParent<EnemyIdentity>();

                if(enemyIdentity != null && enemyIdentity.IsDead) return;

                // if(silentKillAble != null)Debug.Log(silentKillAble.SilentKillAbleTransform.name + " in1");
                if(silentKillAble != null && !_silentKillAbleList.Contains(silentKillAble) && silentKillAble.CanBeKill)
                {
                    _silentKillAbleList.Add(silentKillAble);
                    // Debug.Log(silentKillAble.SilentKillAbleTransform.name + " in2");
                }
            }
        }
    }
    private void OnTriggerExit(Collider other) 
    {
        if(other.gameObject.CompareTag("Interactable"))
        {
            IInteractable temp = other.gameObject.GetComponent<IInteractable>();
            IInteractable interactable =  temp != null ? temp : other.gameObject.GetComponentInParent<IInteractable>();
            
            if(interactable != null && _interactablesList.Contains(interactable))
            {
                // Debug.Log(interactable.InteractableTransform.name + " out");
                _interactablesList.Remove(interactable);
            }
            
        }
        if(other.gameObject.CompareTag("Enemy"))
        {
            ISilentKillAble temp = other.gameObject.GetComponent<ISilentKillAble>();
            ISilentKillAble silentKillAble =  temp != null ? temp : other.gameObject.GetComponentInParent<ISilentKillAble>();
            // if(silentKillAble != null)Debug.Log(silentKillAble.SilentKillAbleTransform.name + " out1");
            if(silentKillAble != null && _silentKillAbleList.Contains(silentKillAble))
            {
                _silentKillAbleList.Remove(silentKillAble);
                // Debug.Log(silentKillAble.SilentKillAbleTransform.name + " out2");
            }
        }
    }
    public void DeleteKilledEnemyFromList(Transform enemy)
    {
        ISilentKillAble temp = enemy.GetComponent<ISilentKillAble>();
        ISilentKillAble silentKillAble =  temp != null ? temp : enemy.GetComponentInParent<ISilentKillAble>();
        
        if(_silentKillAbleList.Contains(silentKillAble))_silentKillAbleList.Remove(silentKillAble);
    }
    #endregion

    #region PickableObj
    public void PickUpObject(PickableObj_IntObj obj)
    {
        if(_heldObject == obj || IsAnimatingThrowingItem)return;
        _playableCharacterIdentity.GetPlayableUseWeaponStateMachine.TellToTurnOffScope();
        _playableCharacterIdentity.GetPlayableUseWeaponStateMachine.ForceStopUseWeapon();
        _playableCharacterIdentity.GetPlayableMovementStateMachine.IsMustLookForward = true; //ini digantiu
        
        _tempHeldObj = obj;
        _isAnimatingPickUpItemInteraction_PickUp = true;

        _leanTweenID = LeanTween.value(1, 0, 0.1f).setOnUpdate((float value) =>
        {
            _playableCharacterIdentity.ChangeIsHoldingItemAnimationValue(value);
        }).id;

        _playableCharacterIdentity.DoPickUpInteractionAnimation();
    }
    public void AddHeldObject()
    {
        if(_tempHeldObj == null || _playableCharacterIdentity.IsDead) return;

        if(_heldObject != null)
        {
            _heldObject.RB.isKinematic = false;
            _heldObject.transform.position = _tempHeldObj.transform.position;
            _heldObject.transform.rotation = _tempHeldObj.transform.rotation;
            _heldObject.transform.SetParent(null);

            _heldObject.StartSparkleParticle();
        }
        
        _heldObject = _tempHeldObj;
        _heldObject.RB.isKinematic = true;
        _heldObject.transform.position = _pickableObjPoint.position;
        _heldObject.transform.rotation = _pickableObjPoint.rotation;
        _heldObject.transform.SetParent(_pickableObjPoint);

        _heldObject.StopSparkleParticle();

        LeanTween.cancel(_leanTweenID);
        _playableCharacterIdentity.ChangeIsHoldingItemAnimationValue(1);
        _isAnimatingPickUpItemInteraction_PickUp = false;
        _tempHeldObj = null;

        _playableCharacterIdentity.StopPickUpInteractionAnimation();
    }
    public void RemoveHeldObject()
    {
        _tempHeldObj = null;
        _isAnimatingPickUpItemInteraction_PickUp = false;
        _isAnimatingPickUpItemInteraction_Throw = false;
        LeanTween.cancel(_leanTweenID);

        if(_heldObject == null)return;
        if(_heldObject != null)
        {
            _playableCharacterIdentity.GetPlayableMovementStateMachine.IsMustLookForward = false;
            _heldObject.RB.isKinematic = false;
            _heldObject.transform.SetParent(null);
            _heldObject.StartSparkleParticle();
            
            _heldObject = null;

            KeybindUIHandler.OnChangeKeybind?.Invoke(KeybindUIType.General);
        }
        _playableCharacterIdentity.StopPickUpInteractionAnimation();
        _playableCharacterIdentity.ChangeIsHoldingItemAnimationValue(0);
    }
    public void ThrowObject()
    {
        if(_heldObject == null || _isAnimatingPickUpItemInteraction_PickUp)return;
        if(_heldObject.IsThrowAble)
        {
            IsAnimatingThrowingItem = true;
            _playableCharacterIdentity.DoThrowInteractionAnimation();
        }
        else
        {
            RemoveHeldObject();
        }
    }
    public void ThrowHeldObject()
    {
        _playableCharacterIdentity.GetPlayableMovementStateMachine.IsMustLookForward = false;
        _heldObject.RB.isKinematic = false;
        _heldObject.transform.SetParent(null);
        _heldObject.IsBeingThrown = true;
        _heldObject.RB.AddForce(_mainCamera.transform.forward.normalized * _throwForceMultiplier, ForceMode.VelocityChange);
        _heldObject = null;

        _playableCharacterIdentity.StopThrowInteractionAnimation();
        _playableCharacterIdentity.ChangeIsHoldingItemAnimationValue(0);
        KeybindUIHandler.OnChangeKeybind?.Invoke(KeybindUIType.General);
    }
    #endregion

}
