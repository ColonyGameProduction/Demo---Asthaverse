using UnityEngine;
using Cinemachine;
using System;

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


    #region new Variable
    private GameInputManager _gameInput;
    
    [Header("States")]
    [SerializeField] private GameState _currState;
    private bool _isPause;

    [Header("Event")]
    public Action<bool> OnPlayerPause;


    public bool gameIsPaused;
    public bool isAtSetting;
    #region GETTER SETTER VARIABLE
    public GameState GetCurrState { get { return _currState; } }
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

        playableCharacterNum = 0;
        FollowCamerasRefrence();
        CreatingBreadcrumbs();
        canSwitch = true;
        scope = false;

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
    public void PauseGame()
    {
        _isPause = !_isPause;
        OnPlayerPause?.Invoke(_isPause);
        
        if(_isPause)
        {
            Debug.Log("Game is Paused");
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0f;
            SetGameState(GameState.Pause);
        }
        else
        {
            Debug.Log("Game is Play");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Time.timeScale = 1f;
            SetGameState(GameState.Play);
        }
    }
    public bool IsGameHaventStart() => CheckGameState(GameState.BeforeStart);
    public bool IsGamePlaying() => CheckGameState(GameState.Play);
    public bool IsGamePause() => CheckGameState(GameState.Pause);
    public bool IsGameCinematic() => CheckGameState(GameState.Cinematic);
    public bool IsGameFinish() => CheckGameState(GameState.Finish);

    private void SetGameState(GameState newState) => _currState = newState;
    private bool CheckGameState(GameState state) => _currState == state;


    private void SubscribeToGameInputManager()
    {
        if(_gameInput != null)_gameInput.OnPauseGamePerformed += PauseGame;
    }

    public void UnsubscribeEvent()
    {
        _gameInput.OnPauseGamePerformed -= PauseGame;
    }
    #endregion


}
