
using UnityEngine;

public class PlayerGunCollide : MonoBehaviour
{
    [ReadOnly(false), SerializeField] private Collider _wallCollide;
    [SerializeField] private PlayableCharacterIdentity _charaIdentity;
    // [SerializeField] private LayerMask _gunInWallLayer;
    // [SerializeField] private LayerMask _normalGunLayer;
    private const string _gunInWallLayerName = "WeaponInWall";
    private const string _normalGunLayerName = "Weapons";
    [SerializeField] private GameObject[] _gunSkin;
    // public Transform example;

    private void Awake() 
    {
        _charaIdentity = GetComponentInParent<PlayableCharacterIdentity>();
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.CompareTag("Interactable") || other.gameObject.CompareTag("Player")) return;
        _wallCollide = other;
        if(_charaIdentity.IsPlayerInput) ChangeGunSkinLayer(_gunInWallLayerName);
    }
    private void OnTriggerStay(Collider other) 
    {
        if(other.gameObject.CompareTag("Interactable") || other.gameObject.CompareTag("Player")) return;
        _wallCollide = other;
        if(_charaIdentity.IsPlayerInput) ChangeGunSkinLayer(_gunInWallLayerName);
    }
    private void OnTriggerExit(Collider other) 
    {
        _wallCollide = null;
        ChangeGunSkinLayer(_normalGunLayerName);
    }

    private void ChangeGunSkinLayer(string layerName)
    {   
        int idx = LayerMask.NameToLayer(layerName);
        foreach(GameObject obj in _gunSkin)
        {
            obj.layer = idx;
        }
    }
    public Vector3 GetClosestPosFromInsideWall(Vector3 originShootPos)
    {
        // Debug.Log(originShootPos + " LAMANYA POS ADALA");
        if(_wallCollide != null)
        {
            Vector3 direction = (originShootPos - _wallCollide.bounds.center).normalized;
            originShootPos = _wallCollide.ClosestPoint(originShootPos + direction * 0.1f);
        }
        // if(example != null)example.transform.position = originShootPos;
        // Debug.Log(originShootPos + " BARUNYA POS ADALA");
        return originShootPos;
    }
    public bool IsInsideWall()
    {
        return _wallCollide != null ? true : false;
    }
    public void ResetCollider()
    {
        _wallCollide = null;
    }
}
