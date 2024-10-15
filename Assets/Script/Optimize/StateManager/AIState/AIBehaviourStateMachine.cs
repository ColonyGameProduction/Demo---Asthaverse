using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBehaviourStateMachine : BaseStateMachine
{
    #region Normal Variable
    [Header("Other Important Variable")]
    [SerializeField] protected FOVMachine _fovMachine;
    [SerializeField] protected CharacterIdentity _charaIdentity;
    [SerializeField] protected MovementStateMachine _moveStateMachine;
    [SerializeField] protected UseWeaponStateMachine _useWeaponStateMachine;
    
    // [Header("Private Variable")]

    #endregion
    protected virtual void Awake() 
    {
        //Dari Chara Identity bisa akses ke semua yg berhubungan dgn characteridentity
        if(_charaIdentity == null)_charaIdentity = GetComponent<CharacterIdentity>();

        if(_fovMachine == null)_fovMachine = GetComponent<FOVMachine>();
        
        if(_moveStateMachine == null) _moveStateMachine = GetComponent<MovementStateMachine>();
        if(_useWeaponStateMachine == null) _useWeaponStateMachine = GetComponent<UseWeaponStateMachine>();
    }
}
