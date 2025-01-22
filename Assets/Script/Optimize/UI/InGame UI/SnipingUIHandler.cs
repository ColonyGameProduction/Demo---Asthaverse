using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnipingUIHandler : MonoBehaviour, IUnsubscribeEvent
{
    [SerializeField] private GameObject _snipingUI;
    private GameManager _gm;
    private void Awake() 
    {
        HideSnipingUI();
    }
    private void Start() 
    {
        _gm = GameManager.instance;
        _gm.OnChangeGamePlayModeToEvent += ShowSnipingUI;
        _gm.OnChangeGamePlayModeToNormal += HideSnipingUI;
    }
    public void ShowSnipingUI()
    {
        _snipingUI.SetActive(true);
    }
    public void HideSnipingUI()
    {
        _snipingUI.SetActive(false);
    }

    public void UnsubscribeEvent()
    {
        _gm.OnChangeGamePlayModeToEvent -= ShowSnipingUI;
        _gm.OnChangeGamePlayModeToNormal -= HideSnipingUI;
    }
}
