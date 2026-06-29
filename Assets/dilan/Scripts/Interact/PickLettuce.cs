using UnityEngine;

public class PickLettuce : MonoBehaviour, IInteractable
{
    [SerializeField] private Inventory playerInventory;

    public void Interact()
    {
        playerInventory.AñadirIngrediente("Lettuce", 1);
    }
}
