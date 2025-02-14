
using UnityEngine;

public class FriendAI_EngageState : FriendAIState
{
    // private bool isFacingToWallWhenCrouching = false;
    public FriendAI_EngageState(FriendAIBehaviourStateMachine currStateMachine, FriendAIStateFactory factory) : base(currStateMachine, factory)
    {
    }

    public override void EnterState()
    {
        _sm.GetPlayableCharaIdentity.Aiming(true);
        if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
    }

    public override void UpdateState()
    {
        // Debug.Log("I'm engaging" + _sm.transform.name);


        if(!_sm.IsAIEngage || PlayableCharacterManager.IsSwitchingCharacter || PlayableCharacterManager.IsAddingRemovingCharacter || !_sm.IsAIInput || _sm.GetPlayableCharaIdentity.IsAnimatingOtherAnimation || _sm.GetPlayableCharaIdentity.IsReviving || _sm.GetPlayableCharaIdentity.IsSilentKilling || _sm.IsCharacterDead || _sm.GetPlayableCharaIdentity.IgnoreThisCharacter)
        {
            _sm.SwitchState(_factory.AI_IdleState());
            return;
        }

        _sm.GetFOVMachine.GetClosestEnemy();
        
        if(_sm.IsTakingCover && _sm.IsAtTakingCoverHidingPlace)
        {
            // Debug.Log("E-I'm idle from engage and at the take cover place" + _sm.transform.name);
                        
            _sm.SwitchState(_factory.AI_TakingCoverState());
            return;
            
        }

        if(!_sm.GotDetectedbyEnemy)
        {
            
            if(_sm.GetFOVMachine.ClosestEnemy != null)
            {
                // Debug.Log("E-NO ONE SEE ME; I SEE SOMEONE" + _sm.transform.name);
                _sm.IsCheckingLastPosTimer = _sm.IsCheckingLastPosTimerMax;

                _sm.GetPlayableCharaIdentity.Aiming(true);
                _sm.AimAIPointLookAt(_sm.GetFOVMachine.ClosestEnemy);
                _sm.SetAllowLookTarget(true, _sm.GetFOVMachine.ClosestEnemy.position, false);
                // if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
                if(!_sm.GetUseWeaponStateMachine.IsReloading && !_sm.GetUseWeaponStateMachine.HasNoMoreBullets)
                {
                    // Debug.Log("E-NO ONE SEE ME; I SEE SOMEONE - I'm SHOOTING THIS PERSON" + _sm.transform.name);

                    _sm.StartShooting(_sm.SearchBestBodyPartToShoot(_sm.GetFOVMachine.ClosestEnemy));
                }
                
                if(_sm.GetUseWeaponStateMachine.IsReloading || _sm.GetUseWeaponStateMachine.HasNoMoreBullets)
                {
                    _sm.StopShooting();
                    // Debug.Log("E-NO ONE SEE ME; I SEE SOMEONE - I'm RELOADING WHILE STILL LOOKING" + _sm.transform.name);
                    //Diam di tempat sambil ngelihat target
                }
            }
            else
            {
                // Debug.Log("E-NO ONE SEE ME; I SEE NO ONE" + _sm.transform.name);

                _sm.AimAIPointLookAt(null);
                _sm.StopShooting();
                if(!_sm.GetMoveStateMachine.IsIdle)
                {
                    // Debug.Log("E-NO ONE SEE ME; I SEE NO ONE - IM MOVING" + _sm.transform.name);
                    if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.GetFOVMachine.EnemyCharalastSeenPosition)
                    {
                        if(!_sm.GetMoveStateMachine.IsRunning && _sm.ClosestEnemyWhoSawAI != null)
                        {
                            Vector3 facedir = (_sm.Agent.steeringTarget - _sm.transform.position).normalized;

                            _sm.GetPlayableCharaIdentity.Aiming(true);
                            _sm.SetAllowLookTarget(true, -facedir, true);
                            // Debug.Log("E-NO ONE SEE ME; I SEE NO ONE - IM JUST RUNNING AWAY" + _sm.transform.name);
                        } 
                        else if(!_sm.GetMoveStateMachine.IsRunning && _sm.ClosestEnemyWhoSawAI == null)
                        {
                            _sm.SetAllowLookTarget(false, Vector3.zero, false);
                            if(!_sm.IsFriendAlreadyAtDefaultDistanceWhenEngaged()) _sm.GetMoveStateMachine.SetAIDirection(_sm.FriendsDefaultDirection.position);
                            else
                            {
                                if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
                            }
                            // Debug.Log("E-NO ONE SEE ME; I SEE NO ONE - IM JUST RUNNING BACK TO FRIEND" + _sm.transform.name);
                        }
                    }
                    else
                    {
                        if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                        {
                            // Debug.Log("E-NO ONE SEE ME; I SEE NO ONE - IM JUST CHECKING LAST SEEN POS" + _sm.transform.name);
                            float distanceBetweenCharaWithEnemyLastSeenPos = Vector3.Distance(_sm.GetFOVMachine.EnemyCharalastSeenPosition, _sm.transform.position);
                            if(distanceBetweenCharaWithEnemyLastSeenPos > _sm.GetFOVMachine.viewRadius - 5 || Physics.Raycast(_sm.transform.position, _sm.GetFOVMachine.EnemyCharalastSeenPosition, out RaycastHit hit, _sm.GetFOVMachine.viewRadius, _sm.GetFOVMachine.GroundMask))
                            {
                                _sm.GetMoveStateMachine.SetAIDirection(_sm.GetFOVMachine.EnemyCharalastSeenPosition);
                            }
                            else
                            {
                                if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
                            }
                            _sm.SetAllowLookTarget(true, _sm.GetFOVMachine.EnemyCharalastSeenPosition, false);
                        }
                    }
                }
                else
                {
                    _sm.GetPlayableCharaIdentity.Aiming(true);
                    if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                    {
                        // Debug.Log("E-NO ONE SEE ME; I SEE NO ONE - IM JUST CHECKING LAST SEEN POS WHEN IDLE" + _sm.transform.name);
                        float distanceBetweenCharaWithEnemyLastSeenPos = Vector3.Distance(_sm.GetFOVMachine.EnemyCharalastSeenPosition, _sm.transform.position);
                        if(distanceBetweenCharaWithEnemyLastSeenPos > _sm.GetFOVMachine.viewRadius - 5 || Physics.Raycast(_sm.transform.position, _sm.GetFOVMachine.EnemyCharalastSeenPosition, out RaycastHit hit,  _sm.GetFOVMachine.viewRadius, _sm.GetFOVMachine.GroundMask))
                        {
                            _sm.GetMoveStateMachine.SetAIDirection(_sm.GetFOVMachine.EnemyCharalastSeenPosition);
                        }
                        _sm.SetAllowLookTarget(true, _sm.GetFOVMachine.EnemyCharalastSeenPosition, false);
                        

                        Vector3 dirEnemyLastSeenToChara = (_sm.GetFOVMachine.EnemyCharalastSeenPosition - _sm.transform.position).normalized;
                        float dotBetweenCharaWithEnemyLastSeen = Vector3.Dot(dirEnemyLastSeenToChara, _sm.transform.forward);
                        if(dotBetweenCharaWithEnemyLastSeen >= 0.95f)
                        {   
                            if(_sm.GetFOVMachine.HasToCheckEnemyLastSeenPosition)
                            {
                                if(_sm.IsCheckingLastPosTimer > 0)_sm.IsCheckingLastPosTimer -= Time.deltaTime;
                                else
                                {   
                                    // Debug.Log("E-NO ONE SEE ME; I SEE NO ONE - IM DONE CHECKING LAST SEEN POS WHEN IDLE" + _sm.transform.name);
                                    _sm.GetFOVMachine.IsCheckingEnemyLastPosition();
                                }
                            }
                        }
                    }
                    else
                    {
                        // Debug.Log("E-NO ONE SEE ME; I SEE NO ONE - I JUST IDLE" + _sm.transform.name);
                        _sm.SetAllowLookTarget(false, Vector3.zero, false);
                        if(!_sm.IsFriendAlreadyAtDefaultDistanceWhenEngaged())
                        {
                            _sm.GetMoveStateMachine.SetAIDirection(_sm.FriendsDefaultDirection.position);
                        }
                    }
                    
                }
            }
        }
        else
        {
            _sm.GetClosestEnemyWhoSawAI();
            Transform currTarget = null;
            bool isClosestEnemy = false;
            if(_sm.GetFOVMachine.ClosestEnemy != null)
            {
                _sm.IsCheckingLastPosTimer = _sm.IsCheckingLastPosTimerMax;
                currTarget = _sm.GetFOVMachine.ClosestEnemy;
                isClosestEnemy = true;
                _sm.AimAIPointLookAt(_sm.GetFOVMachine.ClosestEnemy);
                // Debug.Log("E-SOMEONE SEE ME; I SEE SOMEONE" + _sm.transform.name);
            }
            else
            {
                currTarget = _sm.ClosestEnemyWhoSawAI;
                _sm.AimAIPointLookAt(null);
                // Debug.Log("E-SOMEONE SEE ME; I SEE NO ONE" + _sm.transform.name);
            }
            if(!_sm.GetPlayableCharaIdentity.IsHalfHealthOrLower)
            {
                // Debug.Log("E-SOMEONE SEE ME - I'M HEALTHY" + _sm.transform.name);

                if(_sm.EnemyWhoSawAIList.Count < _sm.MinEnemyMakeCharaFeelOverwhelmed)
                {
                    // Debug.Log("E-SOMEONE SEE ME - I'M HEALTHY - I CAN HANDLE THIS" + _sm.transform.name);
                    if(!_sm.GetUseWeaponStateMachine.IsReloading && !_sm.GetUseWeaponStateMachine.HasNoMoreBullets)
                    {
                        // Debug.Log("E-SOMEONE SEE ME - I'M HEALTHY - I CAN HANDLE THIS - I'M SHOOTING OR TRYING TO LOOK AT THE PERSON WHO SEE ME" + _sm.transform.name);
                        // Debug.Log("is it closest enemy" + isClosestEnemy + _sm.transform.name);
                        if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();

                        _sm.GetPlayableCharaIdentity.Aiming(true);
                        _sm.SetAllowLookTarget(true, currTarget.position, false);

                        if(!isClosestEnemy && _sm.GetMoveStateMachine.IsIdle)
                        {
                            Vector3 dirTargetToCharaFOV = (currTarget.position - _sm.GetFOVMachine.GetFOVPoint.position).normalized;
                            float dotBetweenCharaWithEnemyLastSeen = Vector3.Dot(dirTargetToCharaFOV, _sm.GetFOVMachine.GetFOVPoint.forward);
                            if(dotBetweenCharaWithEnemyLastSeen >= 0.95f)
                            {
                                float dotRight = Vector3.Dot(dirTargetToCharaFOV, _sm.GetFOVMachine.GetFOVPoint.right);
                                if(dotRight >= 0)
                                {
                                    _sm.GetMoveStateMachine.SetAIDirection(_sm.transform.position + _sm.GetFOVMachine.GetFOVPoint.right);
                                }
                                else
                                {
                                    _sm.GetMoveStateMachine.SetAIDirection(_sm.transform.position - _sm.GetFOVMachine.GetFOVPoint.right);
                                }
                                // Debug.Log("Sometimes they went here" + _sm.transform.name);
                            }
                        }
                        if(isClosestEnemy)_sm.StartShooting(_sm.SearchBestBodyPartToShoot(currTarget));
                        else
                        {
                            _sm.StopShooting();
                        }
                        
                    }
                    
                    if(_sm.GetUseWeaponStateMachine.IsReloading || _sm.GetUseWeaponStateMachine.HasNoMoreBullets)
                    {
                        // Debug.Log("E-SOMEONE SEE ME - I'M HEALTHY - I CAN HANDLE THIS - I'M RELOADING" + _sm.transform.name);
                        _sm.StopShooting();
                        
                        // _sm.GetMoveStateMachine.SetAITargetToLook(_sm.GetFOVMachine.ClosestEnemy != null? _sm.GetFOVMachine.ClosestEnemy.position : _sm.ClosestEnemyWhoSawAI.position, false);
                        _sm.TakingCover();
                        if(_sm.CanTakeCoverInThePosition || _sm.LeaveDirection != Vector3.zero)
                        {
                            // if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                            // if(_sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.IsAiming = false;
                            // _sm.GetMoveStateMachine.IsRunning = true;
                            _sm.GetPlayableCharaIdentity.Run(true);
                            if(_sm.CanTakeCoverInThePosition)
                            {
                                // Debug.Log("E-SOMEONE SEE ME - I'M HEALTHY - I CAN HANDLE THIS - I'M TAKING COVER WHILE RUN" + _sm.TakeCoverPosition + " "+ _sm.transform.name);

                                _sm.GetMoveStateMachine.SetAIDirection(_sm.TakeCoverPosition);
                                _sm.IsTakingCover = true;
                            }
                            else if(_sm.LeaveDirection != Vector3.zero)
                            {
                                // Debug.Log("E-SOMEONE SEE ME - I'M HEALTHY - I CAN HANDLE THIS - I'M RUNNING AWAY WHILE RUN" + _sm.LeaveDirection + " "+ _sm.transform.name);
                                _sm.RunAwayOption();
                            }
                            
                        }
                        else if(!_sm.CanTakeCoverInThePosition && _sm.LeaveDirection == Vector3.zero)
                        {
                            // Debug.Log("E-SOMEONE SEE ME - I'M HEALTHY - I CAN HANDLE THIS - NO PLACE TO HIDE" + _sm. TakeCoverPosition + " "+ _sm.transform.name);
                            if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();

                            _sm.GetPlayableCharaIdentity.Aiming(true);
                            _sm.SetAllowLookTarget(true, currTarget.position, false);
                            
                        }
                    }
                }
                else
                {
                    // Debug.Log("E-SOMEONE SEE ME - I'M HEALTHY - I CANT HANDLE THIS" + _sm.transform.name);

                    if(!_sm.GetUseWeaponStateMachine.IsReloading && !_sm.GetUseWeaponStateMachine.HasNoMoreBullets)
                    {
                        // Debug.Log("E-SOMEONE SEE ME - I'M HEALTHY - I CANT HANDLE THIS - STILL TRYING TO SHOOT" + _sm.transform.name);

                        _sm.GetPlayableCharaIdentity.Aiming(true);
                        _sm.SetAllowLookTarget(true, currTarget.position, false);
                        if(isClosestEnemy)_sm.StartShooting(_sm.SearchBestBodyPartToShoot(currTarget));
                        else
                        {
                            _sm.StopShooting();
                        }
                        
                    }
                    else if(_sm.GetUseWeaponStateMachine.IsReloading || _sm.GetUseWeaponStateMachine.HasNoMoreBullets)
                    {
                        // Debug.Log("E-SOMEONE SEE ME - I'M HEALTHY - I CANT HANDLE THIS - RELOADING BUT LOOKING AT THE PERSON" + _sm.transform.name);
                        _sm.StopShooting();
                        _sm.GetPlayableCharaIdentity.Aiming(true);
                        _sm.SetAllowLookTarget(true, currTarget.position, false);
                    }
                    _sm.TakingCover();
                    if(_sm.CanTakeCoverInThePosition || _sm.LeaveDirection != Vector3.zero)
                    {
                        if(_sm.CanTakeCoverInThePosition)
                        {
                            // Debug.Log("E-SOMEONE SEE ME - I'M HEALTHY - I CANT HANDLE THIS - IM TAKING COVER WHILE LOOKING FRONT" + _sm.TakeCoverPosition + " " + _sm.transform.name);
                            _sm.GetMoveStateMachine.SetAIDirection(_sm.TakeCoverPosition);
                            _sm.IsTakingCover = true;
                        }
                        else if(_sm.LeaveDirection != Vector3.zero)
                        {
                            // Debug.Log("E-SOMEONE SEE ME - I'M HEALTHY - I CANT HANDLE THIS - IM RUNNING AWAY WHILE LOOKING FRONT" + _sm.LeaveDirection + " " + _sm.transform.name);
                            _sm.RunAwayOption();
                        }
                    }
                    else if(!_sm.CanTakeCoverInThePosition && _sm.LeaveDirection == Vector3.zero)
                    {
                        // Debug.Log("E-SOMEONE SEE ME - I'M HEALTHY - I CANT HANDLE THIS - NO PLACE TO RUN WHILE LOOKING FRONT" + _sm.transform.name);
                        if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
                    }
                }
            }
            else
            {
                // Debug.Log("E-SOMEONE SEE ME - I'M NOT HEALTHY" + _sm.transform.name);
                _sm.TakingCover();
                if(_sm.CanTakeCoverInThePosition || _sm.LeaveDirection != Vector3.zero)
                {
                    _sm.GetPlayableCharaIdentity.Run(true);
                    if(_sm.CanTakeCoverInThePosition)
                    {
                        _sm.GetPlayableCharaIdentity.Aiming(false);
                        if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
                        _sm.StopShooting();
                        // Debug.Log("E-SOMEONE SEE ME - I'M NOT HEALTHY - I'M TAKING COVER WHILE RUN" + _sm.TakeCoverPosition + " "+ _sm.transform.name);
                        _sm.GetMoveStateMachine.SetAIDirection(_sm.TakeCoverPosition);
                        _sm.IsTakingCover = true;
                    }
                    else if(_sm.LeaveDirection != Vector3.zero)
                    {
                        // Debug.Log("E-SOMEONE SEE ME - I'M NOT HEALTHY - I'M RUNNING AWAY WHILE RUN" + _sm.LeaveDirection + " "+ _sm.transform.name);
                        _sm.RunAwayOption();
                        _sm.GetPlayableCharaIdentity.Run(false);
                        _sm.GetPlayableCharaIdentity.Aiming(true);
                        _sm.SetAllowLookTarget(true, currTarget.position, false);
                        if(isClosestEnemy)_sm.StartShooting(_sm.SearchBestBodyPartToShoot(currTarget));
                        else
                        {
                            _sm.StopShooting();
                        }
                    }
                    
                }
                else if(!_sm.CanTakeCoverInThePosition && _sm.LeaveDirection == Vector3.zero)
                {
                    // Debug.Log("E-SOMEONE SEE ME - I'M NOT HEALTHY - NO PLACE TO RUN" + _sm.transform.name);
                    if(_sm.GetMoveStateMachine.CurrAIDirPos != _sm.transform.position)_sm.GetMoveStateMachine.ForceStopMoving();
                    _sm.GetPlayableCharaIdentity.Aiming(true);
                    
                    // Debug.Log("E-SOMEONE SEE ME - I'M NOT HEALTHY - SHOOTING BACK" + _sm.transform.name);
                    _sm.SetAllowLookTarget(true, currTarget.position, false);
                    if(!_sm.GetUseWeaponStateMachine.IsReloading && !_sm.GetUseWeaponStateMachine.HasNoMoreBullets)
                    {
                        
                        if(isClosestEnemy)_sm.StartShooting(_sm.SearchBestBodyPartToShoot(currTarget));
                        else
                        {
                            _sm.StopShooting();
                        }
                        
                    }

                    
                    if(_sm.GetUseWeaponStateMachine.IsReloading || _sm.GetUseWeaponStateMachine.HasNoMoreBullets)
                    {
                        _sm.StopShooting();
                    }
                }
            }
        }
     
    }
    public override void ExitState()
    {
        if(_sm.GetMoveStateMachine.AllowLookTarget)_sm.GetMoveStateMachine.AllowLookTarget = false;
        if(_sm.GetMoveStateMachine.IsRunning)_sm.GetMoveStateMachine.IsRunning = false;
        if(_sm.GetUseWeaponStateMachine.IsUsingWeapon || _sm.GetUseWeaponStateMachine.IsAiming)_sm.GetUseWeaponStateMachine.ForceStopUseWeapon();
        _sm.IsAtTakingCoverHidingPlace = false;
        
    }



}
