
using UnityEngine;

public class EnemySnipingEventUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject _enemySnipingTargetUI;
    private Camera _camera;
    private void Awake() 
    {
        _camera = Camera.main;
        HideUI();
    }
    private void Update() 
    {
        AlertUIExclamationDirectionIndicator();
    }
    public void ShowUI()
    {
        _enemySnipingTargetUI.SetActive(true);
    }
    public void HideUI()
    {
        _enemySnipingTargetUI.SetActive(false);
    }
    private void AlertUIExclamationDirectionIndicator()
    {
        // if(!_isAlertUIAtEnemyVisibleOnCam || (_isAlertUIAtEnemyVisibleOnCam && !_isShowUI)) return;


        Vector3 cameraPos = _camera.transform.position;
        cameraPos.y = _enemySnipingTargetUI.transform.position.y;
        _enemySnipingTargetUI.transform.LookAt(cameraPos);
    }
}
