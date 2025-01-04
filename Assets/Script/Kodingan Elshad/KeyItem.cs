using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : MonoBehaviour
{
    public Quest thisQuest;
    private void Start()
    {
        thisQuest = GetComponent<Quest>();

        if (!thisQuest.questActivate && thisQuest != null)
        {
            gameObject.SetActive(false);
        }
    }

    public void KeyItemInteract()
    {
        if (thisQuest != null && thisQuest.questComplete != true)
        {
            Debug.Log("Quest Complete");
            thisQuest.questComplete = true;
            thisQuest.ActivatingTheNextQuest();
        }
    }

    
}
