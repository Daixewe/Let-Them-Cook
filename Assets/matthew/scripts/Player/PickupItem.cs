using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    
    public void Interact()
    {
        PlayerPickup player = FindAnyObjectByType<PlayerPickup>();

        if (player == null)
            return;

        if (!player.HasItem())
        {
            player.PickUp(this);
        }
    }

    public string GetInteractionText()
    {
        return "Interactuar";
    }


}
