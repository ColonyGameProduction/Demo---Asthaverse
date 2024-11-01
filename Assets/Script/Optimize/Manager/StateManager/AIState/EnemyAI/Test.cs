using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    float timemax = 0.2f;
    int count = 0;
    public bool yes;
    public Transform test;
    public Transform lookat;

    public bool ssaaaa;
    public NavMeshAgent navMeshAgent;
    public Transform cek;
    void Start()
    {
        // StartCoroutine(testEnum());
        // StartCoroutine(endofframe());
    }
    public IEnumerator testEnum()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.2f);
            ++count; 
            Debug.Log("Helo from Coro " + gameObject.activeSelf + this.enabled);
        }
    }
    public IEnumerator endofframe()
    {
        while(true)
        {
            yield return new WaitForEndOfFrame();
            Debug.Log("Fram end fr fr");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if(timemax > 0)
        // {
        //     timemax -= Time.deltaTime;
        // }
        // else if (timemax <=0)
        // {
        //     timemax = 0.2f;
            
        //     Debug.Log("Helo from Update " + gameObject.activeSelf + this.enabled + count);
        //     // if(count > 3)Debug.LogError("" + gameObject.activeSelf);
        // }
        // // Debug.Log("Frame end" + count);
        // if(focus != null)Debug.DrawRay(transform.position, focus.position, Color.black);
        if(yes)
        {
            test.LookAt(new Vector3(lookat.position.x, lookat.position.y + 1.5f, lookat.position.z));
        }
        if(ssaaaa)
        {
            ssaaaa = false;
            navMeshAgent.SetDestination(cek.position);
        }
    }
}
