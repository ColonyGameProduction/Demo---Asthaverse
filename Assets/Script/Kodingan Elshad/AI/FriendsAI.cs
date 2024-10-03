using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class FriendsAI : ExecuteLogic
{

    GameManager gm;
    private GameObject[] destination = new GameObject[4];

    public int friendsID;

    private bool commandActive = false;
    private bool holdPositionActive = false;

    private PlayerAction activePlayerAction;


    private void Start()
    {
        gm = GameManager.instance;
    }

    void Update()
    {
        FindActivePlayerAction();

        // Setiap frame, periksa status dari isCommand
        if (activePlayerAction != null && activePlayerAction.enabled)
        {
            bool isCommandActive = activePlayerAction.isCommandActive;
            bool isHoldPositionActive = activePlayerAction.isHoldPosition;

            // Lakukan sesuatu berdasarkan status isCommand
            if (isCommandActive)
            {
                // Logika ketika isCommand aktif
                commandActive = true;
            }
            else
            {
                // Logika ketika isCommand tidak aktif
                commandActive = false;
            }

            if (isHoldPositionActive)
            {
                holdPositionActive = true;
            }
            else
            {
                holdPositionActive = false;
            }
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        DetectingPosition();
        DetectingPlayer();
        //if(commandFollow)
        //{
        //
        //}
        CommandFollow();
    }

    private void CommandFollow()
    {
        if (commandActive == true || holdPositionActive == true)
        {
            if (friendsID == 1)
            {
                MoveToDestination(GetNavMesh(), destination[2].transform.position);
            }
            else if (friendsID == 2)
            {
                MoveToDestination(GetNavMesh(), destination[3].transform.position);
            }
        }
        else
        {
            if (friendsID == 1)
            {
                MoveToDestination(GetNavMesh(), destination[0].transform.position);
            }
            else if (friendsID == 2)
            {
                MoveToDestination(GetNavMesh(), destination[1].transform.position);
            }
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

    public void DetectingPosition()
    {
        for (int i = 0; i < gm.playerGameObject.Length; i++)
        {
            if (gm.playableCharacterNum == i)
            {
                destination[2] = gm.playerGameObject[i].GetComponent<PlayerAction>().GetPositionGameObject()[0];
                destination[3] = gm.playerGameObject[i].GetComponent<PlayerAction>().GetPositionGameObject()[1];
            }
        }
    }

    private void FindActivePlayerAction() 
    { 
        // Cari semua PlayerAction
        PlayerAction[] allPlayerActions = FindObjectsOfType<PlayerAction>();

        // Temukan yang aktif
        foreach (PlayerAction playerAction in allPlayerActions)
        {
            if (playerAction.enabled)
            {
                activePlayerAction = playerAction;
                break;
            }
        }
    }

    public NavMeshAgent GetNavMesh()
    {
        return this.GetComponent<NavMeshAgent>();
    }
}
