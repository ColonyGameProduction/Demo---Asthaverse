
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ArrowDamageVisualDirData
{   
    public GameObject arrow;
    public Transform whoShootMe;
    public Image arrowImg;
    public float stayTime;
    public float fadeTime;
}
public class DamageUIHandler : MonoBehaviour
{
    [Header("Damage Visual Phase UI")]
    [SerializeField] private GameObject _dmgVisualContainer;
    [SerializeField] private Image  _dmgVisualPhase2;
    [SerializeField] private Image  _dmgVisualPhase3, _dmgVisualPhase4;

    [Header("Damage Direction Indicator UI")]
    [SerializeField] private GameObject _dmgDirContainer;
    [SerializeField] private GameObject _dmgDirArrowContainer;
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private CanvasGroup _upBloodDamage, _rightBloodDamage, _leftBloodDamage, _bottomBloodDamage;
    [SerializeField] private float _dmgDirStayTime, _dmgDirFadeTime; 
    private float _dmgDirAngle;

    private List<ArrowDamageVisualDirData> _arrowList = new List<ArrowDamageVisualDirData>();
    private List<ArrowDamageVisualDirData> _tempList = new List<ArrowDamageVisualDirData>();

    // [ReadOnly(false), SerializeField] private Transform _currPlayable;
    private Transform _cameraTransform;

    private void Awake() 
    {
        _cameraTransform = Camera.main.transform;

        ResetDamageVisual();
        if(!_dmgVisualContainer.activeSelf)_dmgVisualContainer.SetActive(true);
    }
    
    private void Update() 
    {
        DamageArrowDirAngleHandler();
    }

    #region  Damage Visual Phase
    public void SetDamageVisualPhase(float currHP, float maxHP)
    {
        float healthDifference = maxHP - currHP;

        float imageAlphaPhase2 = (healthDifference - (maxHP/4)) / (maxHP / 4);
        ChangeImageAlphaValue(_dmgVisualPhase2, imageAlphaPhase2);

        float imageAlphaPhase3 = (healthDifference - (maxHP / 2)) / (maxHP / 4);
        ChangeImageAlphaValue(_dmgVisualPhase3, imageAlphaPhase3);

        float imageAlphaPhase4 = (healthDifference - ((maxHP / 4) * 3)) / (maxHP / 4);
        ChangeImageAlphaValue(_dmgVisualPhase4, imageAlphaPhase4);
        
    }

    private void ChangeImageAlphaValue(Image chosen, float to)
    {
        to = Mathf.Clamp01(to);

        Color colorChosen = chosen.color;
        colorChosen.a = to;
        chosen.color = colorChosen;
    }
    #endregion

    #region Visual Direction Function
    public void SpawnArrow(Transform whoShoot)
    {
        ArrowDamageVisualDirData currArrowData = null;

        foreach(ArrowDamageVisualDirData arrowData in _arrowList)
        {
            if(arrowData.whoShootMe == whoShoot)
            {
                currArrowData = arrowData;
                break;
            }
        }

        if (currArrowData == null)
        {
            GameObject arrowGameObject = Instantiate(_arrowPrefab, _dmgDirArrowContainer.transform);
            currArrowData = new ArrowDamageVisualDirData();
            currArrowData.arrow = arrowGameObject;
            currArrowData.arrowImg = arrowGameObject.GetComponentInChildren<Image>();
            currArrowData.whoShootMe = whoShoot;
            _arrowList.Add(currArrowData);
        }
        
        ChangeImageAlphaValue(currArrowData.arrowImg, 1);
        DamageVisualDirectionIndicator(currArrowData);
        
    }
    private void DamageVisualDirectionIndicator(ArrowDamageVisualDirData currArrowData)
    {
        currArrowData.stayTime = _dmgDirStayTime;
        currArrowData.fadeTime = _dmgDirFadeTime;

        DamageArrowDirectionIndicator(currArrowData);

        if(_dmgDirAngle >= -45 && _dmgDirAngle < 45)
        {
            ShowDamageVisualDirectionIndicator(_upBloodDamage);
        }
        else if(_dmgDirAngle >= 45 && _dmgDirAngle < 135)
        {
            ShowDamageVisualDirectionIndicator(_leftBloodDamage);
        }
        else if(_dmgDirAngle >= 135 || _dmgDirAngle < -135)
        {
            ShowDamageVisualDirectionIndicator(_bottomBloodDamage);
        }
        else if(_dmgDirAngle < -45 && _dmgDirAngle >= -135)
        {
            ShowDamageVisualDirectionIndicator(_rightBloodDamage);
        }
    }
    private void ShowDamageVisualDirectionIndicator(CanvasGroup _dmgDirUI)
    {
        LeanTween.cancel(_dmgDirUI.gameObject);

        _dmgDirUI.alpha = 1;
        LeanTween.delayedCall(_dmgDirStayTime, () =>
        {
            LeanTween.alphaCanvas(_dmgDirUI, 0, _dmgDirFadeTime);
        });
}

    private void DamageArrowDirectionIndicator(ArrowDamageVisualDirData currArrowData)
    {
        Vector3 damageLocation = currArrowData.whoShootMe.transform.position;

        damageLocation.y = _cameraTransform.position.y;

        Vector3 flatForward = _cameraTransform.forward;
        flatForward.y = 0;
        flatForward.Normalize();

        Vector3 direction = (damageLocation - _cameraTransform.position).normalized;

        _dmgDirAngle = Vector3.SignedAngle(direction, flatForward, Vector3.up);
        currArrowData.arrow.transform.localEulerAngles = new Vector3(0, 0, _dmgDirAngle);
    }

    private void DamageArrowDirAngleHandler()
    {
        _tempList = _arrowList;

        for(int i=0; i < _tempList.Count;i++)
        {
            DamageArrowDirectionIndicator(_tempList[i]);
            if(_tempList[i].stayTime > 0)
            {
                _tempList[i].stayTime -= Time.deltaTime;
            }
            else
            {
                
                if(_tempList[i].fadeTime > 0)
                {
                    _tempList[i].fadeTime -= Time.deltaTime;

                    float newAlpha = _tempList[i].fadeTime / _dmgDirFadeTime;
                    ChangeImageAlphaValue(_tempList[i].arrowImg, newAlpha);

                }
                else
                {                    
                    Destroy(_tempList[i].arrow); 
                    _arrowList.Remove(_tempList[i]);
                }
            }
        }
    }
    #endregion
    public void ResetDamageVisual()
    {
        ChangeImageAlphaValue(_dmgVisualPhase2, 0);
        ChangeImageAlphaValue(_dmgVisualPhase3, 0);
        ChangeImageAlphaValue(_dmgVisualPhase4, 0);
    }
}
