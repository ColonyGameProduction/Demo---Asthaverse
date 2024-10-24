using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Diagnostics.CodeAnalysis;
using System;

public class GameManager : MonoBehaviour
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

    [Header("States")]
    [SerializeField] private GameState _currState;
    private bool _isPause;

    [Header("Event")]
    public Action<bool> OnPlayerPause;

    public Action<EnemyAI> enemy;

    #region GETTER SETTER VARIABLE
    public GameState GetCurrState { get { return _currState; } }
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _currState = GameState.Play;

        playableCharacterNum = 0;
        FollowCamerasRefrence();
        CreatingBreadcrumbs();
        canSwitch = true;
        scope = false;
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

    public void Pause()
    {
        _isPause = !_isPause;
        OnPlayerPause?.Invoke(_isPause);
        
        if(_isPause)
        {
            _currState = GameState.Pause;
        }
        else
        {
            _currState = GameState.Play;
        }
    }

    
}
