using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [SerializeField] private float WalkSpeed = 5f;
    [SerializeField] private float RunSpeed = 10f;
    [SerializeField] private float GravityValue = -9.81f;
    [SerializeField] private float JumpHeight = 1.2f;
    [SerializeField] private float SpeedChangeRate = 10f;
    [SerializeField] private GameObject MainCamera;
    [SerializeField] private GameObject CinemachineCameraTarget;
    [SerializeField] private float TopClamp = 70.0f;
    [SerializeField] private float BottomClamp = -30.0f;
    [SerializeField] private float CameraDeltaTimeMultiplier = .65f;
    [SerializeField] private LayerMask GroundLayer;

    [SerializeField] private bool IsGamepad = false;

    private bool IsRunPress = false;
    private bool IsJumpPress = false;
    private bool IsGrounded = true;

    private CharacterController controller;
    private PlayerInput playerInput;
    private ControlsInput input;
    private Animator animator;

    private Vector2 movement;
    private Vector2 looking;

    private Vector3 velocity;
    
    private float speed = .0f;
    private float target_rotation = .0f;
    private float rotation_velocity;
    private float rotationSmoothTime = 0.12f;
    private float acceleration = 0.0f;

    // cinemachine
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        input = new ControlsInput();
    }

    private void Start()
    {
        input.Controls.Sprint.started  += ctx => IsRunPress = true;
        input.Controls.Sprint.canceled += ctx => IsRunPress = false;

        input.Controls.Jump.started  += ctx => IsJumpPress = true;
        input.Controls.Jump.canceled += ctx => IsJumpPress = false;

        cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void HandleInput()
    {
        movement = input.Controls.Move.ReadValue<Vector2>();
        looking  = input.Controls.Look.ReadValue<Vector2>().normalized;
        if (!IsGamepad) looking = new Vector2(looking.x, looking.y * -1);
    }

    private void HandleMovement()
    {      
        if (movement == Vector2.zero && speed > 0f)
        {
            speed = Mathf.Lerp(speed, 0f, Time.deltaTime * SpeedChangeRate);
            if (speed < 0.05) speed = 0f;
        }
        else if (movement != Vector2.zero)
        {
            acceleration = Mathf.Lerp(acceleration, IsRunPress ? 1f : 0.5f, Time.deltaTime * SpeedChangeRate);
            speed = Mathf.Lerp(speed, acceleration, Time.deltaTime * SpeedChangeRate);
        }

        Vector3 input_direction = new Vector3(movement.x, .0f, movement.y);
        if (movement != Vector2.zero)
        {
            target_rotation = Mathf.Atan2(input_direction.x, input_direction.z) * Mathf.Rad2Deg + MainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, target_rotation, ref rotation_velocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(.0f, rotation, .0f);
        }

        Vector3 targetDiretion = Quaternion.Euler(.0f, target_rotation, .0f) * Vector3.forward;

        controller.Move(targetDiretion.normalized * (speed * (IsRunPress ? RunSpeed : WalkSpeed) * Time.deltaTime) + velocity * Time.deltaTime);

        velocity.y += GravityValue * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        animator.SetFloat("Speed", speed);
    }

    private void CameraRotation()
    {
        if (looking.sqrMagnitude >= 0.01)
        {
            cinemachineTargetYaw += looking.x * CameraDeltaTimeMultiplier;
            cinemachineTargetPitch += looking.y * CameraDeltaTimeMultiplier;
        }

        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch, cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    public void OnInputChanged(PlayerInput playerInput)
    {
        IsGamepad = playerInput.currentControlScheme.Equals("Gamepad") ? true : false;
    }
}
