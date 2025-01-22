using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilentKill : MonoBehaviour
{
    //bool untuk mengecek apakah player bisa melakukan 'silentkill'
    public bool canKill;

    //fungsi untuk meng-eksekusi 'silentkill'
    public void KilledInSilence(GameObject gameObject)
    {        
        if(canKill)
        {
            // Debug.Log("Killed");
            Destroy(gameObject);
            canKill = false;
        }        
    }

    //fungsi untuk mendeteksi apakah disekitar player bisa melakukan silent kill
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            KilledInSilence(other.gameObject);
        }
    }
}
