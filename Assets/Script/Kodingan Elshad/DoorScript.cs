using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public bool isOpen = false;
    public bool isInfront = false;
    public bool isStaying = false;

    public void DoorInteracted(Transform playerPos)
    {
        Vector3 direction = transform.position - playerPos.position;
        float dot = Vector3.Dot(transform.forward, direction.normalized);        

        if (isOpen)
        {
            if (isInfront)
            {
                LeanTween.rotateAround(transform.GetChild(0).gameObject, Vector3.up, 90f, .2f);
            }
            else
            {
                LeanTween.rotateAround(transform.GetChild(0).gameObject, Vector3.up, -90f, .2f);
            }
            isOpen = false;
        }
        else
        {
            if (dot > 0)
            {
                isInfront = true;
            }
            else
            {
                isInfront = false;
            }

            if (isInfront)
            {
                LeanTween.rotateAround(transform.GetChild(0).gameObject, Vector3.up, -90f, .2f);
            }
            else
            {
                LeanTween.rotateAround(transform.GetChild(0).gameObject, Vector3.up, 90f, .2f);
            }
            isOpen = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.GetComponentInParent<PlayerAction>().enabled)
        {
            if(!isOpen)
            {
                DoorInteracted(other.gameObject.GetComponentInParent<PlayerAction>().transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.GetComponentInParent<PlayerAction>().enabled)
        {
            if(isOpen)
            {
                DoorInteracted(other.gameObject.GetComponentInParent<PlayerAction>().transform);
            }
        }
    }

}
