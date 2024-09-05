using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : ExecuteLogic
{
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > 2)
        {
            Shoot();
            timer = 0;

            
        }        
    }      

    

}
