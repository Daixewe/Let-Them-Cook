using UnityEngine;

public class PickChesse : MonoBehaviour, IInteractable
{
    [SerializeField] private Inventory playerInventory;

    public void Interact()
    {
        playerInventory.AñadirIngrediente("Chesse", 1);
    }
}
