using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveSpeed : MonoBehaviour
{
    bool canRevive;
    public PlayableCharacterIdentity chosenDed;
    public PlayableCharacterIdentity characterIdentity;
    private void Awake() {
        characterIdentity = GetComponent<PlayableCharacterIdentity>();
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            PlayableCharacterIdentity c = other.GetComponent<PlayableCharacterIdentity>();
            if(c != null && c.IsDead)
            {
                chosenDed = c;
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Player"))
        {
            if(chosenDed != null && chosenDed.gameObject == other)
            {
                chosenDed = null;
            }
        }
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.F))
        {
            if(chosenDed!=null)chosenDed.Revive();
        }
    }
}
