using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Diagnostics.CodeAnalysis;

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

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
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
        for(int i = 0;i < 10;i++)
        {
            breadcrumbsGameObject[i] = new GameObject();
        }

        for (int i = 0; i < 10; i++)
        {
            Instantiate(breadcrumbsGameObject[i], Vector3.zero, Quaternion.identity);
        }
    }
}
