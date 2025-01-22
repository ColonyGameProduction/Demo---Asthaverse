using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitTeammatesEvent : MonoBehaviour, IUnsubscribeEvent
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
        _chosenSplitTeammates.GetMovementStateMachine.OnIsTheSamePosition += Gone;
        if(_playableCharacterManager.CurrPlayableChara == _chosenSplitTeammates)
        {
            _playableCharacterManager.ChangePlayer();
        }
        _playableCharacterManager.RemovePlayableCharacter(_chosenSplitTeammates);
        _enemyAIManager.OnRemovedPlayable?.Invoke(_chosenSplitTeammates.transform);

        if(_newPlaceToGo != null) _chosenSplitTeammates.GetFriendAIStateMachine.ChangeFriendDefaultDirectionWhenSplit(_newPlaceToGo);
    }
    private void Gone(Vector3 pos)
    {
        if(!_chosenSplitTeammates.gameObject.activeSelf) return;

        if(pos == _newPlaceToGo.position && Vector3.Distance(_chosenSplitTeammates.transform.position, _newPlaceToGo.position) < 0.5f)
        {
            // Debug.Log(pos + " emang ini sama ??? " + _newPlaceToGo.position);
            _chosenSplitTeammates.gameObject.SetActive(false);
            UnsubscribeEvent();
        }
    }

    public void UnsubscribeEvent()
    {
        _chosenSplitTeammates.GetMovementStateMachine.OnIsTheSamePosition -= Gone;
    }
}
