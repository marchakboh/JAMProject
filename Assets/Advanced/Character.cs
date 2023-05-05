using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class Character : MonoBehaviour
{
    [SerializeField] private float WalkSpeed = 5f;
    [SerializeField] private float RunSpeed = 5f;
    [SerializeField] private float JumpHeight = 4f;
    [SerializeField] private float GravityValue = -9.81f;
    [SerializeField] private bool IsGamepad = false;
    [SerializeField] private float MovementSmooting = 10f;
    [SerializeField] private float RotationSmoothing = 2f;
    [SerializeField] private GameObject CinemachineCamera;
    [SerializeField] private GameObject MainCamera;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Animator animator;
    private ControlsInput input;
    private CameraLook cameraControl;

    private float terminalVelocity = 53.0f;

    protected Vector2 movement;
    protected Vector2 looking;
    protected Vector3 velocity;
    protected bool IsRunPress = false;
    protected bool IsJumpPress = false;
    protected float blendSpeed = 0.0f;
    protected float acceleration = 0.0f;
    protected float targetRotation = 0.0f;
    protected float rotationVelocity;

    private void Awake()
    {
        controller  = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        animator    = GetComponent<Animator>();
        input       = new ControlsInput();
    }

    private void Start()
    {
        input.Controls.Sprint.performed += ctx => IsRunPress = true;
        input.Controls.Sprint.canceled  += ctx => IsRunPress = false;

        input.Controls.Jump.performed += ctx => IsJumpPress = true;
        input.Controls.Jump.canceled  += ctx => IsJumpPress = false;

        cameraControl = CinemachineCamera.GetComponent<CameraLook>();
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
        HandleRotation();
        HandleMovement();
        HandleJump();
        HandleGravity();
    }

    protected virtual void HandleInput()
    {
        movement = input.Controls.Move.ReadValue<Vector2>();
        looking  = input.Controls.Look.ReadValue<Vector2>().normalized;
        if (!IsGamepad) looking = new Vector2(looking.x, looking.y * -1);
        cameraControl.UpdateDelta(looking);
    }

    protected virtual void HandleRotation()
    {
        if (movement != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + MainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothing);

            transform.rotation = Quaternion.Euler(.0f, rotation, .0f);
        }
    }

    protected virtual void HandleMovement()
    {
        if (movement != Vector2.zero)
        {
            acceleration = Mathf.Lerp(acceleration, IsRunPress ? 1f : 0.5f, Time.deltaTime * MovementSmooting);
            blendSpeed = Mathf.Lerp(blendSpeed, acceleration, Time.deltaTime * MovementSmooting);
        }
        else if (movement == Vector2.zero && blendSpeed > 0.0f)
        {
            blendSpeed = Mathf.Lerp(blendSpeed, 0.0f, Time.deltaTime);
            if (blendSpeed < 0.01f) blendSpeed = 0.0f;
        }

        Vector3 targetDiretion = Quaternion.Euler(.0f, targetRotation, .0f) * Vector3.forward;
        targetDiretion.y = 0f;
        controller.Move(targetDiretion.normalized * (blendSpeed * (IsRunPress ? RunSpeed : WalkSpeed) * Time.deltaTime) + velocity * Time.deltaTime);

        animator.SetFloat("BlendSpeed", blendSpeed);
    }

    protected virtual void HandleGravity()
    {
        if (velocity.y < terminalVelocity)
        {
            velocity.y += GravityValue * Time.deltaTime;
            
            //controller.Move(velocity * Time.deltaTime);
        }
    }

    protected virtual void HandleJump()
    {
        if (controller.isGrounded)
        {
            animator.SetBool("JumpOnRun", false);
            animator.SetBool("JumpOnWalk", false);

            if (velocity.y < 0.0f)
            {
                velocity.y = -2f;
            }

            if (IsJumpPress)
            {
                if (IsRunPress)
                {
                    animator.SetBool("JumpOnRun", true);
                }
                else
                {
                    animator.SetBool("JumpOnWalk", true);
                }
                velocity.y = JumpHeight;
            }
        }
        else
        {
            animator.SetBool("Grounded", false);
        }
    }

    public void OnInputSourceChanged(PlayerInput playerInput)
    {
        IsGamepad = playerInput.currentControlScheme.Equals("Gamepad") ? true : false;
    }
}