using System;
using UnityEngine;


[RequireComponent (typeof(Rigidbody))]
public class Tank_Controller : MonoBehaviour
{

    #region VARIABLES
    [Header("Tank Input")]
    [SerializeField] private Tank_Input _input;

    [Header("Tank Components")]
    [SerializeField] private GameObject _tankBody;
    [SerializeField] private GameObject _tankTurret;

    [Header("Tank Rigidbody")]
    [SerializeField] private Rigidbody _rigidbody;

    [Header("Tank Properties")]
    [SerializeField] private float _tankMovementSpeed = 5.0f;
    [SerializeField] private float _tankRotationSpeed = 50.0f;
    [Header("Turret Properties")]
    [SerializeField] private float _turretRotationalMovementSpeed = 5.0f;

    private Vector3 _moveDirection = Vector3.zero; 
    private Vector3 _turretLookDirection = Vector3.zero; 
    


    #endregion

    #region PROPERTIES
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        _input = GetComponent<Tank_Input>();
        _rigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        _turretLookDirection = _tankTurret.transform.forward;
    }

    void FixedUpdate()
    {
        if (_input && _rigidbody) {
            HandleTankMovement();
        }
    }


    #endregion

    #region CUSTOM METHODS
    private void HandleTankMovement()
    {
        HandleTankBodyMovement();
        HandleTankTurretMovement();
    }

    private void HandleTankBodyMovement()
    {
        _moveDirection = new Vector3(_input.BodyMovementInput.x, 0, _input.BodyMovementInput.y);

        //Apply movement using RigidBody MovePosition
        _rigidbody.MovePosition(transform.position + (transform.forward * _moveDirection.magnitude * _tankMovementSpeed * Time.fixedDeltaTime));

        //Apply Rotation using RigidBody MoveRotation
        if (_moveDirection != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);
            Quaternion smoothRotation = Quaternion.Slerp(_rigidbody.rotation, targetRotation, _tankRotationSpeed * Time.fixedDeltaTime);
            _rigidbody.MoveRotation(smoothRotation);
        }
    }

    private void HandleTankTurretMovement()
    {
        _turretLookDirection = new Vector3(_input.TurretMovementInput.x, 0, _input.TurretMovementInput.y);

        if (_turretLookDirection != Vector3.zero)
        {
            _tankTurret.transform.forward = Vector3.Lerp(_tankTurret.transform.forward,_turretLookDirection,_turretRotationalMovementSpeed * Time.fixedDeltaTime);
        }

    }

    #endregion

    #region DEBUG
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(_tankBody.transform.position, _moveDirection * 2.0f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(_tankTurret.transform.position, _turretLookDirection * 2.0f);
    }
    #endregion
}
