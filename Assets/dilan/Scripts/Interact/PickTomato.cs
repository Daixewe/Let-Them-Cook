using UnityEngine;

public class PickTomato : MonoBehaviour, IInteractable
{
    [SerializeField] private Inventory playerInventory;

    public void Interact()
    {
        playerInventory.AñadirIngrediente("Tomato", 1);
    }
}
