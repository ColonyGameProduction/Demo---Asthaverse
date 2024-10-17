using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBehaviourStateMachine : BaseStateMachine
{
    #region Normal Variable
    [Header("Other Important Variable")]
    [SerializeField] protected FOVMachine _fovMachine;
    public FOVMachine GetFOVMachine { get { return _fovMachine; } }

    #endregion
    protected override void Awake() 
    {
        //Dari Chara Identity bisa akses ke semua yg berhubungan dgn characteridentity
        base.Awake();

        if(_fovMachine == null)_fovMachine = GetComponent<FOVMachine>();
        
    }
}
