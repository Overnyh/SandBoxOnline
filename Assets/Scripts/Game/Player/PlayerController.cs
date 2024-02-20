using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float walkingSpeed = 5;
    [SerializeField] private float runningSpeed = 10;
    [SerializeField] private float sensitivity;
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private float physicForce;
    [SerializeField] private float physicDistance;

    private float _xRotation = 0f;
    private Vector3 _moveDirection = Vector3.zero;
    private CharacterController _characterController;
    private Camera _playerCamera;

    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool canModedCamera = true;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            _playerCamera = Camera.main;
            _playerCamera.transform.position = cameraPosition.position;
            _playerCamera.transform.SetParent(transform);
        }
        else
        {
            gameObject.GetComponent<PlayerController>().enabled = false;
        }
    }

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked? CursorLockMode.Confined: CursorLockMode.Locked;
            canModedCamera = CursorLockMode.Locked == Cursor.lockState;
        }
    }

    private void FixedUpdate()
    {
        CharacterMove();
        CameraMove();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void CharacterMove()
    {
        bool isRunning;

        isRunning = Input.GetKey(KeyCode.LeftShift);

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = _moveDirection.y;
        _moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && _characterController.isGrounded)
        {
            _moveDirection.y = 4f;
        }
        else
        {
            _moveDirection.y = movementDirectionY;
        }

        if (!_characterController.isGrounded)
        {
            _moveDirection.y -= 10f * Time.fixedDeltaTime;
        }
        
        _characterController.Move(_moveDirection * Time.deltaTime);
    }

    private void CameraMove()
    {
        if (canModedCamera)
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            _playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            //transform.Rotate(Vector3.up*mouseX);
            transform.rotation *= Quaternion.Euler(0, mouseX, 0);
        }
    }
}