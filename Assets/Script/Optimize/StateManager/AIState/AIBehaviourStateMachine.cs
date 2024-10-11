using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBehaviourStateMachine : BaseStateMachine
{
    #region Normal Variable
    [Header("Other Important Variable")]
    [SerializeField] protected FOVMachine _fovMachine;
    [SerializeField] protected CharacterIdentity _charaIdentity;
    
    // [Header("Private Variable")]

    #endregion
    protected virtual void Awake() 
    {
        //Dari Chara Identity bisa akses ke semua yg berhubungan dgn characteridentity
        if(_charaIdentity == null)_charaIdentity = GetComponent<CharacterIdentity>();

        if(_fovMachine == null)_fovMachine = GetComponent<FOVMachine>();
    }
}
