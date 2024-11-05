using UnityEngine;

public class EnemySoundDetection : MonoBehaviour
{
    public float hearingRange = 5f; // jarak denger dari si enemy
    public float whistleHearingMultiplier = 2f; // ini multiplier jarak whistle, biar enemy tuh bisa dengan whistle dari jauh

    // ini buat handle deteksi sound
    public void HearSound(GameObject player, AudioSource playerAudio)
    {
        float distance = Vector3.Distance(transform.position, player.transform.position); // jarak antara enemy sama si player
        float volume = playerAudio.volume; // volume soundnya

        if (playerAudio.clip.name == "Whistle") // enemy ngedenger whistle kalo bukan whistle yaaaaa footsteps
        {
            if (distance <= hearingRange * volume * whistleHearingMultiplier)
            {
                Debug.Log("Enemy hears the whistle");
            }
        }
        else
        {
            if (distance <= hearingRange * volume)
            {
                Debug.Log("Enemy hears footsteps");
            }
        }
    }

    // buat gambaran jarak dari pendengarannya aja si, merah itu jarak dengan si enemy, biru itu multiplier buat whistle sound
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hearingRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRange * whistleHearingMultiplier);
    }
}
