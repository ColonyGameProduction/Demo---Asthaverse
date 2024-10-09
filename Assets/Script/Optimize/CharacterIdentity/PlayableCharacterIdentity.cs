using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacterIdentity : CharacterIdentity, IPlayableFriendDataHelper
{
    [Header("Friend Data Helper")]
    [SerializeField] protected GameObject[] _friendsNormalPosition;
    protected int _friendID;

    //Thing needed to getcomponent
    protected ICrouchMovementData _getCrouchMovementBool;
    protected IPlayableMovementDataNeeded _getPlayableMovementData;
    protected PlayableCamera _getPlayableCamera;


    [HideInInspector]
    //Getter Setter
    public ICrouchMovementData GetCrouchMovementBool {get { return _getCrouchMovementBool;}}
    public IPlayableMovementDataNeeded GetPlayableMovementData {get { return _getPlayableMovementData;}}
    public int FriendID {get { return _friendID;} set { _friendID = value; }}
    public GameObject[] GetFriendsNormalPosition { get {return _friendsNormalPosition;}}
    public PlayableCamera GetPlayableCamera {get {return _getPlayableCamera;}}

    protected override void Awake()
    {
        base.Awake();
        _getCrouchMovementBool = GetComponent<ICrouchMovementData>();
        _getPlayableMovementData = GetComponent<IPlayableMovementDataNeeded>();
        _getPlayableCamera = GetComponent<PlayableCamera>();
    }
    
}
