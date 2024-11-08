using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIManager : MonoBehaviour
{   
    #region Normal Variable
    public static EnemyAIManager Instance { get; private set;}
    [Header("List")]
    [SerializeField] private List<Transform> _POIPosList;
    [SerializeField] private List<Transform> _POIPosNearLastSeenPosList;
    [SerializeField] private List<Transform> _POIPosNearLastSeenPosListSave;
    [SerializeField] private List<EnemyAIBehaviourStateMachine> _enemyCaptainList;
    [SerializeField] private List<EnemyAIBehaviourStateMachine> _enemyHearAnnouncementList;
    [SerializeField] private float _enemyAnnouncementMaxRange;
    [SerializeField] private float _lastPostToPOIMaxRange;

    [Header("Event")]
    public Action<EnemyAIBehaviourStateMachine> OnCaptainsStartHunting;
    public Action<EnemyAIBehaviourStateMachine> OnCaptainsStartEngaging;
    public Action<EnemyAIBehaviourStateMachine> OnFoundLastCharaSeenPos;
    public Action OnGoToClosestPOI;
    public Action OnEnemyisEngaging;
    public Action OnEnemyStopEngaging;
    public Action<EnemyAIBehaviourStateMachine> OnEnemyDead;

    [Header("Engage timer for telling")]
    [SerializeField] protected float _isEngageTimer;
    [SerializeField] protected float _isEngageTimerMax = 0.3f;
    private bool hasToShoutStopEngage;
    private bool _isEnemyEngaging;
    #endregion
    
    #region GETTER SETTER VARIABLE
    public List<Transform> POIPosList {get {return _POIPosList;}}
    public List<Transform> POIPosNearLastSeenPosList {get {return _POIPosNearLastSeenPosList;}}
    public List<Transform> POIPosNearLastSeenPosListSave {get {return _POIPosNearLastSeenPosListSave;}}
    public List<EnemyAIBehaviourStateMachine> EnemyCaptainList {get {return _enemyCaptainList;}}
    public List<EnemyAIBehaviourStateMachine> EnemyHearAnnouncementList {get {return _enemyHearAnnouncementList;}}
    public float EnemyAnnouncementMaxRange {get {return _enemyAnnouncementMaxRange;}}
    public bool IsEnemyEngaging { get {return _isEnemyEngaging;} }

    #endregion
    private void Awake() 
    {
        if(Instance == null) Instance = this;
    }
    private void Start()
    {
        OnFoundLastCharaSeenPos += FindAllPOINearLastSeenPos;
        OnEnemyisEngaging += EnemyisEngaging;
    }
    private void Update() 
    {
        EngageTimerCounter();
    }

    private void FindAllPOINearLastSeenPos(EnemyAIBehaviourStateMachine enemy)
    {

        for (int i = 0; i < POIPosList.Count; i++)
        {
            if (Vector3.Distance(enemy.transform.position, POIPosList[i].position) < _lastPostToPOIMaxRange)
            {
                if(!POIPosNearLastSeenPosListSave.Contains(POIPosList[i]))
                {
                    POIPosNearLastSeenPosList.Add(POIPosList[i]);
                    POIPosNearLastSeenPosListSave.Add(POIPosList[i]);
                }                
            }
        }
        
        OnGoToClosestPOI?.Invoke();
    }

    public void EditEnemyCaptainList(EnemyAIBehaviourStateMachine enemy, bool isAdding)
    {
        if(isAdding)
        {
            if(!_enemyCaptainList.Contains(enemy))_enemyCaptainList.Add(enemy);
        }
        else
        {
            if(_enemyCaptainList.Contains(enemy))_enemyCaptainList.Remove(enemy);
        }
    }
    public void EditEnemyHearAnnouncementList(EnemyAIBehaviourStateMachine enemy, bool isAdding)
    {
        if(isAdding)
        {
            if(!_enemyHearAnnouncementList.Contains(enemy))_enemyHearAnnouncementList.Add(enemy);
        }
        else
        {
           if(_enemyHearAnnouncementList.Contains(enemy)) _enemyHearAnnouncementList.Remove(enemy);
        }
    }
    
    public Vector3 GetClosestLastSeenPosInfoFromCaptain(Transform currAskerLastSeenPos)
    {
        float closestLastSeenPosDistance = Mathf.Infinity;
        Vector3 closestLastSeenPos = Vector3.zero;

        foreach(EnemyAIBehaviourStateMachine enemy in EnemyCaptainList)
        {
            float lastSeenPosDistance = Vector3.Distance(enemy.GetFOVAdvancedData.EnemyCharalastSeenPosition, currAskerLastSeenPos.position);
            if(lastSeenPosDistance < closestLastSeenPosDistance)
            {
                closestLastSeenPosDistance = lastSeenPosDistance;
                closestLastSeenPos = enemy.GetFOVAdvancedData.EnemyCharalastSeenPosition;
            }
        }
        return closestLastSeenPos;
    }
    public void ResetPOIPosNearLastSeenPos()
    {
        POIPosNearLastSeenPosList.Clear();
        POIPosNearLastSeenPosListSave.Clear();
    }

    public Transform GetClosestPOI(EnemyAIBehaviourStateMachine currAsker)
    {
        Transform closestPOI = null;
        if(!EnemyHearAnnouncementList.Contains(currAsker))
        {
            return closestPOI;
        }

        float closestPOIDistance = Mathf.Infinity;
        if(POIPosNearLastSeenPosList.Count > 0)
        {
            foreach (Transform currPOI in POIPosNearLastSeenPosList)
            {
                float POIDistance = Vector3.Distance(currAsker.transform.position, currPOI.position);
                if (closestPOIDistance > POIDistance)
                {
                    closestPOIDistance = POIDistance;
                    closestPOI = currPOI;
                }
            }
            if(closestPOI != null)POIPosNearLastSeenPosList.Remove(closestPOI);
        }
        else
        {
            foreach (Transform currPOI in POIPosNearLastSeenPosListSave)
            {
                float POIDistance = Vector3.Distance(currAsker.transform.position, currPOI.position);
                if (closestPOIDistance > POIDistance)
                {
                    closestPOIDistance = POIDistance;
                    closestPOI = currPOI;
                }
            }
        }
        
        return closestPOI;
    }
    private void EnemyisEngaging()
    {
        _isEngageTimer = _isEngageTimerMax;
        _isEnemyEngaging = true;
        if(!hasToShoutStopEngage)hasToShoutStopEngage = true;
    }
    public void EngageTimerCounter()
    {
        if(_isEngageTimer > 0)
        {
            _isEngageTimer -= Time.deltaTime;   
        }
        else
        {
            if(hasToShoutStopEngage)
            {
                _isEngageTimer = 0f;
                OnEnemyStopEngaging?.Invoke();
                _isEnemyEngaging = false;
                hasToShoutStopEngage = false;
            }
            
        }
    }

}
