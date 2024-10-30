using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableInteraction : MonoBehaviour
{

    [SerializeField] private List<IInteractable> _interactablesList = new List<IInteractable>();
    [SerializeField] private GameObject _charaGameObject;
    [SerializeField] private IInteractable _currInteractable;
    private IInteractable thisObjInteractable;
    public IInteractable CurrInteractable{get{return _currInteractable;}}
    private void Awake() 
    {
        thisObjInteractable = GetComponentInParent<IInteractable>();
    }

    private void Update() 
    {
        _currInteractable = InteractWithInteractables();
        if(_currInteractable != null)
        {
            Vector3 playerToInteractableDir = (_currInteractable.InteractableTransform.position - _charaGameObject.transform.forward).normalized;

            Debug.Log("Interactable is at " + _currInteractable.InteractableTransform.position);
        }
    }
    public void Interact()
    {
        if(_currInteractable != null)
        {
            _currInteractable.Interact();
        }
    }
    private IInteractable InteractWithInteractables()
    {
        if(_interactablesList.Count == 0)return null;
        float closestDistance = Mathf.Infinity;
        // float smallestDotProduct = -1;
        IInteractable chosenInteractable = null;
        foreach(var interactable in _interactablesList)
        {
            float InteractableToPlayerDistance = Vector3.Distance(interactable.InteractableTransform.position, thisObjInteractable.InteractableTransform.position);
            

            if(closestDistance > InteractableToPlayerDistance)
            {
                closestDistance = InteractableToPlayerDistance;

                chosenInteractable = interactable;
            }
        }
        return chosenInteractable;
    }

    private void OnTriggerEnter(Collider other) 
    {
        IInteractable interactable = other.gameObject.GetComponent<IInteractable>();
        if(interactable != null && !_interactablesList.Contains(interactable) && interactable.CanInteract)
        {
            if(interactable == thisObjInteractable) return;
            Debug.Log(interactable.InteractableTransform.name + " in");
            _interactablesList.Add(interactable);
        }
        
    }
    private void OnTriggerExit(Collider other) 
    {
        IInteractable interactable = other.gameObject.GetComponent<IInteractable>();
        if(interactable != null && _interactablesList.Contains(interactable))
        {
            Debug.Log(interactable.InteractableTransform.name + " out");
            _interactablesList.Remove(interactable);
        }
    }

}
