using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class TwinStickMovement : MonoBehaviour
{
    [SerializeField] private float PlayerSpeed = 5f;
    [SerializeField] private float GravityValue = -9.81f;
    [SerializeField] private float ControllerDeadzone = 0.1f;
    [SerializeField] private float GamepadRotateSmoothing = 1000f;
    [SerializeField] private bool IsGamepad;

    private CharacterController controller;

    private Vector2 movement;
    private Vector2 aim;

    private Vector3 playerVelocity;

    private PlayerActions playerActions;
    private PlayerInput playerInput;

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

    void Update()
    {
        HandleInput();
        HandleMovement();
        HandleRotation();
    }

    void HandleInput()
    {
        movement = playerActions.Controls.Movement.ReadValue<Vector2>();
        aim = playerActions.Controls.Rotate.ReadValue<Vector2>();
    }

    void HandleMovement()
    {
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        controller.Move(move * Time.deltaTime * PlayerSpeed);

        playerVelocity.y += GravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void HandleRotation()
    {
        if (IsGamepad)
        {
            if (Mathf.Abs(aim.x) > ControllerDeadzone || Mathf.Abs(aim.y) > ControllerDeadzone)
            {
                Vector3 player_direction = Vector3.right * aim.x + Vector3.forward * aim.y;
                if (player_direction.sqrMagnitude > 0.0f)
                {
                    Quaternion newRotation = Quaternion.LookRotation(player_direction, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, GamepadRotateSmoothing * Time.deltaTime);
                }
            }
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(aim);
            Plane ground_plane = new Plane(Vector3.up, Vector3.zero);
            float ray_distance;
            if (ground_plane.Raycast(ray, out ray_distance))
            {
                Vector3 point = ray.GetPoint(ray_distance);
                LookAt(point);
            }
        }
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
