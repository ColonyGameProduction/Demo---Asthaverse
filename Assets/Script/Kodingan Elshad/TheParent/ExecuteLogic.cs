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

/* PERHATIAN!!!
 * Kalo mau akses logic di skrip ini
 * Public class nya extend ke class ini
 * Jangan ke MonoBehaviour
 */

public class ExecuteLogic : AILogic
{

    public Collider[] walls;

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
    public void Shoot(Vector3 origin, Vector3 direction, EntityStatSO entityStat, WeaponStatSO weaponStat, LayerMask entityMask)
    {
        WeaponLogicHandler weaponHandler = new WeaponLogicHandler();
        weaponHandler.ShootingPerformed(origin, direction, entityStat, weaponStat, entityMask);
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

        playerAction.isCommandActive = true;
        playerAction.command.SetActive(true);
    }

    //Logic 'UnCommand'
    public void UnCommand()
    {
        PlayerAction playerAction = GetComponent<PlayerAction>();

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
            if (hit.collider.GetComponent<PickableItems>())
            {
                Debug.Log("Ambil!");
            }
            else if (hit.collider.GetComponent<OpenableObject>())
            {
                Debug.Log("Buka!");
            }
        }
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

        /*for(int i = 0; i < walls.Length; i++)
        {
            if (Vector3.Distance(walls[i].transform.position, targetDir) < 5f)
            {
                wallsLength--;
                walls[i] = null;
            }
        }

        if(wallsLength <= 0)
        {
            return;
        }*/

        Debug.Log(wallsLength);

        System.Array.Sort(walls, ColliderSortArrayComparer);

        for (int i = 0; i <= walls.Length - 1; i++)
        {
            Debug.Log("Masuk Loop");
            Debug.Log(walls[i]);

            if (NavMesh.SamplePosition(walls[i].transform.position, out NavMeshHit hit, 5f, agent.areaMask))
            {
                Debug.Log(hit.normal + "Sample Position 1" + hit.position);
                if (!NavMesh.FindClosestEdge(hit.position, out hit, agent.areaMask))
                {
                    Debug.Log("Unable to find edge close");
                    continue;
                }

                Vector3 directionToTarget = HitDirection(target, hit.position);
                if (Vector3.Dot(hit.normal, directionToTarget) < 0) // Jika wall ada di antara agent dan target
                {
                    if (CountNavMeshPathDistance(agent.transform, hit.position, agent) > 100f)
                    {
                        Debug.Log("Masuk Continue");
                        continue;
                    }

                    //hit.position = NewCheckPositionBasedOnWall(hit.position, walls[i]);

                    Debug.Log(hit.normal + "Sample normal 1" + hit.position);
                    Debug.Log(hit.position);
                    agent.SetDestination(hit.position);
                    break;
                }
                else
                {
                    Vector3 coverPosition = walls[i].transform.position - directionToTarget * 2;
                    if (NavMesh.SamplePosition(coverPosition, out NavMeshHit hit2, 2f, agent.areaMask))
                    {

                        //if (Vector3.Dot(hit2.normal, directionToTarget) < 0)
                        //{
                        //    agent.SetDestination(hit2.position);
                        //    break;
                        //}

                        Debug.Log(hit2.normal + "Sample Position 2" + hit2.position);

                        if (!NavMesh.FindClosestEdge(hit2.position, out hit2, agent.areaMask))
                        {
                            Debug.Log("Unable to find edge close");
                            continue;
                        }

                        directionToTarget = HitDirection(target, hit.position);
                        if (Vector3.Dot(hit2.normal, directionToTarget) < 0) // Jika wall ada di antara agent dan target
                        {
                            if (CountNavMeshPathDistance(agent.transform, hit.position, agent) > 100f)
                            {
                                Debug.Log("Masuk Continue");
                                continue;
                            }

                            //hit2.position = NewCheckPositionBasedOnWall(hit2.position, walls[i]);
                            Debug.Log(hit2.normal + "Sample normal 2" + hit2.position);

                            Debug.Log(hit2.position);
                            agent.SetDestination(hit2.position);
                            break;
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

    public Vector3 NewCheckPositionBasedOnWall(Vector3 targetPos, Collider wallColl)
    {
        bool isLeft;

        Vector3 wallCenter = wallColl.bounds.center;
        float wallSize = 0f;
        Vector3 newPos = Vector3.zero;

        float wallSizeX = wallColl.transform.localScale.x * 0.5f;
        float wallSizeZ = wallColl.transform.localScale.z * 0.5f;
        if (targetPos.z > wallCenter.z + wallSizeZ)
        {
            wallSize = wallSizeX + -.5f;
            if (targetPos.x >= wallCenter.x)
            {
                isLeft = false;
            }
            else
            {
                isLeft = true;

            }
            newPos = new Vector3(wallCenter.x + (isLeft ? -wallSize : wallSize), targetPos.y, targetPos.z);
        }
        else if (targetPos.z < wallCenter.z - wallSizeZ)
        {
            wallSize = wallSizeX + -.5f;
            if (targetPos.x >= wallCenter.x)
            {
                isLeft = true;
            }
            else
            {
                isLeft = false;
            }
            newPos = new Vector3(wallCenter.x + (isLeft ? wallSize : -wallSize), targetPos.y, targetPos.z);
        }
        else if ((targetPos.z <= wallCenter.z + wallSizeZ) && (targetPos.z >= wallCenter.z - wallSizeZ))
        {
            wallSize = wallSizeZ + -.5f;
            if (targetPos.x > wallCenter.x + wallSizeX)
            {
                if (targetPos.z >= wallCenter.z)
                {
                    isLeft = true;
                }
                else
                {
                    isLeft = false;
                }
                newPos = new Vector3(targetPos.x, targetPos.y, wallCenter.z + (isLeft ? wallSize : -wallSize));
            }
            else
            {
                if (targetPos.z >= wallCenter.z)
                {
                    isLeft = false;
                }
                else
                {
                    isLeft = true;

                }
                newPos = new Vector3(targetPos.x, targetPos.y, wallCenter.z + (isLeft ? -wallSize : wallSize));
            }
        }

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
