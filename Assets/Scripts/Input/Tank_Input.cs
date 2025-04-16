using System;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tank_Input : MonoBehaviour
{

    #region VARIABLES
    [Header("Input System Actions")]
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private TankInputActions _tankActions;
    [Header("Ray interaction Layer")]
    [SerializeField] private LayerMask _layerMask;

    //Tank Movement Direction
    private Vector2 _bodyMovementInput = Vector2.zero;
    private Vector2 _turretMovementInput = Vector2.zero;

    private Vector3 rayHitPosition;
    private Vector3 turretLookDirection;
    #endregion

    #region PROPERTIES
    public Vector2 BodyMovementInput { get { return _bodyMovementInput; } }
    public Vector2 TurretMovementInput { get { return _turretMovementInput; } }
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _tankActions = new TankInputActions();
    }
    private void OnEnable()
    {
        _tankActions.TankControl.Enable();
    }

    private void OnDisable()
    {
        _tankActions.TankControl.Disable();
    }
    void Start()
    {

    }

    void FixedUpdate()
    {
        HandleInput();
    }
    #endregion

    #region CUSTOM METHODS

    private void HandleInput()
    {
        HandleTankBodyMovementInput();
        HandleTankTurretMovementInput();
    }

    private void HandleTankBodyMovementInput()
    {
        _bodyMovementInput = _tankActions.TankControl.BodyMovement.ReadValue<Vector2>();
        //Debug.Log(_bodyMovement);
    }
    private void HandleTankTurretMovementInput()
    {
        bool usingGamepad = _playerInput.currentControlScheme.Equals(_tankActions.GamepadScheme.name) ? true : false;

        if (usingGamepad) {
            
            _turretMovementInput = _tankActions.TankControl.TurretMovement_Stick.ReadValue<Vector2>();
            Debug.Log("Gamepad Input: " + _turretMovementInput);

        }
        else
        {
            Vector3 mouseScreenPos = _tankActions.TankControl.TurretMovement_Mouse.ReadValue<Vector2>();
            //Cast a Ray from the camera
            Ray cameraRay = Camera.main.ScreenPointToRay(mouseScreenPos);
            RaycastHit rayHit;
            if (Physics.Raycast(cameraRay, out rayHit, Mathf.Infinity, _layerMask)) {
                rayHitPosition = rayHit.point;
            }

            //Get the vector from transform to hit position in XZ plane and normalized
             turretLookDirection = (rayHitPosition - transform.position).normalized;
            _turretMovementInput = new Vector2(turretLookDirection.x, turretLookDirection.z);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(rayHitPosition, 0.5f);
        
    }

    #endregion

}
