using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class InputCapture : MonoBehaviour
{
    [Header("Player Input Value")]
    public Vector2 move;
    public bool sprint;
    public bool jump;
    public bool crouch;

    public bool interact;
    public bool mouse;
    
    [Header("Mouse Input values")]
    public Vector2 look;

    [Header("Mouve Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLock = true;


    #region InputSystem dependency
#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        LookInput(value.Get<Vector2>());
    }

    public void OnCrouch(InputValue value)
    {

    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    public void OnInventory(InputValue value)
    {

    }

    public void OnInteract(InputValue value)
    {

    }
#endif
    #endregion

    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }
    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;

    }

    public void CrouchInput(bool newCrouchState)
    {
        crouch = newCrouchState;
    }

    public void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
    }

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

}
