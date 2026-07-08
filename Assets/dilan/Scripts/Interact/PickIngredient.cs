using UnityEngine;

public class PickIngredient : MonoBehaviour, IInteractable
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Ingredientes ingredientName;

    public void Interact()
    {
        playerInventory.AñadirIngrediente(ingredientName, 1);
    }
}
