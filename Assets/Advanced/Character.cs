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
    [SerializeField] private float RotationSmoothing = 0.4f;
    [SerializeField] private GameObject CinemachineCamera;
    [SerializeField] private GameObject MainCamera;
    [SerializeField] private GameObject SequenceCanvasObject;
    [SerializeField] private GameObject PauseMenuObject;
    [SerializeField] private GameObject ShopMenuObject;
    [SerializeField] private GameObject PlayerScreenObject;
    [SerializeField] private AudioClip OnKickSound;

    [SerializeField] private AudioClip onCarMusic;
    [SerializeField] private AudioClip OnSequenceFinished;

    [SerializeField] private AudioClip[] foneTracks;

    [Range(0, 1)]
    [SerializeField] private float OnKickSoundVolume;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Animator animator;
    public ControlsInput input;
    private CameraLook cameraControl;

    private float terminalVelocity = 53.0f;

    protected Vector2 movement;
    protected Vector2 looking;
    protected Vector3 velocity;
    private float rotationMoveDelta = 1.0f;
    protected bool IsRunPress = false;
    protected bool IsJumpPress = false;
    protected float blendSpeed = 0.0f;
    protected float acceleration = 0.0f;
    protected float targetRotation = 0.0f;
    protected float rotationVelocity;
    private bool IsSequenceNow = false;

    private GameObject currentCar = null;
    private float KickResetTimer = 0f;

    private AudioSource sounds;

    private string paused_music_name;
    private float paused_music_position;

    private int xp = 0;
    private int money = 0;
    private int mylvl = 1;

    private bool isSequenceMusic = false;
    private float sequence_music_delta = 0.0f;

    private void Awake()
    {
        controller  = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        animator    = GetComponent<Animator>();
        sounds      = GetComponent<AudioSource>();
        input       = new ControlsInput();
    }

    private void Start()
    {
        input.Controls.Sprint.performed += ctx => IsRunPress = true;
        input.Controls.Sprint.canceled  += ctx => IsRunPress = false;

        input.Controls.Jump.performed += ctx => IsJumpPress = true;
        input.Controls.Jump.canceled  += ctx => IsJumpPress = false;

        input.Sequence.A.performed += OnA;
        input.Sequence.B.performed += OnB;
        input.Sequence.Y.performed += OnY;
        input.Sequence.X.performed += OnX;

        input.Controls.Pause.performed += TryOpenPauseMenu;
        input.Controls.Interact.performed += TryInteractWithCar;
        input.Controls.Shop.performed += TryOpenShopMenu;

        cameraControl = CinemachineCamera.GetComponent<CameraLook>();

        input.Sequence.Disable();
        input.Pause.Disable();
        input.ShopAction.Disable();

        sounds.clip = foneTracks[0];
        sounds.Play();
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
        HandleAdditional();
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

            rotationMoveDelta = 1.0f - Mathf.Abs(rotationVelocity / 200f);

            transform.rotation = Quaternion.Euler(.0f, rotation, .0f);
        }
    }

    protected virtual void HandleMovement()
    {
        if (movement != Vector2.zero)
        {
            acceleration = Mathf.Lerp(acceleration, IsRunPress ? 1f : 0.5f, Time.deltaTime * MovementSmooting);
            blendSpeed = Mathf.Lerp(blendSpeed, acceleration, Time.deltaTime * MovementSmooting);

            Vector3 targetDiretion = Quaternion.Euler(.0f, targetRotation, .0f) * Vector3.forward;
            targetDiretion.y = 0f;
            controller.Move(targetDiretion.normalized * (blendSpeed * (IsRunPress ? RunSpeed : WalkSpeed) * rotationMoveDelta * Time.deltaTime) + velocity * Time.deltaTime);
        }
        else if (movement == Vector2.zero && blendSpeed > 0.0f)
        {
            blendSpeed = Mathf.Lerp(blendSpeed, 0.0f, Time.deltaTime);
            if (blendSpeed < 0.01f) blendSpeed = 0.0f;
        }

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

    protected void HandleAdditional()
    {
        if (IsSequenceNow)
        {
            KickResetTimer += Time.deltaTime;
            if (KickResetTimer > 1.2)
            {
                animator.SetBool("Kick", false);
                KickResetTimer = 0f;
            }
        }

        if (isSequenceMusic)
        {
            sequence_music_delta += Time.deltaTime;
            if (sequence_music_delta > 6.5f)
            {
                sequence_music_delta = 0f;
                isSequenceMusic = false;
                RestoreFoneMusic();
            }
        }

        if (!sounds.isPlaying)
        {
            sounds.clip = foneTracks[0];
            sounds.Stop();
            sounds.Play();
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

    public void SetCurrentInteractable(GameObject carObject)
    {
        currentCar = carObject;
    }

    public void RemoveCurrentInteractable()
    {
        currentCar = null;
        cameraControl.ChangeLookAt(transform.Find("CameraPoint"));
    }

    private void OnA(InputAction.CallbackContext ctx)
    {
        if (!currentCar) return;
        
        SequenceCanvas sequenceCanvas = SequenceCanvasObject.GetComponent<SequenceCanvas>();
        if (sequenceCanvas)
        {
            CarInteraction car_script = currentCar.GetComponent<CarInteraction>();
            if (sequenceCanvas.ActionPerform("A"))
            {
                AudioSource.PlayClipAtPoint(OnKickSound, transform.TransformPoint(controller.center), OnKickSoundVolume);
                animator.SetBool("Kick", true);
                if (car_script.TryHit())
                {
                    FinishSequence();
                }
                else
                {
                    sequenceCanvas.NextSequenceIteration();
                }
            }
            else
            {
                car_script.Restore();
            }
        }
    }

    private void OnB(InputAction.CallbackContext ctx)
    {
        if (!currentCar) return;
        
        SequenceCanvas sequenceCanvas = SequenceCanvasObject.GetComponent<SequenceCanvas>();
        if (sequenceCanvas)
        {
            CarInteraction car_script = currentCar.GetComponent<CarInteraction>();
            if (sequenceCanvas.ActionPerform("B"))
            {
                AudioSource.PlayClipAtPoint(OnKickSound, transform.TransformPoint(controller.center), OnKickSoundVolume);
                animator.SetBool("Kick", true);
                if (car_script.TryHit())
                {
                    FinishSequence();
                }
                else
                {
                    sequenceCanvas.NextSequenceIteration();
                }
            }
            else
            {
                car_script.Restore();
            }
        }
    }

    private void OnX(InputAction.CallbackContext ctx)
    {
        if (!currentCar) return;
        
        SequenceCanvas sequenceCanvas = SequenceCanvasObject.GetComponent<SequenceCanvas>();
        if (sequenceCanvas)
        {
            CarInteraction car_script = currentCar.GetComponent<CarInteraction>();
            if (sequenceCanvas.ActionPerform("X"))
            {
                AudioSource.PlayClipAtPoint(OnKickSound, transform.TransformPoint(controller.center), OnKickSoundVolume);
                animator.SetBool("Kick", true);
                if (car_script.TryHit())
                {
                    FinishSequence();
                }
                else
                {
                    sequenceCanvas.NextSequenceIteration();
                }
            }
            else
            {
                car_script.Restore();
            }
        }
    }

    private void OnY(InputAction.CallbackContext ctx)
    {
        if (!currentCar) return;
        
        SequenceCanvas sequenceCanvas = SequenceCanvasObject.GetComponent<SequenceCanvas>();
        if (sequenceCanvas)
        {
            CarInteraction car_script = currentCar.GetComponent<CarInteraction>();
            if (sequenceCanvas.ActionPerform("Y"))
            {
                AudioSource.PlayClipAtPoint(OnKickSound, transform.TransformPoint(controller.center), OnKickSoundVolume);
                animator.SetBool("Kick", true);
                if (car_script.TryHit())
                {
                    FinishSequence();
                }
                else
                {
                    sequenceCanvas.NextSequenceIteration();
                }
            }
            else
            {
                car_script.Restore();
            }
        }
    }

    private void TryInteractWithCar(InputAction.CallbackContext ctx)
    {
        if (!currentCar) return;
        
        if (currentCar.GetComponent<CarInteraction>().GetLVLLock() > mylvl) return;

        Vector2 onKickReversePosition = new Vector2(transform.position.x - currentCar.transform.position.x, transform.position.y - currentCar.transform.position.y);
        float need_ang = Mathf.Atan2(onKickReversePosition.x, onKickReversePosition.y);
        if (need_ang < 0f)
        {
            need_ang += 2 * Mathf.PI;
        }
        if (need_ang > Mathf.PI * 2)
        {
            need_ang -= Mathf.PI * 2;
        }
        transform.rotation = Quaternion.Euler(.0f, need_ang * 180 / Mathf.PI, .0f);
        
        SequenceCanvasObject.SetActive(true);
        SequenceCanvas sequenceCanvas = SequenceCanvasObject.GetComponent<SequenceCanvas>();
        sequenceCanvas.Init(this);
        sequenceCanvas.NextSequenceIteration();

        input.Controls.Disable();
        input.Sequence.Enable();
        IsSequenceNow = true;

        paused_music_name = sounds.clip.name;
        paused_music_position = sounds.time;
        sounds.Stop();
        sounds.clip = onCarMusic;        
        sounds.Play();
    }

    private void TryOpenPauseMenu(InputAction.CallbackContext ctx)
    {
        if (IsSequenceNow) return;

        PauseMenuObject.SetActive(true);
        PauseMenu menu = PauseMenuObject.GetComponent<PauseMenu>();
        if (menu)
        {
            menu.SetActions(input.Pause);
            menu.Pause();
            input.Pause.Enable();
            input.Controls.Disable();
            input.Sequence.Disable();
        }
    }

    private void TryOpenShopMenu(InputAction.CallbackContext ctx)
    {
        if (IsSequenceNow) return;

        ShopMenuObject.SetActive(true);
        ShopUiControl shop = ShopMenuObject.GetComponent<ShopUiControl>();
        if (shop)
        {
            shop.SetActions(input.ShopAction);
            shop.OpenShop();
            input.Controls.Disable();
            input.Pause.Disable();
            input.Sequence.Disable();
            input.ShopAction.Enable();
        }
    }

    public void RestoreControls()
    {
        input.Controls.Enable();
        input.Pause.Disable();
        input.Sequence.Disable();
        input.ShopAction.Disable();
    }

    public void FailSequence()
    {
        cameraControl.ChangeLookAt(transform.Find("CameraPoint"));
        input.Controls.Enable();
        input.Sequence.Disable();
        IsSequenceNow = false;
        animator.SetBool("Kick", false);
        RestoreFoneMusic();
    }

    public void FinishSequence()
    {
        input.Controls.Enable();
        input.Sequence.Disable();
        IsSequenceNow = false;
        animator.SetBool("Kick", false);
        sounds.Stop();
        sounds.clip = OnSequenceFinished;
        sounds.Play();
        isSequenceMusic = true;
        //RestoreFoneMusic();
        CarInteraction car_script = currentCar.GetComponent<CarInteraction>();
        xp += car_script.GetXPPrize();
        money += car_script.GetMoneyPrize();
        UpdatePlayerUI();

        RemoveCurrentInteractable();
    }

    private void RestoreFoneMusic()
    {
        if (paused_music_name.Length == 0) return;

        sounds.Stop();
        for (int i = 0; i < foneTracks.Length; i++)
        {
            if (foneTracks[i].name == paused_music_name)
            {
                sounds.clip = foneTracks[i];
                sounds.time = paused_music_position;
                sounds.Play();
                break;
            }
        }
    }

    public bool CanSpendMoney(int count)
    {
        return money >= count;
    }

    public void UnlockEquipment(string eqv_name, int money_spend, int xp_earn)
    {
        string item_name = "";
        if (eqv_name.Equals("Ring 1"))
        {
            item_name = "Ring3";
        }
        if (eqv_name.Equals("Ring 2"))
        {
            item_name = "Ring4";
        }
        if (eqv_name.Equals("Ring 3"))
        {
            item_name = "Ring5";
        }
        if (eqv_name.Equals("Neclace 3"))
        {
            item_name = "Necklace3";
        }
        if (eqv_name.Equals("Neclace 2"))
        {
            item_name = "Necklace2";
        }
        if (eqv_name.Equals("Neclace 1"))
        {
            item_name = "Necklace1";
        }
        if (eqv_name.Equals("Crown 1"))
        {
            item_name = "Crown2";
        }

        if (money_spend > money) return;

        money -= money_spend;
        xp += xp_earn;

        UpdatePlayerUI();

        GameObject obj = RecursiveFindChild(transform.gameObject, item_name);
       
        obj.SetActive(true);
    }

    public void UpdatePlayerUI()
    {
        PlayerUI playerUI = PlayerScreenObject.GetComponent<PlayerUI>();
        int lvl = 1;
        float delta = xp / 600.0f;
        if (xp >= 600)
        {
            lvl = 2;
            delta = xp / 2400.0f;
        }
        if (xp >= 2400)
        {
            lvl = 3;
            delta = xp / 4400.0f;
        }
        if (xp >= 4400)
        {
            lvl = 4;
            delta = 1.0f;
        }
        mylvl = lvl;
        playerUI.SetMoneyCount(money);
        playerUI.SetXPValue(delta);
        playerUI.SetLVL(lvl);
    }

    private GameObject RecursiveFindChild(GameObject obj, string tag)
    {
        for (int index = 0; index < obj.transform.childCount; index++)
        {
            if (obj.transform.GetChild(index).name == tag)
            {
                return obj.transform.GetChild(index).gameObject;
            }

            GameObject child_child = RecursiveFindChild(obj.transform.GetChild(index).gameObject, tag);
            if (child_child)
            {
                return child_child;
            }
        }
        return null;
    }

    public void OnInputSourceChanged(PlayerInput playerInput)
    {
        IsGamepad = playerInput.currentControlScheme.Equals("Gamepad") ? true : false;
    }
}
