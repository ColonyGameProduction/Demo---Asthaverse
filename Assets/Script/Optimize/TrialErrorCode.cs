using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialErrorCode : MonoBehaviour
{
    [SerializeField] private Transform _magCurrGunTransform;
    [SerializeField] private Transform _magGunParent;
    [SerializeField] private Transform _handReloadParent;
    public bool changeParent, isBack;
    void Start()
    {
        
    }

    void Update()
    {
        if(changeParent)
        {
            changeParent = false;
            if(isBack && _magGunParent)_magCurrGunTransform.parent = _magGunParent;
            else if(!isBack && _handReloadParent) _magCurrGunTransform.parent = _handReloadParent;
        }
    }
}
