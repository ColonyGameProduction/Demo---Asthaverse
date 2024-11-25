using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableInteraction : MonoBehaviour
{

    [SerializeField] private List<IInteractable> _interactablesList = new List<IInteractable>();
    [SerializeField] private List<ISilentKillAble> _silentKillAbleList = new List<ISilentKillAble>();
    [SerializeField] private GameObject _charaGameObject;
    [SerializeField] private IInteractable _currInteractable;
    [SerializeField] private ISilentKillAble _currSilentKillAble;
    [SerializeField] private LayerMask _interactableLayerMask;

    [Space(1)]
    [Header("TakeItem")]
    [SerializeField]protected Transform _pickableObjPoint;
    [SerializeField]protected float _throwForceMultiplier = 20f;
    protected PickableObj_IntObj _heldObject;



    private IInteractable _thisObjInteractable;
    private PlayableCharacterIdentity _playableCharacterIdentity;
    private Transform _originInteract, _directionInteract;
    protected Camera _mainCamera;


    public IInteractable CurrInteractable{get{return _currInteractable;}}
    public ISilentKillAble CurrSilentKillAble{get{return _currSilentKillAble;}}
    public bool IsHeldingObject {get{return _heldObject != null? true : false;}}
    private void Awake() 
    {
        _thisObjInteractable = GetComponentInParent<IInteractable>();
        _playableCharacterIdentity = GetComponentInParent<PlayableCharacterIdentity>();
        _mainCamera = Camera.main;
        _originInteract = _mainCamera.transform;
        _directionInteract = _mainCamera.transform;
    }

    private void Update() 
    {
        if(!_playableCharacterIdentity.IsPlayerInput)return;
        _currInteractable = GetClosestInteractables();

        _currSilentKillAble = GetClosestSilentkillable();
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
            // Debug.DrawRay(_originInteract.position, _directionInteract.forward.normalized * 100f, Color.magenta, 2f);
            if(Physics.Raycast(_originInteract.position, _directionInteract.forward.normalized, out RaycastHit hit, 100f, _interactableLayerMask))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>() ?? hit.collider.GetComponentInParent<IInteractable>();
                // Debug.Log("Interaaaact is here" + interactable + "wooo");
                if(interactable != null && !_interactablesList.Contains(interactable) && interactable.CanInteract)
                {
                    if(interactable == _thisObjInteractable) return;
                    // Debug.Log(interactable.InteractableTransform.name + " in");
                    interactable.Interact(_playableCharacterIdentity);
                }
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
        foreach(IInteractable interactable in _interactablesList)
        {
            if(interactable == null)continue;
            float InteractableToPlayerDistance = Vector3.Distance(interactable.InteractableTransform.position, _thisObjInteractable.InteractableTransform.position);
            

            if(closestDistance > InteractableToPlayerDistance)
            {
                closestDistance = InteractableToPlayerDistance;

                chosenInteractable = interactable;
            }
        }
        return chosenInteractable;
    }
    private ISilentKillAble GetClosestSilentkillable()
    {
        if(_silentKillAbleList.Count == 0)return null;
        float closestDistance = Mathf.Infinity;
        // float smallestDotProduct = -1;
        ISilentKillAble chosen = null;
        foreach(ISilentKillAble interactable in _silentKillAbleList)
        {
            if(interactable == null)continue;
            float InteractableToPlayerDistance = Vector3.Distance(interactable.SilentKillAbleTransform.position, _thisObjInteractable.InteractableTransform.position);
            
            if(closestDistance > InteractableToPlayerDistance)
            {
                closestDistance = InteractableToPlayerDistance;

                chosen = interactable;
            }
        }
        return chosen;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.CompareTag("Interactable"))
        {
            IInteractable interactable = other.gameObject.GetComponent<IInteractable>() ?? other.gameObject.GetComponentInParent<IInteractable>();
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
            Debug.Log("Hellooo");
            ISilentKillAble silentKillAble = other.gameObject.GetComponent<ISilentKillAble>() ?? other.gameObject.GetComponentInParent<ISilentKillAble>();
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
            IInteractable interactable = other.gameObject.GetComponent<IInteractable>() ?? other.gameObject.GetComponentInParent<IInteractable>();
            // if(interactable != null)Debug.Log(interactable.InteractableTransform.name + " out1");
            if(interactable != null && _interactablesList.Contains(interactable))
            {
                // Debug.Log(interactable.InteractableTransform.name + " out");
                if(!interactable.CanInteract)_interactablesList.Remove(interactable);
            }

            
        }
        if(other.gameObject.CompareTag("Enemy"))
        {
            ISilentKillAble silentKillAble = other.gameObject.GetComponent<ISilentKillAble>() ?? other.gameObject.GetComponentInParent<ISilentKillAble>();
            if(silentKillAble != null && _silentKillAbleList.Contains(silentKillAble))
            {
                if(!silentKillAble.CanBeKill)_silentKillAbleList.Remove(silentKillAble);
            }
        }
    }
    private void OnTriggerExit(Collider other) 
    {
        if(other.gameObject.CompareTag("Interactable"))
        {
            IInteractable interactable = other.gameObject.GetComponent<IInteractable>() ?? other.gameObject.GetComponentInParent<IInteractable>();
            // if(interactable != null)Debug.Log(interactable.InteractableTransform.name + " out1");
            if(interactable != null && _interactablesList.Contains(interactable))
            {
                // Debug.Log(interactable.InteractableTransform.name + " out");
                _interactablesList.Remove(interactable);
            }
            
        }
        if(other.gameObject.CompareTag("Enemy"))
        {
            ISilentKillAble silentKillAble = other.gameObject.GetComponent<ISilentKillAble>() ?? other.gameObject.GetComponentInParent<ISilentKillAble>();
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
        ISilentKillAble silentKillAble = enemy.GetComponent<ISilentKillAble>() ?? enemy.GetComponentInParent<ISilentKillAble>();
        if(_silentKillAbleList.Contains(silentKillAble))_silentKillAbleList.Remove(silentKillAble);
    }
    #endregion

    #region PickableObj
    public void PickUpObject(PickableObj_IntObj obj)
    {
        if(_heldObject == obj)return;
        _playableCharacterIdentity.GetPlayableUseWeaponData.TellToTurnOffScope();
        _playableCharacterIdentity.GetPlayableUseWeaponData.ForceStopUseWeapon();
        _playableCharacterIdentity.GetPlayableMovementData.IsMustLookForward = false;
        if(_heldObject != null)
        {
            _heldObject.RB.isKinematic = false;
            _heldObject.transform.position = obj.transform.position;
            _heldObject.transform.rotation = obj.transform.rotation;
            _heldObject.transform.SetParent(null);
        }
        
        _heldObject = obj;
        obj.RB.isKinematic = true;
        obj.transform.position = _pickableObjPoint.position;
        obj.transform.rotation = _pickableObjPoint.rotation;
        obj.transform.SetParent(_pickableObjPoint);
    }
    public void RemoveHeldObject()
    {
        if(_heldObject == null)return;
        if(_heldObject != null)
        {
            _heldObject.RB.isKinematic = false;
            _heldObject.transform.SetParent(null);
            _heldObject = null;
        }
    }
    public void ThrowHeldObject()
    {
        if(_heldObject == null)return;
        if(_heldObject.IsThrowAble)
        {
            _heldObject.RB.isKinematic = false;
            _heldObject.transform.SetParent(null);
            _heldObject.IsBeingThrown = true;
            _heldObject.RB.AddForce(_mainCamera.transform.forward.normalized * _throwForceMultiplier, ForceMode.VelocityChange);
            _heldObject = null;
        }
        else
        {
            RemoveHeldObject();
        }
    }
    #endregion

}
