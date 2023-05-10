using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputCapture))]
public class PlayerScript : MonoBehaviour
{
    [Header("Player Self Stats")]
    [Tooltip("Move speed of the character")]
    public float moveSpeed = 5.0f;
    [Tooltip("Move speed of the character when sprinting")]
    public float sprintSpeed = 10.0f;
    [Tooltip("Move speed of the character when crouching")]
    public float crouchSpeed = 2.0f;
    [Tooltip("The rate of acceleration/deceleration")]
    public float accelerationRate = 10.0f;
    [Tooltip("Jump height of character")]
    public float jumpForce = 1.2f;
    [Tooltip("Gravity value - control how fast player fall")]
    public float gravity = -9.81f;
    [Tooltip("Time cooldown before next jump")]
    public float jumpTimeout = 0.50f;
    [Tooltip("Time cooldown before entering the fall state. Stairs")]
    public float fallTimeout = 0.15f;
    [Tooltip("Check if the player is on ground")]
    public bool onGround = false;
    [Tooltip("The offset for the player ground check")]
    public float groundedOffset = -0.14f;
    [Tooltip("How big does the radius check for ground check")]
    public float groundCheckRadius = -0.14f;
    [Tooltip("Which layer does the ground check")]
    public LayerMask groundLayers;

    // 
    private float _targetSpeed;
    private float _speed;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity;

    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private float _gravity;

    private CharacterController _characterController;
    private InputCapture _inputCatcher;
    private GameObject _mainCamera;

    private Animator _animator;
    private bool _animFound;

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        if (TryGetComponent<Animator>(out _animator))
        {
            _animFound = true;
        }
    }

    private void Start()
    {
        // Set Component
        _characterController = GetComponent<CharacterController>();
        _inputCatcher = GetComponent<InputCapture>();

        // Reset Timeouts on Start
        _jumpTimeoutDelta = jumpTimeout;
        _fallTimeoutDelta = fallTimeout;

    }

    private void Update()
    {
        
    }
   
    private void GroundedCheck()
    {
        Vector3 groundCheckSpherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        onGround = Physics.CheckSphere(groundCheckSpherePosition, groundCheckRadius, groundLayers, QueryTriggerInteraction.Ignore);
    }

    private void OnDrawGizmos()
    {
        if(onGround)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z), groundCheckRadius);
    }
}
