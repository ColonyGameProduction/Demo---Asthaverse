
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SnipingUIHandler : MonoBehaviour, IUnsubscribeEvent
{
    private GameManager _gm;
    [SerializeField] private GameObject _snipingUI;
    private SniperShootingEvent _event;

    [Header("Mini Character Profile")]
    [SerializeField] private Image _charaFaceImageContainer;
    [SerializeField] private TextMeshProUGUI _charaNameContainer;
    [SerializeField] private Image _weaponImageContainer;
    [SerializeField] private TextMeshProUGUI _weaponNameContainer;
    [SerializeField] private TextMeshProUGUI _currBulletContainer;
    [SerializeField] private TextMeshProUGUI _maxBulletContainer;
    private void Awake() 
    {
        HideSnipingUI();
    }
    private void Start() 
    {
        _gm = GameManager.instance;
        _gm.OnChangeGamePlayModeToEvent += ShowSnipingUI;
        _gm.OnChangeGamePlayModeToNormal += HideSnipingUI;

        _event = SniperShootingEvent.Instance;
        if(_event != null)
        {
            AssignCharaSniperProfileUI();
        }
    }
    private void AssignCharaSniperProfileUI()
    {
        _event.OnWeaponBulletChange += UpdateBulletWeaponData;

        _charaFaceImageContainer.sprite = _event.CharaStat.cropImage;
        _charaNameContainer.text = _event.CharaStat.entityName.ToUpper();

        _weaponImageContainer.sprite = _event.GetWeaponDataSpecial.weaponStatSO.gunShilouete;
        _weaponNameContainer.text = _event.GetWeaponDataSpecial.weaponStatSO.weaponName;
        
        UpdateBulletWeaponData();
        _maxBulletContainer.text = "XXX";
    }
    private void UpdateBulletWeaponData()
    {
        _currBulletContainer.text = ((int)_event.GetWeaponDataSpecial.currBullet).ToString("D3");
        
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

        if(_event != null)
        {
            _event.OnWeaponBulletChange -= UpdateBulletWeaponData;
        }
    }
}
