using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Door_OpenableObj : OpenableObj_IntObj
{
    private PlayableCharacterManager _playableCharaManager;
    protected virtual void Awake()
    {
        CloseDoor();
    }
    protected void Start() 
    {
        _playableCharaManager = PlayableCharacterManager.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        CharacterIdentity characterIdentity = other.gameObject.GetComponent<CharacterIdentity>();
        if(characterIdentity != null && characterIdentity != _playableCharaManager.CurrPlayableChara)
        {
            if(!_isOpen)
            {
                ToggleOpenClose(characterIdentity.transform);
            }
        }
    }
    private void OnTriggerStay(Collider other) 
    {
        CharacterIdentity characterIdentity = other.gameObject.GetComponent<CharacterIdentity>();
        if(characterIdentity != null && characterIdentity != _playableCharaManager.CurrPlayableChara)
        {
            if(!_isOpen)
            {
                ToggleOpenClose(characterIdentity.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterIdentity characterIdentity = other.gameObject.GetComponent<CharacterIdentity>();
        if(characterIdentity != null && characterIdentity != _playableCharaManager.CurrPlayableChara)
        {
            if(_isOpen)
            {
                ToggleOpenClose(characterIdentity.transform);
            }
        }
    }
    protected abstract void CloseDoor();
}
