using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableMovementStateMachine : MovementStateMachine, ICrouch, IPlayableMovementDataNeeded
{
    
    [Header ("Playable Character Variable")]
    [Space(2)]
    [Header("GetComponent Manager")]
    [SerializeField] private CharacterController _cc;
    [Space(1)]
    [Header("For Rotation by Player")]
    [SerializeField] private Transform _charaGameObject;
    [SerializeField] private float _rotateSpeed;
    [Space(1)]
    [Header("Speed")]
    [SerializeField] private float _crouchSpeed;
    [Space(1)]
    [Header("Hubungan dgn Movement dan Camera")]
    [SerializeField] private Transform _followTarget;
    [SerializeField]protected bool _isCrouch;
    
    public bool IsCrouching { get {return _isCrouch;}set{} }
    public bool IsMustLookForward { get; set; } // Kan kalo nembak gitu hrs liat ke depan selalu, jd is true
    [HideInInspector]
    //getter setter
    public Vector3 InputMovement { get; set;} // Getting Input Movement from playercontroller
    public CharacterController CC {get { return _cc;} }
    public float CrouchSpeed {get{return _crouchSpeed;}}

    protected override void Awake()
    {
        base.Awake();
        if(_cc == null)_cc = GetComponent<CharacterController>();
    }

    public override void Move()
    {
        if(isAI)base.Move();
        else MovePlayableChara(InputMovement);
    }
    /// <summary>
    /// Move yang digunakan untuk kontrol dgn input dari player
    /// </summary>
    public void MovePlayableChara(Vector3 direction)
    {
        Vector3 movement = new Vector3(direction.x, 0, direction.y).normalized;

        Vector3 flatForward = new Vector3(_followTarget.forward.x, 0, _followTarget.forward.z).normalized;
        Vector3 Facedir = flatForward * movement.z + _followTarget.right * movement.x; 

        CC.SimpleMove(direction * _currSpeed);

        CharaAnimator.SetFloat("Horizontal", direction.x);
        CharaAnimator.SetFloat("Vertical", direction.z);

        if(!IsMustLookForward)RotatePlayableChara(Facedir);
        else RotatePlayableChara(flatForward);
    }
    public void RotatePlayableChara(Vector3 direction)
    {
        if(direction != Vector3.zero)
        {
            _charaGameObject.forward = Vector3.Slerp(_charaGameObject.forward, direction.normalized, Time.deltaTime * _rotateSpeed);
        }
    }
    

}
