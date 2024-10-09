using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadWeaponState : UseWeaponState
{
    bool isDoReloading;
    public ReloadWeaponState(UseWeaponStateMachine stateMachine, UseWeaponStateFactory factory) : base(stateMachine, factory)
    {

    }
    public override void EnterState()
    {
        // base.EnterState(); mainkan animasi
        isDoReloading = false;
        _normalUse.CanReload = false;

        Debug.Log("Reload Weapon" + _stateMachine.gameObject.name);
    }
    public override void UpdateState()
    {
        if(!isDoReloading && _normalUse.IsReloading)
        {
            DoReload();
        }
    }
    public override void ExiState()
    {
        //keknya ga ngapa ngapain?
    }
    public void DoReload() => isDoReloading = true;
    public void ReloadDone()
    {
        _normalUse.IsReloading = false;
        isDoReloading = false;

        //DO Delay to make canreload = true;
    }
    // private IEnumerator ReloadDelay()
    // {
    //     yield return new WaitForSeconds(0.1f);
        
    // }
}
