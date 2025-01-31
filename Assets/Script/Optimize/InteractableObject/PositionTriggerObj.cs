using System;

using UnityEngine;

public class PositionTriggerObj : MonoBehaviour, IConnectToQuest
{
    public event Action OnTriggerQuestComplete;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(other.gameObject.GetComponentInParent<PlayableCharacterIdentity>().IsPlayerInput)
            {
                OnTriggerQuestComplete?.Invoke();
            }
        }
    }
}
