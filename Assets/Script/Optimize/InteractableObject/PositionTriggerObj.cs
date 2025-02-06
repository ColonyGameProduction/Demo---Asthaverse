using System;

using UnityEngine;

public class PositionTriggerObj : MonoBehaviour, IConnectToQuest
{
    protected AudioSource _interactSFXAudioSource;
    public event Action OnTriggerQuestComplete;

    protected void Awake()
    {
        _interactSFXAudioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(other.gameObject.GetComponentInParent<PlayableCharacterIdentity>().IsPlayerInput)
            {
                if(_interactSFXAudioSource != null) _interactSFXAudioSource.Play();
                OnTriggerQuestComplete?.Invoke();
            }
        }
    }
    public virtual void WhenQuestActivated()
    {
        
    }
}
