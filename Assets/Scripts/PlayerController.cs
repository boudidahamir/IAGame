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
    private Vector3 boxCastSize = new Vector3(0.5f, -0.1f, 0.5f);
    private float groundDistance = 0.9f;

    // Zoom
    private float mouseScrollY;
    public CinemachineFreeLook cineFreeLook;
/*    private float[] InitialOrbits;
*/
    private void Awake()
    {
        mvmspeed = mvmspeedControl;

        input = new CharacterControllerInputSystem();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        /*InitialOrbits = new float[3];
        for (int i = 0; i < InitialOrbits.Length; i++)
        {
            InitialOrbits[i] = cineFreeLook.m_Orbits[i].m_Radius;
        }*/
    }

    #region Enable/Disable Controls

    private void OnEnable()
    {
        input.Enable();
        input.NMVM.Movement.performed += OnMovementPerformed;
        input.NMVM.Movement.canceled += OnMovementCanceled;
        input.NMVM.Jump.performed += OnJumpPerformed;
/*        input.NMVM.ZoomInOut.performed += OnScrollMousePerformed;
        input.NMVM.ZoomInOut.canceled += OnScrollMouseCanceled;
        input.NMVM.ResetZoom.performed += OnZoomResetPerformed;*/
        input.NMVM.Sprint.performed += OnSprintPerformed;
        input.NMVM.Sprint.canceled += OnSprintCanceled;
    }

    private void OnDisable()
    {
        input.Disable();
        input.NMVM.Movement.performed -= OnMovementPerformed;
        input.NMVM.Movement.canceled -= OnMovementCanceled;
        input.NMVM.Jump.performed -= OnJumpPerformed;
/*        input.NMVM.ZoomInOut.performed -= OnScrollMousePerformed;
        input.NMVM.ZoomInOut.canceled -= OnScrollMouseCanceled;
        input.NMVM.ResetZoom.performed -= OnZoomResetPerformed;*/
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

    /*#region Camera Zoom

    private void OnScrollMousePerformed(InputAction.CallbackContext value)
    {
        if ((cineFreeLook.m_Orbits[0].m_Radius <= 15 && cineFreeLook.m_Orbits[1].m_Radius <= 21 && cineFreeLook.m_Orbits[2].m_Radius <= 15) ||
            (cineFreeLook.m_Orbits[0].m_Radius >= 2 && cineFreeLook.m_Orbits[1].m_Radius >= 8 && cineFreeLook.m_Orbits[2].m_Radius >= 2))
        {
            mouseScrollY = -value.ReadValue<float>();
        }
    }

    private void OnScrollMouseCanceled(InputAction.CallbackContext value)
    {
        mouseScrollY = 0;
    }

    private void OnZoomResetPerformed(InputAction.CallbackContext value)
    {
        for (int i = 0; i < InitialOrbits.Length; i++)
        {
            cineFreeLook.m_Orbits[i].m_Radius = InitialOrbits[i];
        }
    }

    private void ZoomInOut()
    {
        if (mouseScrollY != 0)
        {
            float[] minRadius = new float[3];
            float[] maxRadius = new float[3];
            minRadius[0] = 2f;
            maxRadius[0] = 15f;
            minRadius[1] = 2f;
            maxRadius[1] = 15f;
            minRadius[2] = 2f;
            maxRadius[2] = 15f;
            float zoomSpeed = 5.0f;

            bool anyRadiusOutOfRange = (cineFreeLook.m_Orbits[0].m_Radius >= minRadius[0]
                                        && cineFreeLook.m_Orbits[1].m_Radius >= minRadius[1]
                                        && cineFreeLook.m_Orbits[2].m_Radius >= minRadius[2]) &&

                                        (cineFreeLook.m_Orbits[0].m_Radius <= maxRadius[0]
                                        && cineFreeLook.m_Orbits[1].m_Radius <= maxRadius[1]
                                        && cineFreeLook.m_Orbits[2].m_Radius <= maxRadius[2]);

            if (anyRadiusOutOfRange)
            {
                for (int i = 0; i < InitialOrbits.Length; i++)
                {
                    float targetRadius = Mathf.Clamp(cineFreeLook.m_Orbits[i].m_Radius + mouseScrollY * Time.deltaTime * zoomSpeed, minRadius[i], maxRadius[i]);
                    cineFreeLook.m_Orbits[i].m_Radius = Mathf.Lerp(cineFreeLook.m_Orbits[i].m_Radius, targetRadius, Time.deltaTime * zoomSpeed);
                }
            }
        }
    }

    #endregion*/

    private void Update()
    {
        JumpDecend();
        MovePlayer();
/*        ZoomInOut();
*/    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheck.position - new Vector3(0, groundDistance / 2, 0), boxCastSize);
        }
    }
}
