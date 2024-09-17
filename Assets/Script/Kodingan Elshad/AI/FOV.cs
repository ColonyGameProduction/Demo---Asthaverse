using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV : MonoBehaviour
{
    //Function untuk handle FOV
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask playerMask;
    public LayerMask groundMask;
    public List<Transform> visibleTargets = new List<Transform>();

    public float meshResolution;

    private void Start()
    {
        StartCoroutine("FindTargetWithDelay", .2f);
    }

    private void FixedUpdate()
    {
        DrawFieldOfView();
    }

    //Delay untuk mencari musuh yang ada
    public IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargetsForEnemy();
        }
    }

    //Untuk membuat FOV nya
    public void DrawFieldOfView()
    {
        //Menghitung ada berapa banyak ray yang harus dibuat
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        //Menghitung jarak antara ray
        float stepAngleSize = viewAngle / stepCount;

        //Membuat ray nya
        for(int i = 0; i <= stepCount; i++)
        {
            //Menghitung sudut untuk membuat ray-nya
            //Berawal dari sudut paling kiri (transform.eulerAngles.y - viewAngle/2)
            //Lalu ditambah dengan jarak antara ray dan di kalikan dengan i
            //Misal jarak nya 5 derajat, maka ray pertama dan kedua memiliki jarak 5 derajat
            //i = 0 -> 0 derajat
            //i = 1 -> 5 derajat
            //i = 2 -> 10 derajat
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            Debug.DrawLine(transform.position, transform.position + DirectionFromAngle(angle, true) * viewRadius, Color.red);
        }
    }    


    //Untuk mendeteksi semua collider yang ada di dalam FOV
    public void FindVisibleTargetsForEnemy()
    {
        visibleTargets.Clear();
        //Menyimpan semua collider yang terdeteksi 
        //Titik awal berada di posisi character
        //Selanjutnya akan membuat lingkaran sebesar 'viewRadius'
        //Jika collider game object nya memiliki mask yang ditentukan, maka akan disimpan
        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, playerMask);

        //Untuk mendeteksi jarak dan arah musuh/player
        for (int i = 0; i < targetInViewRadius.Length; i++)
        {
            //menghitung arah dari collider yang dideteksi
            Transform target = targetInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            //jika arah nya berada di dalam FOV
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, groundMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    //Untuk menentukan arah dari sudut menjadi radian
    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        //Untuk mengubah arah FOV sesuai dengan arah character
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }


        //Untuk menentukan besarnya FOV (membuat 2 point, 1 dikiri, 1 dikanan)
        //
        //Class Mathf menggunakan satuan radian untuk menghitung sudut.
        //Sudut dalam unity berbeda dengan kehidupan nyata, dimana 0 derajat letak nya ada dikanan
        //Letak 0 derajat pada unity berada di forward (depan/atas)
        //
        //Maka dari itu "angleInDegrees" diubah menjadi radian dahulu
        //Supaya Mathf.Sin, dan Mathf.Cos bisa menghitung nya
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
