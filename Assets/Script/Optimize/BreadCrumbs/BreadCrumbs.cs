using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadCrumbs : MonoBehaviour
{
    private float _breadCrumbsTimeCounter;

    public float BreadCrumbsGoneTime { set {_breadCrumbsTimeCounter = value;}}
    private void Update()
    {
        if(_breadCrumbsTimeCounter > 0) _breadCrumbsTimeCounter -= Time.deltaTime;
        else if(_breadCrumbsTimeCounter <= 0) gameObject.SetActive(false);
    }
}
