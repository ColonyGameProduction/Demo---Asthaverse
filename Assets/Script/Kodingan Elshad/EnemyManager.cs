using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public List<Transform> POIList;
    public List<Transform> POIPosList;
    public List<Transform> tempPOIPosList;
    public List<EnemyAI> enemyList;
    public List<EnemyAI> enemyCaptain;
    public Action<EnemyAI> enemyHunted;
    public Action<EnemyAI> enemyEngage;
    public Action<EnemyAI> lastPosIsFound;
    public Action closestPOI;
    public Action isEngaging;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        lastPosIsFound += LastPosIsFound;
    }

    public void LastPosIsFound(EnemyAI enemy)
    {
        


        for (int i = 0; i < POIList.Count; i++)
        {
            if (Vector3.Distance(enemy.transform.position, POIList[i].position) < 50f)
            {
                if(!tempPOIPosList.Contains(POIList[i]))
                {
                    POIPosList.Add(POIList[i]);
                    tempPOIPosList.Add(POIList[i]);
                }                
            }
        }

        closestPOI?.Invoke();


    }

}
