using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    private CharacterControllerInputSystem input;
    private Rigidbody rb;
    private Animator anim;

    // Movement
    private Vector3 direction;
    [SerializeField]
    private float mvmspeedControl;
    private float mvmspeed;
    [SerializeField]
    private float speedMultiplier = 2;
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    public Transform cam;

    // Jump
    [SerializeField]
    private bool isGrounded;
    private float jumpForce = 5f;
    public Transform groundCheck;
    private Vector3 boxCastSize = new Vector3(0.3f, -0.05f, 0.3f);
    private float groundDistance = 0.1f;

    // Zoom
    private float mouseScrollY;
    public CinemachineFreeLook cineFreeLook;

    GameManager gameManager;

    private void Awake()
    {
        mvmspeed = mvmspeedControl;

        input = new CharacterControllerInputSystem();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        gameManager = GameObject.FindObjectOfType<GameManager>().GetComponent<GameManager>();
    }

    #region Enable/Disable Controls

    private void OnEnable()
    {
        input.Enable();
        input.NMVM.Movement.performed += OnMovementPerformed;
        input.NMVM.Movement.canceled += OnMovementCanceled;
        input.NMVM.Jump.performed += OnJumpPerformed;
        input.NMVM.Sprint.performed += OnSprintPerformed;
        input.NMVM.Sprint.canceled += OnSprintCanceled;
    }

    private void OnDisable()
    {
        input.Disable();
        input.NMVM.Movement.performed -= OnMovementPerformed;
        input.NMVM.Movement.canceled -= OnMovementCanceled;
        input.NMVM.Jump.performed -= OnJumpPerformed;
        input.NMVM.Sprint.performed -= OnSprintPerformed;
        input.NMVM.Sprint.canceled -= OnSprintCanceled;
    }

    #endregion

    #region Player Movement

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        Vector2 inputvalue = value.ReadValue<Vector2>();
        direction = new Vector3(inputvalue.x, 0, inputvalue.y).normalized;
    }

    private void OnMovementCanceled(InputAction.CallbackContext value)
    {
        direction = Vector3.zero;
    }

    private void OnSprintPerformed(InputAction.CallbackContext value)
    {
        mvmspeed *= speedMultiplier;
        anim.SetBool("run", true);
    }

    private void OnSprintCanceled(InputAction.CallbackContext value)
    {
        mvmspeed = mvmspeedControl;
        anim.SetBool("run", false);
    }

    private void MovePlayer()
    {
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized;
            rb.velocity = new Vector3(moveDir.x * mvmspeed, rb.velocity.y, moveDir.z * mvmspeed);
            anim.SetBool("move", true);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            anim.SetBool("move", false);
        }
    }


    #endregion

    #region Player Jump

    private void CheckIsGround()
    {
        isGrounded = Physics.BoxCast(groundCheck.position, boxCastSize / 2, Vector3.down, Quaternion.identity, groundDistance);
    }

    private void OnJumpPerformed(InputAction.CallbackContext value)
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
            anim.SetBool("jump", true);
        }
    }

    private void JumpDecend()
    {
        CheckIsGround();

        if (isGrounded && rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            anim.SetBool("fall", false);
            anim.SetBool("land", true);
        }
        else if (!isGrounded && rb.velocity.y < 0)
        {
            anim.SetBool("jump", false);
            anim.SetBool("fall", true);
        }
    }

    #endregion

    private void Update()
    {
        if(gameManager.gameType == GameType.PlayerSeek)
        {
            if (!gameManager.startGame)
                return;
        }

        JumpDecend();
        MovePlayer();
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheck.position - new Vector3(0, groundDistance / 2, 0), boxCastSize);
        }
    }
}
