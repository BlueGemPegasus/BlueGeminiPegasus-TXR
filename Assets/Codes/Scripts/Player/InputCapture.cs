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

    public bool inventory;
    public bool interact;
    public bool mouse;

    [Header("Mouse Input values")]
    public Vector2 look;

    [Header("Mouve Cursor Settings")]
    public bool cursorLocked = true;

    private bool tempCursorState;

    #region InputSystem
#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        if (cursorLocked)
            MoveInput(value.Get<Vector2>());
        else
            MoveInput(Vector2.zero);
    }

    public void OnLook(InputValue value)
    {
        if (cursorLocked)
            LookInput(value.Get<Vector2>());
        else
            LookInput(Vector2.zero);
    }

    public void OnCrouch(InputValue value)
    {
        if (cursorLocked)
            CrouchInput(value.isPressed);
    }

    public void OnJump(InputValue value)
    {
        if (cursorLocked)
            JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        if (cursorLocked)
            SprintInput(value.isPressed);
    }

    public void OnInventory(InputValue value)
    {
        if(cursorLocked)
            InventoryInput(value.isPressed);
    }

    public void OnInteract(InputValue value)
    {
        if(cursorLocked)
            InteractInput(value.isPressed);
    }

    public void OnMouse(InputValue value)
    {
        //MouseInput(value.isPressed);
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

    public void InventoryInput(bool newInventoryState)
    {
        inventory = newInventoryState;
    }

    public void InteractInput(bool newInteractState)
    {
        interact = newInteractState;
    }

    public void MouseInput(bool newMouseState)
    {
        cursorLocked = !newMouseState;
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void OnApplicationFocus(bool focus)
    {
        SetCursorState(cursorLocked);
    }

    private void Start()
    {
        tempCursorState = cursorLocked;
    }

    private void Update()
    {
        if (cursorLocked != tempCursorState)
        {
            SetCursorState(cursorLocked);
            tempCursorState = cursorLocked;
        }
    }
}
