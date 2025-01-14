using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using UnityEngine;

public class RiggingController : MonoBehaviour, IUnsubscribeEvent
{

    private CharacterIdentity _characterIdentity;
    [SerializeField] private TwoBoneIKConstraint[] _leftHandRigs;
    [SerializeField] private Rig[] _followHandRigs;
    private bool _isChangeInUpdateLeftHand, _isChangeInUpdateFollowHand;
    private bool _isTurnOnLeftHand, _isTurnOnFollowHand;

    private void Awake() 
    {
        _characterIdentity = GetComponentInParent<CharacterIdentity>();
        _characterIdentity.OnToggleLeftHandRig += ToggleLeftHandRig;
        _characterIdentity.OnToggleFollowHandRig += ToggleFollowHandRig;
    }

    private void Update()
    {
        if(_isChangeInUpdateLeftHand)
        {
            _isChangeInUpdateLeftHand = false;
            ChangeLeftHandRigWeight(_isTurnOnLeftHand);
        }
        if(_isChangeInUpdateFollowHand)
        {
            _isChangeInUpdateFollowHand = false;
            ChangeFollowHandRigWeight(_isTurnOnFollowHand);
        }
    }
    private void ToggleLeftHandRig(bool isTurnOn, bool isChangeInUpdate)
    {
        if(_leftHandRigs != null) 
        {
            ChangeLeftHandRigWeight(isTurnOn);

            _isChangeInUpdateLeftHand = isChangeInUpdate;
            _isTurnOnLeftHand = isTurnOn;
        }
    }
    private void ChangeLeftHandRigWeight(bool isTurnOn)
    {
        foreach(TwoBoneIKConstraint leftHandRig in _leftHandRigs)
        {
            leftHandRig.weight = isTurnOn ? 1 : 0;
        }
    }
    private void ToggleFollowHandRig(bool isTurnOn, bool isChangeInUpdate)
    {
        if(_followHandRigs != null) 
        {
            ChangeLeftHandRigWeight(isTurnOn);

            _isChangeInUpdateFollowHand = isChangeInUpdate;
            _isTurnOnFollowHand = isTurnOn;
        }
    }
    private void ChangeFollowHandRigWeight(bool isTurnOn)
    {
        foreach(Rig followHand in _followHandRigs)
        {
            followHand.weight = isTurnOn ? 1 : 0;
        }
    }

    public void UnsubscribeEvent()
    {
        _characterIdentity.OnToggleLeftHandRig -= ToggleLeftHandRig;
        _characterIdentity.OnToggleFollowHandRig -= ToggleFollowHandRig;
    }
}
