using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenableObj_IntObj : InteractableObject
{
    protected bool _isAnimatingOpening;
    protected bool _isOpen;
    [SerializeField]protected float _animationOpenTimer = 0.5f;
    public bool IsOpen{get {return _isOpen;}}

    public override bool CanInteract {get {return !_isAnimatingOpening ? true : false;}}
    public override void Interact(PlayableCharacterIdentity characterIdentity)
    {
        //jalankan animasi buka 
        Debug.Log("Opening this");
        StartCoroutine(OpenObj());
    }
    private IEnumerator OpenObj()
    {
        _isAnimatingOpening = true;
        yield return new WaitForSeconds(_animationOpenTimer);
        _isAnimatingOpening = false;
    }
}
