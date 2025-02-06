using UnityEngine;
using Cinemachine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour, IUnsubscribeEvent
{
    public static GameManager instance;


    [Header("All Playable Character")]
    public GameObject[] playerGameObject;

    [Header("All Camera")]
    public CinemachineVirtualCamera[] followCameras;

    [Header("Sedang memainkan character ke berapa")]
    public int playableCharacterNum;

    [Header("Apakah bisa berganti character")]
    public bool canSwitch;

    [Header("Apakah sudah scope")]
    public bool scope;

    public GameObject[] breadcrumbsGameObject;
    public Action<EnemyAI> enemy;

    public bool gameIsPaused;
    public bool isAtSetting;

    #region new Variable
    private GameInputManager _gameInput;
    
    [Header("States")]
    [ReadOnly(true), SerializeField] private GameState _currGameState;
    [ReadOnly(false), SerializeField] private GamePlayMode _currGamePlayMode;
    [ReadOnly(false), SerializeField] private GamePauseState _currGamePauseState;
    private bool _isPause;
    private FadeBGUIHandler _fadeUIHandler;
    private QuestManager _questManager;

    [Header("Checkpoint")]
    public bool startAtCheckPoint;
    [SerializeField] private List<GameObject> _checkPointList;
    [SerializeField] private Vector3 _friendOffset;
    private int _currCheckPointIdx;
    public const string CHECK_POINT_PREF = "Checkpoint";


    [Header("Event")]
    public Action<bool> OnPlayerPause;
    public Action OnChangeGamePlayModeToEvent, OnChangeGamePlayModeToNormal;
    public Action OnGameOver;
    public Action OnQuitSettings;


    #region GETTER SETTER VARIABLE
    public GameState GetCurrGameState { get { return _currGameState; } }
    public GamePlayMode GetCurrGamePlayMode { get { return _currGamePlayMode; } }
    public GamePauseState GetCurrGamePauseState { get { return _currGamePauseState; } }
    #endregion
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _gameInput = GameInputManager.Instance;
        SubscribeToGameInputManager();

        if(startAtCheckPoint) LoadCheckPoint();
        
        _fadeUIHandler = FadeBGUIHandler.Instance;
        _questManager = QuestManager.Instance;
        _fadeUIHandler.HideBGStart(StartGame);
    }
    
    private void StartGame()
    {
        _questManager.ActivateQuest();
        SetGameState(GameState.Play);
        
    }

    public void FollowCamerasRefrence()
    {
        for (int i = 0; i < playerGameObject.Length; i++)
        {
            followCameras[i] = playerGameObject[i].GetComponentInChildren<CinemachineVirtualCamera>();
        }
    }

    public void CreatingBreadcrumbs()
    {
        breadcrumbsGameObject = new GameObject[10];
        for(int i = 0; i < 10; i++)
        {
            breadcrumbsGameObject[i] = new GameObject("Breadcrumbs");
            breadcrumbsGameObject[i].AddComponent<BoxCollider>().isTrigger = true;
            breadcrumbsGameObject[i].layer = 7;
            breadcrumbsGameObject[i].SetActive(false);
            
        }
    }

    #region NewCode
    #region GameState
    public void PauseGame()
    {
        if(CheckGamePauseState(GamePauseState.Settings))
        {
            OnQuitSettings?.Invoke();
            return;
        }

        
        if(_currGameState != GameState.Play && _currGameState != GameState.Pause) return;
        _isPause = !_isPause;
        OnPlayerPause?.Invoke(_isPause);
        AudioManager.Instance.ChangeBGMVolumeWhenPause(_isPause);
        
        if(_isPause)
        {
            // Debug.Log("Game is Paused");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0f;
            SetGameState(GameState.Pause);
        }
        else
        {
            // Debug.Log("Game is Play");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Time.timeScale = 1f;
            SetGameState(GameState.Play);
        }
    }
    public void GameCompleted()
    {
        SetGameState(GameState.Finish);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void GameOver()
    {
        SetGameState(GameState.GameOver);
        // Time.timeScale = 0f;
        OnGameOver?.Invoke();
    }

    
    public bool IsGameHaventStart() => CheckGameState(GameState.BeforeStart);
    public bool IsGamePlaying() => CheckGameState(GameState.Play);
    public bool IsGamePause() => CheckGameState(GameState.Pause);
    public bool IsGameCinematic() => CheckGameState(GameState.Cinematic);
    public bool IsGameFinish() => CheckGameState(GameState.Finish);
    public bool IsGameOver() => CheckGameState(GameState.GameOver);

    private void SetGameState(GameState newState)
    {
        _currGameState = newState;
        if(_currGameState != GameState.Pause) SetGamePauseState(GamePauseState.None);
        else OpenPauseMenu();
    }
    private bool CheckGameState(GameState state) => _currGameState == state;

    #endregion

    #region GamePauseState
    public void OpenSettingsMenu() => SetGamePauseState(GamePauseState.Settings);
    public void OpenPauseMenu() => SetGamePauseState(GamePauseState.PauseMenu);
    public bool IsNotPauseState() => CheckGamePauseState(GamePauseState.None);
    public bool IsStatePauseMenu() => CheckGamePauseState(GamePauseState.PauseMenu);
    public bool IsStateSettingsMenu() => CheckGamePauseState(GamePauseState.Settings);

    private void SetGamePauseState(GamePauseState newState) => _currGamePauseState = newState;
    private bool CheckGamePauseState(GamePauseState state) => _currGamePauseState == state;
    #endregion

    #region GamePlayMode
    public void SetToSwitchGamePlayMode() => SetGamePlayMode(GamePlayMode.Switching);
    public void SetToNormalGamePlayMode() => SetGamePlayMode(GamePlayMode.Normal);
    public void SetToEventGamePlayMode() => SetGamePlayMode(GamePlayMode.Event);

    public bool IsNormalGamePlayMode() => CheckGamePlayMode(GamePlayMode.Normal);
    public bool IsEventGamePlayMode() => CheckGamePlayMode(GamePlayMode.Event);
    private void SetGamePlayMode(GamePlayMode newMode) => _currGamePlayMode = newMode;
    private bool CheckGamePlayMode(GamePlayMode mode) => _currGamePlayMode == mode;
    #endregion

    private void SubscribeToGameInputManager()
    {
        if(_gameInput != null)_gameInput.OnPauseGamePerformed += PauseGame;
    }

    public void UnsubscribeEvent()
    {
        _gameInput.OnPauseGamePerformed -= PauseGame;
    }
    #endregion

    #region CheckPoint
    private void LoadCheckPoint()
    {
        _currCheckPointIdx = PlayerPrefs.GetInt(CHECK_POINT_PREF, 0);
        SpawnAtCheckPoint();
    }
    private void SpawnAtCheckPoint()
    {
        Vector3 startPos = _checkPointList[_currCheckPointIdx].transform.position;
        for(int i=0; i< playerGameObject.Length; i++)
        {
            Vector3 charaPos = playerGameObject[i].transform.position;
            startPos.y = charaPos.y;

            playerGameObject[i].transform.position = startPos;
            startPos += _friendOffset;
        }
    }
    public int GetCurrCheckPointIdx() => _currCheckPointIdx;
    public void SaveCheckPoint(int idx)
    {
        _currCheckPointIdx = idx;
        PlayerPrefs.SetInt(CHECK_POINT_PREF, idx);
    }
    #endregion

}
