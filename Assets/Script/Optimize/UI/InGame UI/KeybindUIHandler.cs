using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class KeybindUIHandler : MonoBehaviour, IUnsubscribeEvent
{
    [Header("Keybind General")]
    private KeybindUIType _currType;
    [SerializeField] private Image _keybindImage;

    [SerializeField] private Sprite _generalSprite;
    [SerializeField] private Sprite _pickUpSprite;
    [SerializeField] private Sprite _commandSprite;
    public static Action<KeybindUIType> OnChangeKeybind;

    [Header("Keybind Floating")]
    [SerializeField] private GameObject _silentTakeDownContainer;
    public static Action<bool> OnShowSilentTakeDownKeybind;
    [SerializeField] private GameObject _interactContainer;
    public static Action<bool> OnShowInteractKeybind;
    [SerializeField] private GameObject _pickUpContainer;
    public static Action<bool> OnShowPickUpKeybind;
    [SerializeField] private GameObject _reviveContainer;
    public static Action<bool> OnShowReviveKeybind;

    private void Awake() 
    {
        ToggleSilentTakeDownKeybindContainer(false);
        ToggleInteractKeybindContainer(false);
        TogglePickUpKeybindContainer(false);
        ToggleReviveKeybindContainer(false);

        OnChangeKeybind += ChangeKeybindUI;
        ChangeKeybindUI(KeybindUIType.General);

        OnShowSilentTakeDownKeybind += ToggleSilentTakeDownKeybindContainer;
        OnShowInteractKeybind += ToggleInteractKeybindContainer;
        OnShowPickUpKeybind += TogglePickUpKeybindContainer;
        OnShowReviveKeybind += ToggleReviveKeybindContainer;
    }

    private void ToggleSilentTakeDownKeybindContainer(bool show)
    {
        _silentTakeDownContainer.SetActive(show);
    }
    private void ToggleInteractKeybindContainer(bool show)
    {
        _interactContainer.SetActive(show);
    }
    private void TogglePickUpKeybindContainer(bool show)
    {
        _pickUpContainer.SetActive(show);
    }
    private void ToggleReviveKeybindContainer(bool show)
    {
        _reviveContainer.SetActive(show);
    }
    
    private void ChangeKeybindUI(KeybindUIType newType)
    {
        if(newType == _currType) return;

        _currType = newType;
        if(_currType == KeybindUIType.General)
        {
            _keybindImage.sprite = _generalSprite;
        }
        else if(_currType == KeybindUIType.PickUp)
        {
            _keybindImage.sprite = _pickUpSprite;
        }
        else if(_currType == KeybindUIType.Command)
        {
            _keybindImage.sprite = _commandSprite;
        }
    }

    public void UnsubscribeEvent()
    {
        OnChangeKeybind -= ChangeKeybindUI;
        OnShowSilentTakeDownKeybind -= ToggleSilentTakeDownKeybindContainer;
        OnShowInteractKeybind -= ToggleInteractKeybindContainer;
        OnShowPickUpKeybind -= TogglePickUpKeybindContainer;
        OnShowReviveKeybind -= ToggleReviveKeybindContainer;
    }
}
