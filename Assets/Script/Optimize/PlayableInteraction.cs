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
    private IInteractable _thisObjInteractable;
    private PlayableCharacterIdentity _playableCharacterIdentity;
    public IInteractable CurrInteractable{get{return _currInteractable;}}
    public ISilentKillAble CurrSilentKillAble{get{return _currSilentKillAble;}}
    private void Awake() 
    {
        _thisObjInteractable = GetComponentInParent<IInteractable>();
        _playableCharacterIdentity = GetComponentInParent<PlayableCharacterIdentity>();
    }

    private void Update() 
    {
        _currInteractable = GetClosestInteractables();
        if(_currInteractable != null)
        {
            // Vector3 playerToInteractableDir = (_currInteractable.InteractableTransform.position - _charaGameObject.transform.forward).normalized;

            // Debug.Log("Interactable is at " + _currInteractable.InteractableTransform.position);
        }

        _currSilentKillAble = GetClosestSilentkillable();
        // Debug.Log(_currSilentKillAble + " " + _silentKillAbleList.Count);
        if(_currSilentKillAble != null)
        {

        }
    }
    public void Interact()
    {
        if(_currInteractable != null)
        {
            if(_interactablesList.Contains(_currInteractable))
            {
                _interactablesList.Remove(_currInteractable);
            }
            _currInteractable.Interact(_playableCharacterIdentity);
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

}
