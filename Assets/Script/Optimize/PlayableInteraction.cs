using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableInteraction : MonoBehaviour
{
    [SerializeField] private List<IInteractable> _interactablesList;
    [SerializeField] private GameObject _charaGameObject;

    private void Update() 
    {
        Debug.DrawRay(transform.position, _charaGameObject.transform.forward, Color.cyan);
    }
    public void InteractWithInteractables()
    {
        // Vector3 charaFaceDir = 
    }

    private void OnTriggerEnter(Collider other) 
    {
        IInteractable interactable = other.gameObject.GetComponent<IInteractable>();
        if(interactable != null && !_interactablesList.Contains(interactable))_interactablesList.Add(interactable);
    }
    private void OnTriggerExit(Collider other) 
    {
        IInteractable interactable = other.gameObject.GetComponent<IInteractable>();
        if(interactable != null && _interactablesList.Contains(interactable))_interactablesList.Remove(interactable);
    }

}
