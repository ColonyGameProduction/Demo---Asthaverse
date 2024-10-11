using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayableCharacterCommandManager : MonoBehaviour
{
    [SerializeField] private PlayableCharacterManager _playableCharaManager;
    [SerializeField] private LayerMask groundMask;
    private int selectedFriendID = -1;

    [SerializeField] private GameObject _commandUIContainer;

    private void Awake() 
    {
        if(_playableCharaManager == null)_playableCharaManager = GetComponent<PlayableCharacterManager>();

        _playableCharaManager.OnCommandingBoolChange += PlayableCharaManager_OnCommandingBoolChange;
    }


    void Update()
    {
        if(PlayableCharacterManager.IsCommandingFriend)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                selectedFriendID = 1; // Select FriendAI with ID 1
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                selectedFriendID = 2; // Select FriendAI with ID 2
            }
            if (selectedFriendID != -1 && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector3 rayOrigin = Camera.main.transform.position;
                Vector3 rayDirection = Camera.main.transform.forward.normalized;

                Debug.DrawRay(rayOrigin, rayDirection * 100f, Color.red, 2f);

                if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, 100f, groundMask))
                {
                    // Set the destination for the selected friend based on the mouse click
                    _playableCharaManager.ChangeFriendCommandPosition(selectedFriendID, hit.point);
                }
            }
        }
    }
    private void PlayableCharaManager_OnCommandingBoolChange(bool obj)
    {
        _commandUIContainer?.SetActive(obj);
    }
}
