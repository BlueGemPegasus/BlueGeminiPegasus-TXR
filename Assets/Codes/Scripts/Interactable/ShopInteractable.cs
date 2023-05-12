using UnityEngine;

public class ShopInteractable : MonoBehaviour, IInteractable
{
    [Tooltip("This is the Prompt player press message.")]
    [SerializeField] private string _prompt = "E";
    [Tooltip("This is the default message if there is no MessageSO loaded")]
    [SerializeField] private string _defaultMessage = "(Stares at you and says nothing...)";

    public string interactionPrompt => _prompt;

    public string defaultMessage => _defaultMessage;

    public void Interact(PlayerScript interactable)
    {
    
    }
}
