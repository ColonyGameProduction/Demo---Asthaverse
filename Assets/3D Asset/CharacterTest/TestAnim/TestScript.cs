using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Animator[] animators;
    public bool SetNormalWalkBoth;
    public bool SetNormal, SetCrazy;
    // Start is called before the first frame update

    [Space(5)]    
    public string nameAnim;
    public bool Starts, Stops;

    public int x;
    public bool setHor, setVer, setidctr;

    [Space(5)] 

    public bool Stop;


    
    void Start()
    {
        
    }
    public void SetHorizontal()
    {
        foreach(Animator an in animators)
        {
            an.SetFloat("Horizontal", x);
        }
    }
    public void SetVertical()
    {
        foreach(Animator an in animators)
        {
            an.SetFloat("Vertical", x);
        }
    }
    public void StartAnim()
    {
        foreach(Animator an in animators)
        {
            an.SetBool(nameAnim, true);
        }
    }
    public void StopAnim()
    {
        foreach(Animator an in animators)
        {
            an.SetBool(nameAnim, false);
        }
    }
    public void SetIdleCtr()
    {
        foreach(Animator an in animators)
        {
            an.SetFloat("IdleCounter", x);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Starts)
        {
            Starts=false;
            StartAnim();
        }
        if(Stops)
        {
            Stops=false;
            StopAnim();
        }
        if(setVer)
        {
            setVer = false;
            SetVertical();
        }
        if(setHor)
        {
            setHor = false;
            SetHorizontal();
        }
        if(setidctr)
        {
            setidctr = false;
            SetIdleCtr();
        }




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
