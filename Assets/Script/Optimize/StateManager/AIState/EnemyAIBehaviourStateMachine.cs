using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIBehaviourStateMachine : AIBehaviourStateMachine
{
    [Header("Enemy Alert Value")]
    [SerializeField] private float _alertValue;
    [SerializeField] private float _maxAlertValue;
    [Header("Enemy States")]
    protected alertState _enemyState;
    
    [Header("Patrol Path")]
    [SerializeField] private GameObject[] _patrolPath;
    private bool _switchingPath;
    private int _currPath;
    private Vector3 _enemyCharaLastSeenPosition;
    
    public override void SwitchState(BaseState newState)
    {
        throw new System.NotImplementedException();
    }
    private void Update() 
    {
        ChangingEnemyState();
        StateChecker();
    }
    public void ChangingEnemyState()
    {
        if (_fovMachine.VisibleTargets.Count > 0)
        {
            foreach(Transform transform in _fovMachine.VisibleTargets)
            {
                // Debug.Log(transform.gameObject.name + "name" + transform);
                CharacterIdentity _enemyIdentity = transform.gameObject.GetComponentInParent<CharacterIdentity>();
                float enemyStealthStat = 0;
                if(_enemyIdentity)
                {
                    enemyStealthStat = _enemyIdentity.StealthStat;
                }
                if(_maxAlertValue > enemyStealthStat || _maxAlertValue == 0)
                {
                    _maxAlertValue = enemyStealthStat;
                }
            }
            
            if (_alertValue <= _maxAlertValue)
            {
                _alertValue += Time.deltaTime * 10;
            }
        }
        else
        {
            if (_alertValue >= 0 && _fovMachine.OtherVisibleTargets.Count == 0 && _enemyCharaLastSeenPosition == Vector3.zero)
            {
                _alertValue -= Time.deltaTime * 10;
            }
        }

        if(_maxAlertValue > 0)
        {
            if (_alertValue <= _maxAlertValue/2)
            {
                _enemyState = alertState.Idle;
            }
            else if (_alertValue >= _maxAlertValue / 2 && _alertValue < _maxAlertValue)
            {
                _enemyState = alertState.Hunted;
            }
            else if (_alertValue >= _maxAlertValue)
            {
                _enemyState = alertState.Engage;
            }

        }
    }
    public void StateChecker()
    {
        switch (_enemyState)
        {
            case alertState.Idle:
                if(_charaIdentity.GetNormalUseWeaponData.IsUsingWeapon)_charaIdentity.UseWeaponStateMachine.ForceStopUseWeapon();
                if(_fovMachine.VisibleTargets.Count == 0)
                {
                    Patrol();
                }
                else
                {
                    _charaIdentity.MovementStateMachine.ForceStopMoving();
                    _enemyCharaLastSeenPosition = _fovMachine.VisibleTargets[0].position;
                }
                Debug.Log("idle");
                break;
            case alertState.Hunted:
                if(_charaIdentity.GetNormalUseWeaponData.IsUsingWeapon)_charaIdentity.UseWeaponStateMachine.ForceStopUseWeapon();
                if (_fovMachine.OtherVisibleTargets.Count > 0)
                {
                    _charaIdentity.MovementStateMachine.GiveAIDirection(_fovMachine.OtherVisibleTargets[0]);

                }
                else
                {
                    if (_enemyCharaLastSeenPosition != Vector3.zero)
                    {
                        _charaIdentity.MovementStateMachine.GiveAIDirection(_fovMachine.VisibleTargets[0]);
                    }
                    else
                    {
                        _charaIdentity.MovementStateMachine.ForceStopMoving();
                    }

                    if (Vector3.Distance(transform.position, _enemyCharaLastSeenPosition) < 0.5f)
                    {
                        _enemyCharaLastSeenPosition = Vector3.zero;
                    }
                }
                Debug.Log("hunted");
                break;
            case alertState.Engage:
                Debug.Log("engage");
                switch(_fovMachine.CurrState)
                {
                    case FOVDistState.far:
                        if(_fovMachine.VisibleTargets.Count > 0)
                        {
                            _charaIdentity.MovementStateMachine.GiveAIDirection(_fovMachine.VisibleTargets[0]);
                            _charaIdentity.UseWeaponStateMachine.GiveChosenTarget(_fovMachine.VisibleTargets[0]);
                        }
                        Debug.Log("Far");
                        break;
                    case FOVDistState.middle:
                        if(_fovMachine.VisibleTargets.Count > 0)_charaIdentity.UseWeaponStateMachine.GiveChosenTarget(_fovMachine.VisibleTargets[0]);
                        Debug.Log("Middle");
                        break; 
                    case FOVDistState.close:
                        if(_fovMachine.VisibleTargets.Count > 0)_charaIdentity.UseWeaponStateMachine.GiveChosenTarget(_fovMachine.VisibleTargets[0]);
                        Debug.Log("Close");
                        break;
                }
                break;
        }

        if(_charaIdentity.HealthNow <= 0)
        {
            Debug.Log("Dead");
        }
    }
    private void Patrol()
    {
        if (_patrolPath.Length > 1)
        {
            if (_charaIdentity.MovementStateMachine.IsIdle)
            {
                if (!_switchingPath)
                {
                    _currPath++;
                }
                else
                {
                    _currPath--;
                }

                if (_currPath == _patrolPath.Length - 1)
                {
                    _switchingPath = true;
                }
                else if (_currPath == 0)
                {
                    _switchingPath = false;
                }
            }
            _charaIdentity.MovementStateMachine.GiveAIDirection(_patrolPath[_currPath].transform);
        }
    }


}
