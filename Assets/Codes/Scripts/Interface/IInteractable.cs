using UnityEngine;

public interface IInteractable
{
    public string interactionPrompt { get; }
    public string defaultMessage { get; }

    public void Interact(PlayerScript interactable);

}
