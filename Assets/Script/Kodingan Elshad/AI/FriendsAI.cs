using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class FriendsAI : ExecuteLogic
{

    GameManager gm;
    private GameObject[] destination = new GameObject[2];

    public int friendsID;

    

    private void Start()
    {
        gm = GameManager.instance;
    }
    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        DetectingPlayer();

        if(friendsID == 1)
        {
            MoveToDestination(GetNavMesh(), destination[0]);
        }
        else if(friendsID == 2)
        {
            MoveToDestination(GetNavMesh(), destination[1]);
        }
    }

    public void DetectingPlayer()
    {
        for(int i = 0; i<gm.playerGameObject.Length; i++)
        {
            if(gm.playableCharacterNum == i)
            {
                destination[0] = gm.playerGameObject[i].GetComponent<PlayerAction>().GetDestinationGameObject()[0];
                destination[1] = gm.playerGameObject[i].GetComponent<PlayerAction>().GetDestinationGameObject()[1];
            }
        }
    }

    public NavMeshAgent GetNavMesh()
    {
        return this.GetComponent<NavMeshAgent>();
    }
}
