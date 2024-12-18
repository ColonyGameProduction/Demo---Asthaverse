using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAlertUIHandler : MonoBehaviour
{
    private GameManager _gm;
    private Camera _camera;
    private List<EnemyAlertUI> _enemyAlertUIList;
    private void Awake() 
    {
        _camera = Camera.main;
        _enemyAlertUIList = FindObjectsOfType<EnemyAlertUI>().ToList();
    }
    private void Start() 
    {
        _gm = GameManager.instance;
    }

    private void Update() 
    {
        if(!_gm.IsGamePlaying()) return;

        CheckEnemyAlertUICameraVisibility();
        foreach(EnemyAlertUI enemy in _enemyAlertUIList)
        {
            enemy.AlertUIDirectionIndicator();
        }
    }

    public void CheckEnemyAlertUICameraVisibility()
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(_camera);

        foreach(EnemyAlertUI enemy in _enemyAlertUIList)
        {
            Transform point = enemy.AlertEnemyUIExclamationContainer.transform;
            bool isVisible = true;
            foreach(var plane in planes)
            {
                if(plane.GetDistanceToPoint(point.position) < 0)
                {
                    isVisible = false;
                    break;
                }
            }
            enemy.IsAlertUIAtEnemyVisibleOnCam = isVisible;
        }
    }
}
