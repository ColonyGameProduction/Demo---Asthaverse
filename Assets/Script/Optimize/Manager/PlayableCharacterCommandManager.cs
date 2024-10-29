using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayableCharacterCommandManager : MonoBehaviour
{
    [SerializeField] private PlayableCharacterManager _playableCharaManager;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _maxCommandDistance = 100f;
    private static int _selectedFriendID = -1;

    [SerializeField] private GameObject _commandUIContainer;
    public static int SelectedFriendID {get { return _selectedFriendID; } }

    private void Awake() 
    {
        if(_playableCharaManager == null)_playableCharaManager = GetComponent<PlayableCharacterManager>();

        _playableCharaManager.OnCommandingBoolChange += PlayableCharaManager_OnCommandingBoolChange;
        _playableCharaManager.OnRegroupOneFriendInput += PlayableCharaManager_OnRegroupOneFriendInput;
        // _playableCharaManager.OnCommandUnHoldInput += PlayableCharaManager_OnCommandUnHoldInput;
    }

    void Update()
    {
        if(PlayableCharacterManager.IsCommandingFriend)
        {
            if (_selectedFriendID != -1 && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector3 rayOrigin = Camera.main.transform.position;
                Vector3 rayDirection = Camera.main.transform.forward.normalized;

                Debug.DrawRay(rayOrigin, rayDirection * _maxCommandDistance, Color.red, 2f);
                

                if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, _maxCommandDistance, _groundMask))
                {
                    // Set the destination for the selected friend based on the mouse click
                    _playableCharaManager.ChangeFriendCommandPosition(_selectedFriendID, hit.point);
                }
            }
        }
    }
    private void PlayableCharaManager_OnCommandingBoolChange(bool obj, int friendID)
    {
        _commandUIContainer?.SetActive(obj);
        _selectedFriendID = friendID;
    }
    
    private void PlayableCharaManager_OnRegroupOneFriendInput()
    {
        _playableCharaManager.ChangeHoldCommandFriend(false, SelectedFriendID);
    }

}
