using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Animator[] animators;
    public bool SetNormalWalkBoth;
    public bool SetNormal, SetCrazy;
    // Start is called before the first frame update
    public bool Stop;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(SetNormalWalkBoth)
        {
            SetNormalWalkBoth = false;
            animators[0].SetFloat("Vertical", 1);
            animators[1].SetFloat("Vertical", 1);
            animators[0].SetBool("Aim", true);
            animators[1].SetBool("Aim", true);
        }

        if(SetNormal && SetCrazy)
        {
            SetNormal = false;
            SetCrazy = false;
            animators[0].SetFloat("Vertical", 1);
            animators[1].SetFloat("Vertical", 1);
            animators[0].SetFloat("Speed", 1);
            // animators[1].SetFloat("Speed", 1);
            animators[0].SetBool("Aim", true);
            animators[1].SetBool("Aim", true);
        }
        if(Stop)
        {
            Stop = false;
            animators[0].SetFloat("Vertical", 0);
            animators[1].SetFloat("Vertical", 0);
            animators[0].SetFloat("Speed", 0);
            // animators[1].SetFloat("Speed", 1);
            animators[0].SetBool("Aim", false);
            animators[1].SetBool("Aim", false);
        }
    }
}
