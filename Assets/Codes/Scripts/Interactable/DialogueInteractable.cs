using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInteractable : MonoBehaviour, IInteractable
{
    [Tooltip("The messageSO to load into Dialogue")]
    public MessageSO messageToLoad;

    [Tooltip("This is the Prompt player press message, show what to press when facing this NPC")]
    [SerializeField] private string _prompt = "E";
    [Tooltip("This is the default message if there is no MessageSO loaded")]
    [SerializeField] private string _defaultMessage = "(Stares at you and says nothing...)";

    public string interactionPrompt => _prompt;

    public string defaultMessage => _defaultMessage;


    public void Interact(PlayerScript interactable)
    {
        if (messageToLoad != null)
        {
            MessageManager.Instance.StartMessage(messageToLoad);
        }
        else
        {
            MessageManager.Instance.StartMessage(_defaultMessage);
        }
    }
}
