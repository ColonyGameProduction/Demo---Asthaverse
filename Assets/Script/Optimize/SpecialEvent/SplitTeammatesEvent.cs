using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitTeammatesEvent : MonoBehaviour
{
    [SerializeField] private PlayableCharacterIdentity _chosenSplitTeammates;
    [SerializeField] private Transform _newPlaceToGo;
    private PlayableCharacterManager _playableCharacterManager;
    private EnemyAIManager _enemyAIManager;
    public bool _go;

    private void Start() 
    {
        _playableCharacterManager = PlayableCharacterManager.Instance;
        _enemyAIManager = EnemyAIManager.Instance;
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
        _enemyAIManager.OnRemovedPlayable?.Invoke(_chosenSplitTeammates.transform);

        if(_newPlaceToGo != null) _chosenSplitTeammates.GetFriendAIStateMachine.ChangeFriendDefaultDirectionWhenSplit(_newPlaceToGo);
    }
}
