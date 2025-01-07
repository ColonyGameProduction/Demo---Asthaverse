using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointTrigger : MonoBehaviour
{
    private GameManager _gm;
    [SerializeField] private int _checkPointNumber;
    private void Start() 
    {
        _gm = GameManager.instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(other.gameObject.GetComponentInParent<PlayableCharacterIdentity>().IsPlayerInput)
            {
                if(_gm.GetCurrCheckPointIdx() >= _checkPointNumber)
                {
                    this.gameObject.SetActive(false);
                }
                else
                {
                    _gm.SaveCheckPoint(_checkPointNumber);
                    this.gameObject.SetActive(false);
                }
            }
        }
    }
}
