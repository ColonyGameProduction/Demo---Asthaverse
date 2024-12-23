using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.AI;
using System;
using System.Security.Cryptography;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.InputSystem.Android;
using Unity.VisualScripting;
using UnityEditor.Rendering;

/* PERHATIAN!!!
 * Kalo mau akses logic di skrip ini
 * Public class nya extend ke class ini
 * Jangan ke MonoBehaviour
 */

public class ExecuteLogic : AILogic
{

    public Collider[] walls;
    public float buffer = -1.5f;

    //setelah di extend, klean bisa make function ini tanpa perlu refrence

    public void StartingSetup()
    {
        GameManager gm = GameManager.instance;
        gm.playerGameObject[1].GetComponent<FriendsAI>().friendsID = 1;
        gm.playerGameObject[2].GetComponent<FriendsAI>().friendsID = 2;
    }

    public void BreadcrumbsFollowPlayer(PlayerAction playerAction, ref int currBreadcrumbs)
    {
        GameManager gm = GameManager.instance;


        if (currBreadcrumbs < gm.breadcrumbsGameObject.Length - 1)
        {
            gm.breadcrumbsGameObject[currBreadcrumbs].SetActive(true);
            gm.breadcrumbsGameObject[currBreadcrumbs].transform.position = playerAction.transform.position;
            gm.breadcrumbsGameObject[currBreadcrumbs].transform.forward = playerAction.transform.forward;
            currBreadcrumbs++;
        }
        else if (currBreadcrumbs == gm.breadcrumbsGameObject.Length - 1)
        {
            gm.breadcrumbsGameObject[currBreadcrumbs].SetActive(true);
            gm.breadcrumbsGameObject[currBreadcrumbs].transform.position = playerAction.transform.position;
            gm.breadcrumbsGameObject[currBreadcrumbs].transform.forward = playerAction.transform.forward;
            currBreadcrumbs = 0;
        }


    }

    //untuk Reloading
    public void Reload(WeaponStatSO weaponStatSO)
    {
        float bulletNeed = weaponStatSO.magSize - weaponStatSO.currBullet;
        if (weaponStatSO.totalBullet >= bulletNeed)
        {
            weaponStatSO.currBullet = weaponStatSO.magSize;
            weaponStatSO.totalBullet -= bulletNeed;
        }
        else if (weaponStatSO.totalBullet > 0)
        {
            weaponStatSO.currBullet += weaponStatSO.totalBullet;
            weaponStatSO.totalBullet = 0;
        }        

    }

    //untuk ganti weapon
    public void ChangeWeapon(PlayerAction playerAction, WeaponStatSO[] weaponStats, int weaponNum)
    {
        if (weaponNum >= weaponStats.Length - 1)
        {
            weaponNum = 0;
        }
        else
        {
            weaponNum++;
            if (weaponStats[weaponNum] == null)
            {
                weaponNum--;
            }

        }
        Debug.Log(weaponStats[weaponNum].weaponName);
        playerAction.SetCurrentWeapon(weaponStats[weaponNum], weaponNum);
    }

    
    //logic 'Shoot'
    public void Shoot(GameObject whoShoot, Vector3 origin, Vector3 direction, EntityStatSO entityStat, WeaponStatSO weaponStat, LayerMask entityMask, float recoil)
    {
        WeaponLogicHandler weaponHandler = new WeaponLogicHandler();
        weaponHandler.ShootingPerformed(this.gameObject, origin, direction, entityStat, weaponStat, entityMask, recoil);
    }    


    //logic 'SilentKill'
    public void SilentKill()
    {
        SilentKill silentKill = GetComponentInChildren<SilentKill>();
        silentKill.canKill = true;
    }

    //Logic 'Scope'
    public void Scope()
    {
        GameManager gm = GameManager.instance;

        if(!gm.scope)
        {
            gm.followCameras[gm.playableCharacterNum].m_Lens.FieldOfView = 40;
            gm.scope = true;
        }
        else
        {
            gm.followCameras[gm.playableCharacterNum].m_Lens.FieldOfView = 70;
            gm.scope = false;
        }

    }

    //Logic 'Command'
    public void Command()
    {
        //ShowMouseCursor();

        PlayerAction playerAction = GetComponent<PlayerAction>();
        ChangeKeybindsUI changeKeybind = FindObjectOfType<ChangeKeybindsUI>();

        changeKeybind.isCommand = true;

        playerAction.isCommandActive = true;
        playerAction.command.SetActive(true);
    }

    //Logic 'UnCommand'
    public void UnCommand()
    {
        PlayerAction playerAction = GetComponent<PlayerAction>();
        ChangeKeybindsUI changeKeybind = FindObjectOfType<ChangeKeybindsUI>();

        changeKeybind.isCommand = false;

        if (playerAction.isCommandActive == true)
        {
            HideMouseCursor();

            playerAction.isCommandActive = false;
            playerAction.command.SetActive(false);
        }
    }

