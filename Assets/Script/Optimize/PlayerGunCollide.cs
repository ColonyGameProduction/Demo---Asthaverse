using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunCollide : MonoBehaviour
{
    private Collider _wallCollide;
    public Transform example;
    private void OnCollisionEnter(Collision other) 
    {
        _wallCollide = other.collider;
    }
    private void OnCollisionExit(Collision other) 
    {
        _wallCollide = null;
    }
    public Vector3 GetClosestPosFromInsideWall(Vector3 originShootPos)
    {
        Debug.Log(originShootPos + " LAMANYA POS ADALA");
        if(_wallCollide != null)
        {
            Vector3 direction = (originShootPos - _wallCollide.bounds.center).normalized;
            originShootPos = _wallCollide.ClosestPoint(originShootPos + direction * 0.1f);
        }
        if(example != null)example.transform.position = originShootPos;
        Debug.Log(originShootPos + " BARUNYA POS ADALA");
        return originShootPos;
    }
    public bool IsInsideWall()
    {
        return _wallCollide != null ? true : false;
    }
}
