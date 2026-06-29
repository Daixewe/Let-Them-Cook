using UnityEngine;

public class PickMeat : MonoBehaviour, IInteractable
{

    [SerializeField] private Inventory playerInventory;

    public void Interact()
    {
        playerInventory.AñadirIngrediente("Meat", 1);
    }
}
