using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenSwitch : MonoBehaviour, IInteractable
{
    [Header("References")]
    public GameObject DoorToOpen;

    private Vector3 _doorOriPosition;
    private Vector3 _doorOpenPosition;

    [Tooltip("This is the Prompt player press message, show what to press when facing this NPC")]
    [SerializeField] private string _prompt = "E";
    [Tooltip("This is the default message if there is no MessageSO loaded")]
    [SerializeField] private string _defaultMessage = "(Stares at you and says nothing...)";

    public string interactionPrompt => _prompt;

    public string defaultMessage => _defaultMessage;

    private void Start()
    {
        _doorOriPosition = DoorToOpen.transform.localPosition;
        _doorOpenPosition = new Vector3(_doorOriPosition.x, _doorOriPosition.y + 10f, _doorOriPosition.z);
        
    }
    public void Interact(PlayerScript interactable)
    {
        DoorToOpen.transform.localPosition = _doorOpenPosition;
    }
}
