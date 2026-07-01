using UnityEngine;

public class PickIngredient : MonoBehaviour, IInteractable
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private string ingredientName;

    public void Interact()
    {
        playerInventory.AñadirIngrediente(ingredientName, 1);
    }
}
