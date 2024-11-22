using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWallWidthCheck : MonoBehaviour
{
    public Transform aaaa;
    public bool byo;
    private void Update() {
        if(byo)
        {
            byo = false;
            float _wallWidth = aaaa.localScale.x;
            float _wallLength = aaaa.localScale.z;

            Debug.Log("Wall WIdht" + _wallWidth + " Wall Length" + _wallLength + " " + aaaa.transform.forward + " " + aaaa.transform.right);
            Debug.DrawRay(aaaa.position, aaaa.transform.forward * 100f, Color.red, 2f, false );
            Debug.DrawRay(aaaa.position, aaaa.transform.right * 100f, Color.black, 2f, false );
        }
    }
}
