
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandUIHandler : MonoBehaviour
{
    private GameInputManager _gameInputManager;
    private AudioManager _am;
    [SerializeField] private List<Button> _commandButton = new List<Button>();

    private int _currentIDX = 0;
    private bool _isPressed = false;

    private void Start() 
    {
        _gameInputManager = GameInputManager.Instance;

        _am = AudioManager.Instance;
        foreach(Button button in _commandButton)
        {
            button.onClick.AddListener(_am.PlayUIClick);
        }
    }
    void Update()
    {

        if (_isPressed)
        {
            float scrollDelta = _gameInputManager.GetCommandValue();

            if(scrollDelta > 0)
            {
                NextButton();
            }
            else if(scrollDelta < 0)
            {
                PrevButton();
            }
        }

    }
    public void OpenCommandUI()
    {
        KeybindUIHandler.OnChangeKeybind?.Invoke(KeybindUIType.Command);

        _currentIDX = 0;
        _isPressed = true;
        SelectButton(_currentIDX);
        _commandButton[_currentIDX].gameObject.SetActive(true);
    }
    public void CloseCommandUI()
    {
        KeybindUIHandler.OnChangeKeybind?.Invoke(KeybindUIType.General);

        _isPressed = false;
        if(EventSystem.current.currentSelectedGameObject == _commandButton[_currentIDX].gameObject)
        {
            _commandButton[_currentIDX].onClick.Invoke();
        }
        EventSystem.current.SetSelectedGameObject(null);
        _commandButton[_currentIDX].gameObject.SetActive(false);
    }

    public void NextButton()
    {
        _commandButton[_currentIDX].gameObject.SetActive(false);
        _currentIDX = (_currentIDX + 1) % _commandButton.Count;
        SelectButton(_currentIDX);
        _commandButton[_currentIDX].gameObject.SetActive(true);
    }

    public void PrevButton()
    {
        _commandButton[_currentIDX].gameObject.SetActive(false);
        _currentIDX = (_currentIDX - 1 + _commandButton.Count) % _commandButton.Count;
        SelectButton(_currentIDX);
        _commandButton[_currentIDX].gameObject.SetActive(true);
    }

    public void SelectButton(int index)
    {
        EventSystem.current.SetSelectedGameObject(_commandButton[index].gameObject);

        _am.PlayUIClick();
    }
}
