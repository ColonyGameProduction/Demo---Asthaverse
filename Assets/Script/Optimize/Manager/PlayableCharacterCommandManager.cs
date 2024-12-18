using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayableCharacterCommandManager : MonoBehaviour, IUnsubscribeEvent
{
    [SerializeField] private PlayableCharacterManager _playableCharaManager;
    [SerializeField] private LayerMask _groundMask, _wallMask;
    [SerializeField] private float _maxCommandDistance = 100f;
    private static int _selectedFriendID = -1;

    [SerializeField] private CommandUIHandler _commandUIHandler;
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
        if (PlayableCharacterManager.IsCommandingFriend && Mouse.current.leftButton.wasPressedThisFrame)
        {
            GetCommandPosForFriend();
        }
    }
    public void GetCommandPosForFriend()
    {
        if(_selectedFriendID != -1)
        {
            Vector3 rayOrigin = Camera.main.transform.position;
            Vector3 rayDirection = Camera.main.transform.forward.normalized;

            // Debug.DrawRay(rayOrigin, rayDirection * _maxCommandDistance, Color.red, 2f);

            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, _maxCommandDistance, _groundMask))
            {
                // Set the destination for the selected friend based on the mouse click
                _playableCharaManager.ChangeFriendCommandPosition(_selectedFriendID, hit.point);
            }
        }
        
    }
    private void PlayableCharaManager_OnCommandingBoolChange(bool obj, int friendID)
    {
        if(obj) _commandUIHandler.OpenCommandUI();
        else _commandUIHandler.CloseCommandUI();

        _selectedFriendID = friendID;
    }
    
    public void PlayableCharaManager_OnRegroupOneFriendInput()
    {
        _playableCharaManager.ChangeHoldCommandFriend(false, SelectedFriendID);
    }

    public void UnsubscribeEvent()
    {
        _playableCharaManager.OnCommandingBoolChange -= PlayableCharaManager_OnCommandingBoolChange;
        _playableCharaManager.OnRegroupOneFriendInput -= PlayableCharaManager_OnRegroupOneFriendInput;
    }
}
