using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using UnityEngine;

public class RiggingController : MonoBehaviour, IUnsubscribeEvent
{

    private CharacterIdentity _characterIdentity;
    [SerializeField] private TwoBoneIKConstraint[] _leftHandRigs;
    private bool _isChangeInUpdate;
    private bool _isTurnOn;

    private void Awake() 
    {
        _characterIdentity = GetComponentInParent<CharacterIdentity>();
        _characterIdentity.OnToggleLeftHandRig += ToggleLeftHandRig;
    }

    private void Update()
    {
        if(_isChangeInUpdate)
        {
            _isChangeInUpdate = false;
            ChangeLeftHandRigWeight(_isTurnOn);
        }
    }
    private void ToggleLeftHandRig(bool isTurnOn, bool isChangeInUpdate)
    {
        if(_leftHandRigs != null) 
        {
            ChangeLeftHandRigWeight(isTurnOn);

            _isChangeInUpdate = isChangeInUpdate;
            _isTurnOn = isTurnOn;
        }
    }
    private void ChangeLeftHandRigWeight(bool isTurnOn)
    {
        foreach(TwoBoneIKConstraint leftHandRig in _leftHandRigs)
        {
            leftHandRig.weight = isTurnOn ? 1 : 0;
        }
    }

    public void UnsubscribeEvent()
    {
        _characterIdentity.OnToggleLeftHandRig -= ToggleLeftHandRig;
    }
}
