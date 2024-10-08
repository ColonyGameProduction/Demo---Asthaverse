using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacterIdentity : CharacterIdentity, IPlayableFriendDataHelper
{
    protected ICrouch _getCrouchMovementBool;
    protected IPlayableMovementDataNeeded _getPlayableMovementData;
    protected PlayableCamera _getPlayableCamera;
    [Header("Friend Data Helper")]
    [SerializeField] protected GameObject[] _friendsNormalPosition;
    protected int _friendID;

    [HideInInspector]
    //Getter Setter
    public ICrouch GetCrouchMovementBool {get { return _getCrouchMovementBool;}}
    public IPlayableMovementDataNeeded GetPlayableMovementData {get { return _getPlayableMovementData;}}
    public int FriendID {get { return _friendID;} set { _friendID = value; }}
    public GameObject[] GetFriendsNormalPosition { get {return _friendsNormalPosition;}}
    public PlayableCamera GetPlayableCamera {get {return _getPlayableCamera;}}

    protected override void Awake()
    {
        base.Awake();
        _getCrouchMovementBool = GetComponent<ICrouch>();
        _getPlayableMovementData = GetComponent<IPlayableMovementDataNeeded>();
    }
    
}
