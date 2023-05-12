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
    [Tooltip("Mouse sensitivity")]
    public float mouseSensitivity = 0.1f;
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float rotationSmoothTime = 0.12f;
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

    [Header("Raycast")]
    //[Tooltip("The range that the raycast can reach")]
    //public float rayCastRange = 5.0f;
    [Tooltip("The layer it should be detect")]
    public LayerMask rayCastLayer;

    //[Header("Detection")]
    //[Tooltip("The detection offset, Set 0 to stick to player, set value to move it front of player, set negative, behind of player.")]
    //public float detectOffset = 1f;
    //[Tooltip("This control the detection radius, the detector is a sphere")]
    //public float detectorRadius = 0.5f;

    [Header(" Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject cinemachineCameraTarget;
    [Tooltip("Top Camera clamp")]
    public float topClamp = 70.0f;
    [Tooltip("Bottom Camera clamp")]
    public float bottomClamp = 30.0f;
    [Tooltip("Additional degree")]
    public float cameraAngleOverride = 0.0f;
    [Tooltip("To lock the camera position on all axis")]
    public bool lockCameraPosition = false;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private float _threshold = 0.01f;
    private bool _interactableFound;

    private CharacterController _characterController;
    private InputCapture _inputCatcher;
    private GameObject _mainCamera;
    private Camera _camera;
    private IInteractable interactable;

    private Animator _animator;
    private bool _animFound;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;

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

        if (_mainCamera != null)
        {
            _camera = _mainCamera.GetComponent<Camera>();
        }
    }

    private void Start()
    {
        _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;

        _animFound = TryGetComponent(out _animator);
        // Set Component
        _characterController = GetComponent<CharacterController>();
        _inputCatcher = GetComponent<InputCapture>();

        //AssignAnimationIDs();

        // Reset Timeouts on Start
        _jumpTimeoutDelta = jumpTimeout;
        _fallTimeoutDelta = fallTimeout;
    }

    private void Update()
    {
        //_animFound = TryGetComponent(out _animator)
        if (_inputCatcher.cursorLocked)
        {
            JumpAndGravity();
            Move();
            RaycastTarget();
            CameraRotation();
        }

        GroundedCheck();
    }

    //private void AssignAnimationIDs()
    //{
    //    _animIDSpeed = Animator.StringToHash("Speed");
    //    _animIDGrounded = Animator.StringToHash("Grounded");
    //    _animIDJump = Animator.StringToHash("Jump");
    //    _animIDFreeFall = Animator.StringToHash("FreeFall");

    //}

    private void OnDrawGizmos()
    {

    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        onGround = Physics.CheckSphere(spherePosition, groundCheckRadius, groundLayers, QueryTriggerInteraction.Ignore);

        // update animator if using character
        if (_animFound)
        {
            _animator.SetBool(_animIDGrounded, onGround);
        }
    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_inputCatcher.look.sqrMagnitude >= _threshold && !lockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = 1.0f;

            _cinemachineTargetYaw += _inputCatcher.look.x * deltaTimeMultiplier * mouseSensitivity;
            _cinemachineTargetPitch += -_inputCatcher.look.y * deltaTimeMultiplier * mouseSensitivity;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

        // Cinemachine will follow this target
        cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }

    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _inputCatcher.sprint ? sprintSpeed : moveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_inputCatcher.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * accelerationRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * accelerationRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(_inputCatcher.move.x, 0.0f, _inputCatcher.move.y).normalized;

        // if there is a move input rotate player when the player is moving
        if (_inputCatcher.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        _characterController.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        // update animator if using character
        if (_animFound)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
        }
    }

    private void JumpAndGravity()
    {
        if (onGround)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = fallTimeout;

            // update animator if using character
            if (_animFound)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (_inputCatcher.jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);

                // update animator if using character
                if (_animFound)
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = jumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                if (_animFound)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }

            // if we are not grounded, do not jump
            _inputCatcher.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += gravity * Time.deltaTime;
        }
    }

    //private void InteractableDetect()
    //{
    //    Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + detectOffset);
    //    Collider[] interactorDetector = Physics.OverlapSphere(spherePosition, detectorRadius, rayCastLayer);
    //    Physics.OverlapSphereNonAlloc(spherePosition, detectorRadius, interactorDetector, rayCastLayer);

    //    if (interactorDetector.Length > 0)
    //    {
    //        interactorDetector[0].TryGetComponent<IInteractable>(out interactable);

    //        if (interactable != null && _inputCatcher.interact)
    //        {
    //            interactable.Interact(this);
    //            Debug.Log("Found and pressed");
    //        }

    //    }
    //    _inputCatcher.interact = false;
    //}

    private void RaycastTarget()
    {
        // This was use for First Person, I edited the RayCast fly infinitely
        Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);
        Ray ray = _camera.ScreenPointToRay(point);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, rayCastLayer))
        {
            Debug.Log(hit.transform.name);
            hit.transform.TryGetComponent<IInteractable>(out interactable);

            //hit.transform.GetComponent<IInteractable>();
            if (interactable != null)
            {
                MessageManager.Instance.hintText.SetActive(true);

            }
            else
            {
                MessageManager.Instance.hintText.SetActive(false);
            }

            if (interactable != null && _inputCatcher.interact)
            {
                interactable.Interact(this);
            }
        }
        
        
            

        _inputCatcher.interact = false;

    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
