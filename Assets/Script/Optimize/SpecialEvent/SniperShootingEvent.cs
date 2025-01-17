using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperShootingEvent : MonoBehaviour, IUnsubscribeEvent
{
    // [Header("Test")]
    public bool StartEvent, StopEvent;
    [Space(1)]
    [SerializeField] private GameObject _shootingGameObjectContainer;
    [SerializeField] private EntityStatSO _specialCharaStatSO;
    
    #region private non serializefield
    private PlayableCameraSniperEvent _playableCamera;
    private WeaponLogicManager _weaponLogicManager;
    private GameInputManager _gameInputManager;
    private GameManager _gm;
    private WeaponShootVFX _weaponShootVFX;
    private FadeBGUIHandler _fadeUIHandler;
    private PlayableCharacterManager _playableCharacterManager;
    private PlayableSkill _playableSkill;
    #endregion
    [Header("Shooting Variable")]
    [ReadOnly(false), SerializeField] private WeaponData _currWeaponData;
    [ReadOnly(false), SerializeField] private bool _isReloading, _isfireRateOn, _isShooting;
    [ReadOnly(false), SerializeField] private bool _canReload = true, _isDoingReloading;
    [SerializeField] protected float _notAimSpamClickDelayTime = 0.1f;
    [SerializeField] private LayerMask _charaEnemyMask;
    private float _charaAimAccuracy;
    private Transform _cameraTransform;

    #region Recoil
    [Header("Recoil Data")]
    [SerializeField]protected float _recoilCoolDownBuffer = 0.1f;
    protected float _currRecoil, _maxRecoil, _recoilCoolDown, _finalCountRecoil;
    #endregion

    [Header("Quest Connecting with Event")]
    [Space(1)]
    [SerializeField] private QuestHandlerParent _questToStartEvent;
    [SerializeField] private QuestHandlerParent _questToStopEvent;

    [Header("Place to Look")]
    [SerializeField] private List<Transform> _placeToLookList;
    [ReadOnly(true)] private int _currPlaceToLookIdx = 0;

    #region Getter Setter
    public float FinalCountRecoil
    {
        get
        {
            if(_recoilCoolDown > 0) return _finalCountRecoil;
            return 0;
        }
    }
    public float AimAccuracy {get {return _charaAimAccuracy;}}
    public bool IsShooting 
    { 
        get {return _isShooting;} 
        set{ 
            if(value) 
            {
                if(_currWeaponData.currBullet > 0) _isShooting = value;
            }
            
            else _isShooting = value;
            
        } 
    }
    public bool IsReloading 
    { 
        get {return _isReloading;} 
        set{ 
            if(value)
            {
                if(_currWeaponData.totalBullet > 0)
                {
                    if(_canReload)_isReloading = value;
                }
            }
            else _isReloading = value;
        }
    }
    #endregion

    private void Awake() 
    {
        _playableCamera = GetComponent<PlayableCameraSniperEvent>();
        _weaponShootVFX = GetComponent<WeaponShootVFX>();
        _cameraTransform = Camera.main.transform;
        _playableSkill = GetComponent<PlayableSkill>();

        _questToStartEvent.OnQuestCompleted += TriggerStartEvent;
        _questToStopEvent.OnQuestCompleted += TriggerStopEvent;
        InitializeGunData();
    }
    private void InitializeGunData()
    {
        WeaponStatSO weaponStat = _specialCharaStatSO.weaponStat[0];
        _currWeaponData = new WeaponData(weaponStat);
        _currWeaponData.totalBullet = _currWeaponData.weaponStatSO.magSize * _currWeaponData.weaponStatSO.magSpare;
        _currWeaponData.currBullet = _currWeaponData.weaponStatSO.magSize;

        _weaponShootVFX.CurrWeaponIdx = 0;
        
        _weaponShootVFX.SpawnTrail((int)(weaponStat.magSize * weaponStat.bulletPerTap), _cameraTransform.position, weaponStat.bulletTrailPrefab, weaponStat.gunFlashPrefab);

        _charaAimAccuracy = _specialCharaStatSO.acuracy;
    }
    private void Start() 
    {
        _weaponLogicManager = WeaponLogicManager.Instance;
        _gameInputManager = GameInputManager.Instance;
        _playableCharacterManager = PlayableCharacterManager.Instance;
        
        SubscribeToGameInputManager();

        _gm = GameManager.instance;
        _fadeUIHandler = FadeBGUIHandler.Instance;
    }
    private void Update() 
    {
        if(StartEvent)
        {
            StartEvent = false;
            SetCameraPosToLook();
        }

    }
    private void FixedUpdate()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsEventGamePlayMode()) return;

        ComplexRecoil();
        if(!_isReloading)
        {
            if(_isShooting) Shoot();
        }
        else
        {
            if(_canReload && !_isDoingReloading)
            {
                ReloadWeapon();
            }
        }

    }

    private void SubscribeToGameInputManager()
    {
        _gameInputManager.OnShootingPerformed += GameInputManager_OnShootingPerformed;
        _gameInputManager.OnShootingCanceled += GameInputManager_OnShootingCanceled;
        _gameInputManager.OnReloadPerformed += GameInputManager_OnReloadPerformed;
    }
    public void UnsubscribeEvent()
    {
        _questToStartEvent.OnQuestCompleted -= TriggerStartEvent;
        _questToStopEvent.OnQuestCompleted -= TriggerStopEvent;

        _gameInputManager.OnShootingPerformed -= GameInputManager_OnShootingPerformed;
        _gameInputManager.OnShootingCanceled -= GameInputManager_OnShootingCanceled;
        _gameInputManager.OnReloadPerformed -= GameInputManager_OnReloadPerformed;
    }

    private void GameInputManager_OnShootingPerformed()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsEventGamePlayMode()) return;

        IsShooting = true;
    }

    private void GameInputManager_OnShootingCanceled()
    {
        IsShooting = false;
    }

    private void GameInputManager_OnReloadPerformed()
    {
        if(!_gm.IsGamePlaying() || !_gm.IsEventGamePlayMode()) return;

        IsReloading = true;
    }

    public void TriggerStartEvent()
    {
        _gm.SetToSwitchGamePlayMode();
        _fadeUIHandler.FadeBG(1, 0.8f, StartSniperShootingEvent);
    }
    public void TriggerStopEvent()
    {
        _gm.SetToSwitchGamePlayMode();
        _fadeUIHandler.FadeBG(1, 0.8f, StopSniperShootingEvent);
    }
    public void StartSniperShootingEvent()
    {
        SetCameraPosToLook();
        _gm.OnChangeGamePlayModeToEvent?.Invoke();
        _playableCharacterManager.SetCurrPlayableSkill(_playableSkill);
        _shootingGameObjectContainer.SetActive(true);

        _fadeUIHandler.FadeBG(0, 0.2f, ()=> _gm.SetToEventGamePlayMode());
    }
    public void StopSniperShootingEvent()
    {
        _gm.OnChangeGamePlayModeToNormal?.Invoke();
        _playableCharacterManager.SetCurrPlayableSkill(null);
        _shootingGameObjectContainer.SetActive(false);

        IsShooting = false;
        IsReloading = false;
        _isDoingReloading = false;
        _canReload = true;
        _finalCountRecoil = 0;

        _fadeUIHandler.FadeBG(0, 0.2f, ()=> _gm.SetToNormalGamePlayMode());

    }
    protected void Shoot()
    {
        if(_currWeaponData.currBullet > 0 && !_isfireRateOn)
        {
            RecoilHandler();
            _playableCamera.GiveRecoilToCamera();
            _weaponLogicManager.ShootingPerformed(this.transform, _cameraTransform.position, _cameraTransform.forward.normalized, _charaAimAccuracy, _currWeaponData.weaponStatSO, _charaEnemyMask, 0, _cameraTransform.position, false, false, _weaponShootVFX);
            _currWeaponData.currBullet -= 1;
            if(!_currWeaponData.weaponStatSO.allowHoldDownButton) _isShooting = false;
            StartCoroutine(FireRate(_currWeaponData.weaponStatSO.fireRate));

            if (_currWeaponData.currBullet == 0)
            {
                if(_currWeaponData.totalBullet > 0)
                {
                    if(!_canReload)_canReload = true;
                    IsReloading = true;
                }
                else 
                {
                    _isShooting = false;
                }
                
            }
            
        }
    }

    protected IEnumerator FireRate(float fireRateTime)
    {
        _isfireRateOn = true;

        yield return new WaitForSeconds(fireRateTime);

        _isfireRateOn = false;
    }

    public void ReloadWeapon()
    {
        StartCoroutine(ReloadWeaponActive(_currWeaponData.weaponStatSO.reloadTime));
    }
    protected IEnumerator ReloadWeaponActive(float reloadTime)
    {
        _isDoingReloading = true;

        yield return new WaitForSeconds(reloadTime);

        ReloadAnimationFinished();
        
    }
    public void ReloadAnimationFinished()
    {
        if(!_isReloading) return;

        
        float bulletNeed = _currWeaponData.weaponStatSO.magSize - _currWeaponData.currBullet;
        if (_currWeaponData.totalBullet >= bulletNeed)
        {
            _currWeaponData.currBullet = _currWeaponData.weaponStatSO.magSize;
            _currWeaponData.totalBullet -= bulletNeed;
        }
        else if (_currWeaponData.totalBullet > 0)
        {
            _currWeaponData.currBullet += _currWeaponData.totalBullet;
            _currWeaponData.totalBullet = 0;
        } 


        _canReload = false;
        _isDoingReloading = false;
        IsReloading = false;
        CanReloadWeapon_Coroutine();
    }
    
    public void CanReloadWeapon_Coroutine()
    {
        StartCoroutine(CanReloadWeapon());
    }
    protected IEnumerator CanReloadWeapon()
    {
        yield return new WaitForSeconds(_notAimSpamClickDelayTime);
        _canReload = true;
    }

    protected virtual void RecoilHandler()
    {
        _recoilCoolDown = _currWeaponData.weaponStatSO.fireRate + (_currWeaponData.weaponStatSO.fireRate * _recoilCoolDownBuffer);
        _maxRecoil = _currWeaponData.weaponStatSO.recoil;
        _finalCountRecoil = _currRecoil;
    }
    protected virtual void ComplexRecoil()
    {
        if (_recoilCoolDown > 0)
        {
            _recoilCoolDown -= Time.deltaTime;
            if (_currRecoil <= _maxRecoil) // Ini batasnya sampai maxrecoil ato lebihin max recoil ?
            {
                _currRecoil += Time.deltaTime * _currWeaponData.weaponStatSO.recoil;
            }
            // else if(_currRecoil > _maxRecoil)_currRecoil = 
        }
        else
        {
            _currRecoil = _currWeaponData.weaponStatSO.recoil;
        }
    }
    private void SetCameraPosToLook()
    {
        _playableCamera.SetCameraToLookAt(_placeToLookList[_currPlaceToLookIdx]);
        _currPlaceToLookIdx++;
    }
    
}
