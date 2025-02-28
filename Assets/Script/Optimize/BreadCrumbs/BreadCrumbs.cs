using UnityEngine;

public class BreadCrumbs : MonoBehaviour
{
    private GameManager _gm;
    private float _breadCrumbsTimeCounter;

    public float BreadCrumbsGoneTime { set {_breadCrumbsTimeCounter = value;}}
    private void Start() 
    {
        _gm = GameManager.instance;
    }
    private void Update()
    {
        if(!_gm.IsGamePlaying()) return;
        
        if(_breadCrumbsTimeCounter > 0) _breadCrumbsTimeCounter -= Time.deltaTime;
        else if(_breadCrumbsTimeCounter <= 0) gameObject.SetActive(false);
    }
}
