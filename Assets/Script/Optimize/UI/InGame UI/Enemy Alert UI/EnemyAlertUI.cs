using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAlertUI : MonoBehaviour, IUnsubscribeEvent
{
    private GameManager _gm;
    private EnemyAIBehaviourStateMachine _enemyAIBehaviourStateMachine;
    private Camera _camera;

    [Header("Fill Sprite List")]
    [SerializeField] private Sprite _backGroundFillSpriteExclamation;
    [SerializeField] private Sprite _fillSpriteExclamation;

    [SerializeField] private Sprite _backGroundFillSpriteNormal;
    [SerializeField] private Sprite _fillSpriteNormal;

    [Header("UI List - Exclamation")]
    [SerializeField] private GameObject _alertUIExclamationContainer;
    [SerializeField] private Image _backGroundImageExclamation, _fillImageExclamation;

    [Header("UI List - Normal")]
    [ReadOnly(false), SerializeField] private GameObject _alertUINormalContainer;
    private Image _backGroundImageNormal, _fillImageNormal;
    [SerializeField] private Vector3 normalScale;
    [SerializeField] private GameObject _alertUICanvasParent;

    [Header("Chosen")]
    [ReadOnly(false), SerializeField] private GameObject _chosenUIContainer;
    [ReadOnly(false), SerializeField] private Image _chosenFillImage;
    [ReadOnly(false), SerializeField] private bool _isAlertUIAtEnemyVisibleOnCam = true;
    [ReadOnly(true), SerializeField] private bool _isShowUI;
    [ReadOnly(false), SerializeField] private float _fillImageValue = 0;

    public bool IsAlertUIAtEnemyVisibleOnCam 
    {
        get {return _isAlertUIAtEnemyVisibleOnCam;} 
        set
        {
            if(_isAlertUIAtEnemyVisibleOnCam != value)
            {
                _isAlertUIAtEnemyVisibleOnCam = value;
                AlertUIContainerToggleHandler();
            }
            else
            {
                _isAlertUIAtEnemyVisibleOnCam = value;
            }
        }
    }
    public GameObject AlertEnemyUIExclamationContainer {get {return _alertUIExclamationContainer;}}
    // public GameObject AlertEnemyUINormalContainer {get {return _alertUINormalContainer;}}


    private void Awake() 
    {
        _camera = Camera.main;
        _enemyAIBehaviourStateMachine = GetComponentInParent<EnemyAIBehaviourStateMachine>();
        _enemyAIBehaviourStateMachine.OnAlertValueChanged += ChangeAlertFillVisual;

        SpawnAlertUINormal();
        AlertUIContainerToggleHandler();

        SetAllFillImage(_fillImageValue);
        _fillImageExclamation.fillMethod = Image.FillMethod.Vertical;
    }
    private void Start() 
    {
        _gm = GameManager.instance;
        HideAllAlertUIContainer();

    }
    private void SpawnAlertUINormal()
    {
        _alertUINormalContainer = Instantiate(_alertUIExclamationContainer, _alertUICanvasParent.transform);
        _alertUINormalContainer.SetActive(false);

        _alertUINormalContainer.GetComponent<RectTransform>().localScale = normalScale;
        _backGroundImageNormal = _alertUINormalContainer.transform.GetChild(0).GetComponent<Image>();
        _fillImageNormal = _alertUINormalContainer.transform.GetChild(1).GetComponent<Image>();

        _backGroundImageNormal.sprite = _backGroundFillSpriteNormal;
        _fillImageNormal.sprite = _fillSpriteNormal;
        _fillImageNormal.fillMethod = Image.FillMethod.Horizontal;
    }

    public void ChangeAlertFillVisual(float currAlertValue, float maxAlertValue)
    {
        if(maxAlertValue == 0) _fillImageValue = 0;
        else _fillImageValue = currAlertValue / maxAlertValue;
        SetFillImage(_fillImageValue);

        AlertUIVisibilityChecker();
    }
    private void AlertUIVisibilityChecker()
    {
        if (_fillImageValue <= 0)
        {
            _isShowUI = false;
        }
        else if (_fillImageValue > 0 && _fillImageValue < 1)
        {
            _isShowUI = true;
        }
        else if (_fillImageValue >= 1)
        {
            _isShowUI = false;
        }
        if(_isShowUI) ShowAlertUIContainer();
        else HideAlertUIContainer();
    }

    public void AlertUIContainerToggleHandler()
    {
        HideAllAlertUIContainer();
        if(_isAlertUIAtEnemyVisibleOnCam)
        {
            _chosenUIContainer = _alertUIExclamationContainer;
            _chosenFillImage = _fillImageExclamation;
        }
        else
        {
            _chosenUIContainer = _alertUINormalContainer;
            _chosenFillImage = _fillImageNormal;
        }

        SetFillImage(_fillImageValue);
        if(_isShowUI) ShowAlertUIContainer();
        else HideAlertUIContainer();
    }
    
    public void AlertUIDirectionIndicator()
    {
        if(!_isShowUI) return;
        if(_isAlertUIAtEnemyVisibleOnCam) AlertUIExclamationDirectionIndicator();
        else AlertUINormalDirectionIndicator();
    }
    private void AlertUINormalDirectionIndicator()
    {
        // if(_isAlertUIAtEnemyVisibleOnCam || (!_isAlertUIAtEnemyVisibleOnCam && !_isShowUI)) return;

        Vector3 enemyLocation = _enemyAIBehaviourStateMachine.transform.position;
        Transform cameraTransform = _camera.transform;

        enemyLocation.y = cameraTransform.position.y;

        Vector3 flatForward = cameraTransform.forward;
        flatForward.y = 0;
        flatForward.Normalize();

        Vector3 direction = (enemyLocation - cameraTransform.position).normalized;

        float angle = Vector3.SignedAngle(direction, flatForward, Vector3.up);
        _alertUINormalContainer.transform.localEulerAngles = new Vector3(0, 0, angle);
    }
    private void AlertUIExclamationDirectionIndicator()
    {
        // if(!_isAlertUIAtEnemyVisibleOnCam || (_isAlertUIAtEnemyVisibleOnCam && !_isShowUI)) return;


        Vector3 cameraPos = _camera.transform.position;
        cameraPos.y = _alertUIExclamationContainer.transform.position.y;
        _alertUIExclamationContainer.transform.LookAt(cameraPos);
    }

    private void ShowAlertUIContainer()
    {
        if(_chosenUIContainer != null) ToggleAlertUIContainer(_chosenUIContainer, true);
    }
    private void HideAlertUIContainer()
    {
        if(_chosenUIContainer != null) ToggleAlertUIContainer(_chosenUIContainer, false);
    }
    private void HideAllAlertUIContainer()
    {
        ToggleAlertUIContainer(_alertUIExclamationContainer, false);
        ToggleAlertUIContainer(_alertUINormalContainer, false);
    }
    private void SetAllFillImage(float value)
    {
        _fillImageExclamation.fillAmount = value;
        _fillImageNormal.fillAmount = value;
    }
    private void SetFillImage(float value)
    {
        if(_chosenFillImage != null) _chosenFillImage.fillAmount = value;
    }
    private void ToggleAlertUIContainer(GameObject alertUIContainer,bool change)=> alertUIContainer.SetActive(change);

    public void RemoveAlertUINormal()
    {
        Destroy(_alertUINormalContainer);
    }

    public void UnsubscribeEvent()
    {
        _enemyAIBehaviourStateMachine.OnAlertValueChanged -= ChangeAlertFillVisual;
    }
}
