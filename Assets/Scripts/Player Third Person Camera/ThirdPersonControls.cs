using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class ThirdPersonControls : MonoBehaviour
{
    [SerializeField] private float PlayerSpeed = 5f;
    [SerializeField] private float GravityValue = -9.81f;
    [SerializeField] private bool IsGamepad;

    private float turnSmoothVelocity;

    private CharacterController controller;
    private PlayerActions playerActions;
    private PlayerInput playerInput;

    private Vector2 movement;
    private Vector2 aim;
    private Vector3 playerVelocity;

    private Transform cameraMain;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerActions = new PlayerActions();
        playerInput = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        playerActions.Enable();
    }

    void OnDisable()
    {
        playerActions.Disable();
    }

    private void Start()
    {
        cameraMain = Camera.main.transform;
    }

    void Update()
    {
        HandleInput();
        HandleMovement();
    }

    void HandleInput()
    {
        movement = playerActions.Controls.Movement.ReadValue<Vector2>();
        aim = playerActions.Controls.Rotate.ReadValue<Vector2>();
    }

    void HandleMovement()
    {
        Vector3 move = (cameraMain.forward * movement.y + cameraMain.right * movement.x);
        move.y = 0f;
        controller.Move(move * Time.deltaTime * PlayerSpeed);

        if (move != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0.1f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        playerVelocity.y += GravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void LookAt(Vector3 point)
    {
        Vector3 height_corrected = new Vector3(point.x, transform.position.y, point.z);
        transform.LookAt(height_corrected);
    }

    public void OnDeviceChange(PlayerInput pi)
    {
        IsGamepad = pi.currentControlScheme.Equals("Gamepad") ? true : false;
    }
}
