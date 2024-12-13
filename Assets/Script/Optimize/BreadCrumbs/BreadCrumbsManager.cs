using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadCrumbsManager : MonoBehaviour, IUnsubscribeEvent
{
    private GameManager _gm;
    private PlayableCharacterManager _playableCharaManager;
    [SerializeField] private Transform _breadCrumbsPrefab;
    [SerializeField] private Transform _instantiateParent;
    [SerializeField] private List<BreadCrumbs> _breadCrumbsList;

    [SerializeField] private float _breadCrumbsSpawnDelayMax, _breadCrumbsGoneMaxTimer;
    private float _breadCrumbsSpawnDelay;
    [SerializeField] private int _totalBreadCrumbs = 50;
    
    private Transform _currPlayable;
    private int _currBreadCrumbs;
    private IEnumerator _currCoroutine;
    
    private void Awake() 
    {
        if(_playableCharaManager == null)_playableCharaManager = GetComponent<PlayableCharacterManager>();

        _playableCharaManager.OnPlayerSwitch += PlayableCharaManager_OnPlayerSwitch;
    }


    private void Start() 
    {
        _gm = GameManager.instance;
        CreatingBreadcrumbs();
        _currCoroutine = BreadCrumbsDrop();
        // StartCoroutine(_currCoroutine);

        _breadCrumbsSpawnDelay = _breadCrumbsSpawnDelayMax;
        
    }
    private void Update() 
    {
        if(!_gm.IsGamePlaying()) return;
        
        if(_breadCrumbsSpawnDelay > 0) _breadCrumbsSpawnDelay -= Time.deltaTime;
        else
        {
            _breadCrumbsSpawnDelay = _breadCrumbsSpawnDelayMax;
            BreadcrumbsFollowPlayer();
        }
    }

    public void CreatingBreadcrumbs()
    {
        _breadCrumbsList = new List<BreadCrumbs>(_totalBreadCrumbs);

        for(int i=0; i < _totalBreadCrumbs; i++)
        {
            Transform newBreadCrumbs = Instantiate(_breadCrumbsPrefab, _instantiateParent);
            newBreadCrumbs.gameObject.SetActive(false);
            newBreadCrumbs.gameObject.name = "Breadcrumbs";
            BreadCrumbs _new = newBreadCrumbs.GetComponent<BreadCrumbs>();
            _breadCrumbsList.Add(_new);
        }

        _currBreadCrumbs = 0;
    }
    
    private IEnumerator BreadCrumbsDrop()
    {
        while (true)
        {
            yield return new WaitForSeconds(_breadCrumbsSpawnDelayMax);
            BreadcrumbsFollowPlayer();
        }
    }

    public void BreadcrumbsFollowPlayer()
    {
        if(_currPlayable == null)return;

        _breadCrumbsList[_currBreadCrumbs].BreadCrumbsGoneTime = _breadCrumbsGoneMaxTimer;
        _breadCrumbsList[_currBreadCrumbs].transform.position = _currPlayable.transform.position;
        _breadCrumbsList[_currBreadCrumbs].transform.forward = _currPlayable.transform.forward;
        _breadCrumbsList[_currBreadCrumbs].gameObject.SetActive(true);

        _currBreadCrumbs++;
        if (_currBreadCrumbs == _breadCrumbsList.Count) _currBreadCrumbs = 0;

    }
    public void StopCoroutine()
    {
        if(_currCoroutine == null) return;
        StopCoroutine(_currCoroutine);
        _currCoroutine = null;
    }

    private void PlayableCharaManager_OnPlayerSwitch(Transform transform)
    {
        _currPlayable = transform;
    }

    public void UnsubscribeEvent()
    {
        _playableCharaManager.OnPlayerSwitch -= PlayableCharaManager_OnPlayerSwitch;
    }
}
