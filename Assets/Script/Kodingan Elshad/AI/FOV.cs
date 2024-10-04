using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV : MonoBehaviour
{
    //Function untuk handle FOV
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public int edgeResolveIteration;

    public LayerMask playerMask;
    public LayerMask groundMask;
    public List<Transform> visibleTargets = new List<Transform>();
    
    public float meshResolution;
    public float edgeDistanceTreshold;

    public Transform FOVPoint;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        StartCoroutine("FindTargetWithDelay", .2f);
    }

    private void LateUpdate()
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

        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

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
            float angle = FOVPoint.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;

            //Memasukan info untuk mesh nya
            ViewCastInfo newViewCast = ViewCast(angle);

            //Optimising FOV supaya bisa bagus tanpa harus memiliki resolusi yang tinggi
            if(i>0)
            {
                bool edgeDistanceTresholdExeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceTreshold;
                //Membandingkan kedua ViewCast, apakah mereka mendeteksi adanya pinggiran sudut
                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceTresholdExeded))
                {
                    //jika iya, maka akan menjalankan fungsi FindEdge
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);

                    //Akan menambahkan viewPoint (titik untuk mesh) sesuai value yang direturn
                    if(edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    else if(edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }


            //Memberikan titik untuk mesh
            viewPoints.Add(newViewCast.point);
            //Titik yang sudah di tambah tadi, disimpan sementara dan menjadi titik yang lama
            //digunakan untuk mencari sudut pada suatu object
            oldViewCast = newViewCast;
        }

        //Untuk membuat mesh
        //Menentukan ada berapa banyak titik yang ada (ditambah 1 karena titik origin)
        int vertexCount = viewPoints.Count + 1;
        //Menentukan posisi titik yang ada
        Vector3[] vertices = new Vector3[vertexCount];
        //Menentukan segitiga untuk mesh
        int[] triangles = new int[(vertexCount - 2) * 3];

        //Memasukan titik awal (origin)
        vertices[0] = Vector3.zero;

        //Membuat mesh
        for(int i = 0; i < vertexCount - 1; i++)
        {
            //Memasukan value titik (i+1 karena index ke 0 sudah diisi oleh titik origin)
            vertices[i + 1] = FOVPoint.InverseTransformPoint(viewPoints[i]);

            //supaya tidak out of bounds
            if(i<vertexCount-2)
            {
                //Point segitiga pertama selalu di titik origin (0)
                triangles[i * 3] = 0;
                //Point segitiga kedua selalu di titik (i+1)
                triangles[i * 3 + 1] = i + 1;
                //Point segitiga terakir selalu ada di titik (i+2)
                triangles[i * 3 + 2] = i + 2;
            }
        }

        //Membuat mesh segitiganya
        //Di clear dulu, supaya ketika sudah di frame selanjutnya, mesh nya tida duplikat dan jadi hancur
        viewMesh.Clear();
        //Titik mesh dimasukan
        viewMesh.vertices = vertices;
        //Segitiga mesh dimasukan
        viewMesh.triangles = triangles;
        //Mengkalkulasi lagi mesh nya supaya sesuai dengan orientasi
        viewMesh.RecalculateNormals();

    }    

    //Memasukan info tentang raycast
    public ViewCastInfo ViewCast(float globalAngle)
    {
        //Arah raycast (global karena sudut local sudah ditentukan di dalam DrawFieldOfView())
        Vector3 direction = DirectionFromAngle(globalAngle, true);
        RaycastHit hit;

        //Mengecek apakah raycast mengenai sesuatu
        if(Physics.Raycast(FOVPoint.position, direction, out hit, viewRadius, groundMask))
        {
            //jika iya, maka ray akan diteruskan hanya sampai point yang terkena
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            //jika tidak, maka ray akan diteruskan hingga FOV maksimal (viewRadius)
            return new ViewCastInfo(false, FOVPoint.position + direction * viewRadius, viewRadius, globalAngle);
        }
    }

    //Untuk mencari sudut ada dimana
    public EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        //Memasukan data sudut titik pertama dan kedua
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;

        //Untuk memberikan titik baru untuk selanjutnya dibuat di mesh
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        //untuk mencari sudut baru
        for(int i = 0; i<edgeResolveIteration; i++)
        {
            //Merata rata kedua sudut, supaya bisa mendapatkan sudut tengah
            float angle = (minAngle + maxAngle) / 2;
            //Memasukan info sudut yang baru
            ViewCastInfo newViewcastInfo = ViewCast(angle);

            bool edgeDistanceTresholdExeded = Mathf.Abs(minViewCast.distance - maxViewCast.distance) > edgeDistanceTreshold;
            //Jika sudut baru = sudut min, artinya sudut itu adalah sudut baru
            if (newViewcastInfo.hit == minViewCast.hit && !edgeDistanceTresholdExeded)
            {
                minAngle = angle;
                minPoint = newViewcastInfo.point;
            }
            //Jika sudut baru = sudut max, artinya sudut itu adalah sudut baru
            else
            {
                maxAngle = angle;
                maxPoint = newViewcastInfo.point;
            }
        }

        //Mereturn min/max
        //jika min = 0, maka titik max yang dipakai
        //begitu juga sebaliknya
        return new EdgeInfo(minPoint, maxPoint);

    }


    //Untuk mendeteksi semua collider yang ada di dalam FOV
    public void FindVisibleTargetsForEnemy()
    {
        visibleTargets.Clear();
        //Menyimpan semua collider yang terdeteksi 
        //Titik awal berada di posisi character
        //Selanjutnya akan membuat lingkaran sebesar 'viewRadius'
        //Jika collider game object nya memiliki mask yang ditentukan, maka akan disimpan
        Collider[] targetInViewRadius = Physics.OverlapSphere(FOVPoint.position, viewRadius, playerMask);

        //Untuk mendeteksi jarak dan arah musuh/player
        for (int i = 0; i < targetInViewRadius.Length; i++)
        {
            //menghitung arah dari collider yang dideteksi
            Transform target = targetInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - FOVPoint.position).normalized;

            //jika arah nya berada di dalam FOV
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                
                float distanceToTarget = Vector3.Distance(FOVPoint.position, target.position);
                if (!Physics.Raycast(FOVPoint.position, dirToTarget, distanceToTarget, groundMask))
                {
                    if(visibleTargets.Count > 0)if(visibleTargets.Contains(target))continue;
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
            angleInDegrees += FOVPoint.eulerAngles.y;
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


    //Struct untuk menyimpan info raycast FOV
    public struct ViewCastInfo
    {
        //Apakah ray mengenai sesuatu
        public bool hit;
        //Point ujung ray nya
        public Vector3 point;
        //Jarak ujung ray nya
        public float distance;
        //Sudut ray nya
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
        {
            hit = _hit;
            point = _point;
            distance = _distance;
            angle = _angle;
        }

    }

    //Untuk menyimpan value sudut
    public struct EdgeInfo
    {
        //Point A untuk min point(point yang pendek)
        public Vector3 pointA;
        //Point B untuk max point(point yang terjauh)
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

}
