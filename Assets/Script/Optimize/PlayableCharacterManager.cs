using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableCharacterManager : MonoBehaviour
{
    GameManager _gm;
    GameInputManager _gameInputManager;
    void Start()
    {
        _gm = GameManager.instance;
        
        _gameInputManager = GameInputManager.Instance;
        SubscribeToGameInputManager();

    }

    // Update is called once per frame
    public void Update()
    {
        
    }
    private void SubscribeToGameInputManager()
    {
        _gameInputManager.OnRunPerformed += GameInput_OnRunPerformed;
        _gameInputManager.OnRunCanceled += GameInput_OnRunCanceled;
        _gameInputManager.OnCrouchPerformed += GameInput_OnCrouchPerformed;
        _gameInputManager.OnCrouchCanceled += GameInput_OnCrouchCanceled;

        _gameInputManager.OnChangePlayerPerformed += GameInput_OnChangePlayerPerformed;
        _gameInputManager.OnChangeWeaponPerformed += GameInput_OnChangeWeaponPerformed;

        _gameInputManager.OnCommandPerformed += GameInput_OnCommandPerformed;
        _gameInputManager.OnUnCommandPerformed += GameInput_OnUnCommandPerformed;
        _gameInputManager.OnHoldPosPerformed += GameInput_OnHoldPosPerformed;
        _gameInputManager.OnUnHoldPosPerformed += GameInput_OnUnHoldPosPerformed;

        _gameInputManager.OnSilentKillPerformed += GameInput_OnSilentKillPerformed;
        _gameInputManager.OnShootingPerformed += GameInput_OnShootingPerformed;
        _gameInputManager.OnShootingCanceled += GameInput_OnShootingCanceled;
        _gameInputManager.OnScopePerformed += GameInput_OnScopePerformed;
        _gameInputManager.OnReloadPerformed += GameInput_OnReloadPerformed;
    }

    private void GameInput_OnRunPerformed()
    {
        throw new NotImplementedException();
    }

    private void GameInput_OnRunCanceled()
    {
        throw new NotImplementedException();
    }

    private void GameInput_OnCrouchPerformed()
    {
        throw new NotImplementedException();
    }

    private void GameInput_OnCrouchCanceled()
    {
        throw new NotImplementedException();
    }

    private void GameInput_OnChangePlayerPerformed()
    {
        throw new NotImplementedException();
    }
    private void GameInput_OnChangeWeaponPerformed()
    {
        throw new NotImplementedException();
    }

    private void GameInput_OnCommandPerformed()
    {
        throw new NotImplementedException();
    }
    private void GameInput_OnUnCommandPerformed()
    {
        throw new NotImplementedException();
    }
    private void GameInput_OnHoldPosPerformed()
    {
        throw new NotImplementedException();
    }
    private void GameInput_OnUnHoldPosPerformed()
    {
        throw new NotImplementedException();
    }

    private void GameInput_OnSilentKillPerformed()
    {
        throw new NotImplementedException();
    }
    private void GameInput_OnShootingPerformed()
    {
        throw new NotImplementedException();
    }
    private void GameInput_OnShootingCanceled()
    {
        throw new NotImplementedException();
    }
    private void GameInput_OnScopePerformed()
    {
        throw new NotImplementedException();
    }
    private void GameInput_OnReloadPerformed()
    {
        throw new NotImplementedException();
    }
}