    //Logic 'Hold Position'
    public void HoldPosition()
    {
        PlayerAction playerAction = GetComponent<PlayerAction>();

        if (playerAction.isCommandActive == true)
        {
            playerAction.isHoldPosition = true;
        }
    }

    //Logic 'UnHold Position'
    public void UnHoldPosition()
    {
        PlayerAction playerAction = GetComponent<PlayerAction>();

        if (playerAction.isCommandActive == true)
        {
            playerAction.isHoldPosition = false;
        }
    }

    public void Interact()
    {
        Vector3 rayOrigin = Camera.main.transform.position;
        Vector3 rayDirection = Camera.main.transform.forward.normalized;

        Debug.DrawRay(rayOrigin, rayDirection * 100f, Color.magenta, 2f);

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, 100f, LayerMask.GetMask("Interactable")))
        {
            PlayerAction playerAction = GetComponent<PlayerAction>();

            ChangeKeybindsUI changeKeybind = FindObjectOfType<ChangeKeybindsUI>();

            if (hit.collider.GetComponent<PickableItems>())
            {
                Debug.Log("Ambil!");

                GameObject newObject = hit.collider.gameObject;

                if (playerAction.heldObject == null)
                {
                    changeKeybind.isPickUp = true;

                    playerAction.heldObject = newObject;
                    PickupObject(playerAction.heldObject);
                }
                else
                {
                    SwapObject(playerAction.heldObject, newObject);
                    playerAction.heldObject = newObject;
                    PickupObject(playerAction.heldObject);
                }
            }
            else if (hit.collider.GetComponent<OpenableObject>())
            {
                Debug.Log("Buka!");
            }
        }
    }

    private void PickupObject(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        PlayerAction playerAction = GetComponent<PlayerAction>();

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        obj.transform.position = playerAction.holdPoint.position;
        obj.transform.rotation = playerAction.holdPoint.rotation;
        obj.transform.SetParent(playerAction.holdPoint);
    }

    public void ThrowObject()
    {
        PlayerAction playerAction = GetComponent<PlayerAction>();
        ChangeKeybindsUI changeKeybind = FindObjectOfType<ChangeKeybindsUI>();

        Rigidbody rb = playerAction.heldObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            changeKeybind.isPickUp = false;
            rb.isKinematic = false;
            rb.AddForce(Camera.main.transform.forward * 10f, ForceMode.VelocityChange);

            playerAction.heldObject.transform.SetParent(null);
            playerAction.heldObject = null;
        }
    }

    private void SwapObject(GameObject heldObject, GameObject newObject)
    {
        Rigidbody heldRb = heldObject.GetComponent<Rigidbody>();
        if (heldRb != null)
        {
            heldRb.isKinematic = false;
        }

        heldObject.transform.SetParent(null);
        heldObject.transform.position = newObject.transform.position;
        heldObject.transform.rotation = newObject.transform.rotation;
    }

    public void PlayFootstepsSound(AudioSource footstepsSource)
    {
        if (!footstepsSource.isPlaying)
        {
            footstepsSource.Play();
            NotifyEnemies(footstepsSource);
        }
    }

    public void PlayWhistleSound(AudioSource whistleSource)
    {
        whistleSource.Play();
        NotifyEnemies(whistleSource);
    }

    // ini tuh logic buat ngasih tau si enemy kalo yang lagi dia denger tuh audio apa
    public void NotifyEnemies(AudioSource soundSource)
    {
        EnemySoundDetection[] enemies = FindObjectsOfType<EnemySoundDetection>();

        foreach (EnemySoundDetection enemy in enemies)
        {
            enemy.HearSound(gameObject, soundSource); // ngasih tau setiap enemy dia dengan sound apa
        }
    }

    //untuk taking cover
    public void TakingCover(NavMeshAgent agent, List<Transform> target)
    {
        walls = Physics.OverlapSphere(agent.transform.position, 20f, LayerMask.GetMask("Wall"));
        int wallsLength = walls.Length;

        for(int i = 0; i < walls.Length; i++)
        {
            foreach(Transform targetPos in target)
            {
                if (Vector3.Distance(walls[i].transform.position, targetPos.position) < 5f)
                {
                    wallsLength--;
                    walls[i] = null;
                    break;
                }
            }
        }

        if(wallsLength <= 0)
        {
            return;
        }


        System.Array.Sort(walls, ColliderSortArrayComparer);

        if(wallsLength > 7)
        {
            wallsLength = 7;
        }

        for (int i = 0; i <= wallsLength - 1; i++)
        {

            if (NavMesh.SamplePosition(walls[i].transform.position, out NavMeshHit hit, 5f, agent.areaMask))
            {
                if (NavMesh.FindClosestEdge(hit.position, out hit, agent.areaMask))
                {
                    Vector3 directionToTarget = HitDirection(target, hit.position);
                    if (Vector3.Dot(hit.normal, directionToTarget) < 0) // Jika wall ada di antara agent dan target
                    {
                        if (CountNavMeshPathDistance(agent.transform, hit.position, agent) > 100f)
                        {
                            Debug.Log("Masuk Continue");
                            continue;
                        }
                        Vector3 edgePos = CheckPositionBasedOnWall(hit.position, walls[i]);
                        if (walls[i].transform.localScale.y > agent.height)
                        {
                            hit.position = edgePos;
                        }

                        agent.SetDestination(hit.position);
                        break;
                    }
                    else
                    {
                        Vector3 coverPosition = walls[i].transform.position - directionToTarget * 2;
                        if (NavMesh.SamplePosition(coverPosition, out NavMeshHit hit2, 5f, agent.areaMask))
                        {

                            //if (Vector3.Dot(hit2.normal, directionToTarget) < 0)
                            //{
                            //    agent.SetDestination(hit2.position);
                            //    break;
                            //}

                            Debug.Log("hit position before find edge 2 = " + hit.position);
                            if (NavMesh.FindClosestEdge(hit.position, out hit2, agent.areaMask))
                            {
                                Debug.Log("hit position after find edge 2 = " + hit.position);
                                directionToTarget = HitDirection(target, hit.position);
                                if (Vector3.Dot(hit2.normal, directionToTarget) < 0) // Jika wall ada di antara agent dan target
                                {
                                    if (CountNavMeshPathDistance(agent.transform, hit.position, agent) > 100f)
                                    {
                                        Debug.Log("Masuk Continue");
                                        continue;
                                    }

                                    if (walls[i].transform.localScale.y > agent.height)
                                    {
                                        hit2.position = CheckPositionBasedOnWall(hit2.position, walls[i]);
                                    }

                                    agent.SetDestination(hit2.position);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public Vector3 HitDirection(List<Transform> target, Vector3 position)
    {
        Vector3 direction = Vector3.zero;
        Vector3 totalDir = Vector3.zero;

        if (target.Count > 0)
        {
            direction = (target[0].position - position).normalized;

            for (int i = 0; i < target.Count; i++)
            {
                totalDir += direction;
            }
        }

        return totalDir;
    }

    public Vector3 CheckPositionBasedOnWall(Vector3 targetPos, Collider wallColl)
    {
        bool isLeft = false;
        float ySafe = targetPos.y;

        // targetPos = new Vector3(targetPos.x, 0, targetPos.z);
        Vector3 wallCenter = wallColl.bounds.center;
        Vector3 wallFwd = wallColl.transform.forward;
        Vector3 wallRight = wallColl.transform.right;

        float halfWallLength = wallColl.transform.localScale.z * 0.5f;
        float halfWallWidth = wallColl.transform.localScale.x * 0.5f;
        
        Vector3 wallToTarget = targetPos - wallCenter;

        float forwardDistance = Vector3.Dot(wallToTarget, wallFwd);
        float rightDistance = Vector3.Dot(wallToTarget, wallRight);
        
        Vector3 newPos = wallCenter;
   
        
        float newHalfWallWidth = 0;
        float newHalfWallLength = 0;

        if(Mathf.Abs(forwardDistance) > halfWallLength)
        {
            newHalfWallWidth = halfWallWidth + buffer;
            newHalfWallLength = halfWallLength - buffer;
            // isX = true;
            newPos += wallRight * (rightDistance >= 0 ? newHalfWallWidth : -newHalfWallWidth);
            newPos += wallFwd * Mathf.Clamp(forwardDistance, -newHalfWallLength, newHalfWallLength);
            if(rightDistance >= 0)
            {
                if(forwardDistance >= 0) isLeft = false;
                else isLeft = true;
            }
            else
            {
                if(forwardDistance > 0) isLeft = true;
                else isLeft = false;
            }
            
        }
        else
        {
            newHalfWallLength = halfWallLength + buffer;
            newHalfWallWidth = halfWallWidth - buffer;
            // isX = false;
            newPos += wallFwd * (forwardDistance >= 0 ? newHalfWallLength : -newHalfWallLength);
            newPos += wallRight * Mathf.Clamp(rightDistance, -newHalfWallWidth, newHalfWallWidth);
            if(forwardDistance >= 0)
            {
                if(rightDistance > 0) isLeft = true;
                else isLeft = false;
            }
            else
            {
                if(rightDistance >= 0) isLeft = false;
                else isLeft = true;
            }
        }   
        newPos = new Vector3(newPos.x, ySafe, newPos.z);
        return newPos;
    }

    public int ColliderSortArrayComparer(Collider A, Collider B)
    {
        if (A == null && B != null) return 1;
        else if (A != null && B == null) return -1;
        else if (A == null && B == null) return 0;
        return Vector3.Distance(transform.position, A.transform.position).CompareTo(Vector3.Distance(transform.position, B.transform.position));
    }

    public float CountNavMeshPathDistance(Transform origin, Vector3 target, NavMeshAgent agent)
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(origin.position, target, agent.areaMask, path))
        {
            float distance = Vector3.Distance(origin.position, path.corners[0]);
            for (int j = 1; j < path.corners.Length; j++)
            {
                distance += Vector3.Distance(path.corners[j - 1], path.corners[j]);
            }
            return distance;
        }
        return 0;
    }

    //Logic 'Switch Character'
    public void SwitchCharacter()
    {
        //kategori untuk refrensikan yang diperlukan
        GameManager gm = GameManager.instance;        

        //kategori logic script
        gm.playerGameObject[gm.playableCharacterNum].GetComponent<PlayerAction>().enabled = false;
        gm.playerGameObject[gm.playableCharacterNum].GetComponent<PlayerCamera>().enabled = false;
        gm.playerGameObject[gm.playableCharacterNum].GetComponent<NavMeshAgent>().enabled = true;
        gm.playerGameObject[gm.playableCharacterNum].GetComponent<FriendsAI>().enabled = true;

        //kategori kamera
        gm.followCameras[gm.playableCharacterNum].m_Lens.FieldOfView = 60;
        gm.followCameras[gm.playableCharacterNum].Priority = 1;        
        gm.scope = false;
        StartCoroutine(CameraDelay(gm));

        //kategori untuk friendsAI
        gm.playerGameObject[gm.playableCharacterNum].GetComponent<FriendsAI>().friendsID = 1;

        gm.playableCharacterNum++;

        if (gm.playableCharacterNum >= gm.playerGameObject.Length) // check if playerNumber more than playableCharacters.Length, then playerNumber back to 0
        {
            gm.playableCharacterNum = 0;
        }

        SetActiveCharacter(gm, gm.playableCharacterNum);

        StartCoroutine(Switching(gm));
    }

    //Logic 'Mengaktifkan karakter ketika di switch'
    private void SetActiveCharacter(GameManager gm, int playerNumber)
    {       
        //kategori logic script
        gm.playerGameObject[playerNumber].GetComponent<PlayerAction>().enabled = true;        
        gm.playerGameObject[playerNumber].GetComponent<PlayerCamera>().enabled = true;
        gm.playerGameObject[gm.playableCharacterNum].GetComponent<FriendsAI>().enabled = false;
        gm.playerGameObject[gm.playableCharacterNum].GetComponent<NavMeshAgent>().enabled = false;


        //kategori kamera
        gm.followCameras[playerNumber].Priority = 2;

        //kategori untuk friendsAI
        if(gm.playableCharacterNum == gm.playerGameObject.Length-1)
        {
            gm.playerGameObject[0].GetComponent<FriendsAI>().friendsID = 2;
            gm.playerGameObject[gm.playableCharacterNum].GetComponent<NavMeshAgent>().enabled = false;
        }
        else
        {
            gm.playerGameObject[gm.playableCharacterNum + 1].GetComponent<FriendsAI>().friendsID = 2;
            gm.playerGameObject[gm.playableCharacterNum].GetComponent<NavMeshAgent>().enabled = false;
        }


    }

    //delay untuk switch karakter
    public IEnumerator Switching(GameManager gm)
    {
        gm.canSwitch = false;

        yield return new WaitForSeconds(1);

        gm.canSwitch = true;
    }

    //delay untuk perpindahan kamera
    public IEnumerator CameraDelay(GameManager gm)
    {
        yield return new WaitForSeconds(0.1f);

        gm.playerGameObject[gm.playableCharacterNum].gameObject.transform.GetChild(0).GetChild(0).GetChild(0).eulerAngles = Vector3.zero;
    }

    public IEnumerator ReloadTime(Action<bool> isReloading, float reloadTime)
    {
        yield return new WaitForSeconds(reloadTime);

        isReloading(false);
    }

    public IEnumerator FireRate(Action<bool> fireRateOn, float fireRateTime)
    {
        fireRateOn(true);

        yield return new WaitForSeconds(fireRateTime);

        fireRateOn(false);
    }


    public void HideMouseCursor()
    {
        // hide mouse cursor when game start
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowMouseCursor()
    {
        // hide mouse cursor when game start
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
