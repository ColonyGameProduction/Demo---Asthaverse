using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTrigger : MonoBehaviour
{  
    public Quest thisQuest;
    public bool canProceedToNextQuest;

    private void Start()
    {
        thisQuest = GetComponent<Quest>();
        if(!thisQuest.questActivate)
        {
            gameObject.SetActive(false);
        }    
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            thisQuest.ActivatingTheNextQuest();
            gameObject.SetActive(false);
        }
    }

    
}
