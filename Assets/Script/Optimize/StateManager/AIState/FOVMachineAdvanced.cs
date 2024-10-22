using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVMachineAdvanced : FOVMachine, IFOVMachineState
{
    #region Normal Variable
    [SerializeField] private FOVDistState _currstate;
    public FOVDistState CurrState { get{return _currstate;} }
    #endregion
    private void Update() 
    {
        FOVStateHandler();
    }

    #region States Function
    private void FOVStateHandler()
    {
        Debug.Log("Is Update First?");
        float distance;
        // _currDistance = Mathf.Infinity;

        // foreach()
        if (_visibleTargets.Count > 0)
        {
            distance = Vector3.Distance(transform.position, _visibleTargets[0].position);
            _enemyCharalastSeenPosition = _visibleTargets[0].position;
        }
        else
        {            
            distance = Vector3.Distance(transform.position, _enemyCharalastSeenPosition);
        }


        if (distance <= _viewRadius && distance > _viewRadius - (_viewRadius/3))
        {
            _currstate = FOVDistState.far;
        }
        else if(distance <= _viewRadius - (_viewRadius / 3) && distance > _viewRadius - (_viewRadius / 3 * 2))
        {
            _currstate = FOVDistState.middle;
        }
        else if(distance <= _viewRadius - (_viewRadius / 3*2) && distance >= 0)
        {
            _currstate = FOVDistState.close;
        }

        // switch(FOVState)
        // {
        //     case FOVDistState.far:                
        //         Debug.Log("Far");
        //         break;
        //     case FOVDistState.middle:
        //         Debug.Log("Middle");
        //         break; 
        //     case FOVDistState.close:
        //         Debug.Log("Close");
        //         break;
        // }
        
    }
    #endregion
    public override void StartFOVMachine()
    {
        _currstate = FOVDistState.far;
        base.StartFOVMachine();
    }
    public override void StopFOVMachine()
    {
        _currstate = FOVDistState.far;
        base.StopFOVMachine();
        
    }
}
