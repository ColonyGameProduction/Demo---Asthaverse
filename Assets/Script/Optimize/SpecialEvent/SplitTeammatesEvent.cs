using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitTeammatesEvent : MonoBehaviour
{
    [SerializeField] private PlayableCharacterIdentity _chosenSplitTeammates;
    [SerializeField] private Transform _newPlaceToGo;
    private PlayableCharacterManager _playableCharacterManager;
    public bool _go;

    private void Start() 
    {
        _playableCharacterManager = PlayableCharacterManager.Instance;
    }
    private void Update() 
    {
        if(_go)
        {
            _go = false;
            SplitEvent();
        }
    }
    public void SplitEvent()
    {
        if(_playableCharacterManager.CurrPlayableChara == _chosenSplitTeammates)
        {
            _playableCharacterManager.ChangePlayer();
        }
        _playableCharacterManager.RemovePlayableCharacter(_chosenSplitTeammates);
        if(_newPlaceToGo != null) _chosenSplitTeammates.GetFriendAIStateMachine.ChangeFriendDefaultDirectionWhenSplit(_newPlaceToGo);
    }
}
