using UnityEngine;

public class IngredientPickupStation : MonoBehaviour, IInteractable
{
    [Header("Ingrediente")]
    [SerializeField] private Ingredientes ingredient;
    [SerializeField] private int amount = 1;

    [Header("Inventario del jugador")]
    [SerializeField] private Inventory playerInventory;

    public void Interact()
    {
        if (playerInventory == null)
        {
            Debug.LogError("No se asignó el Inventory.");
            return;
        }

        playerInventory.AñadirIngrediente(ingredient, amount);

        Debug.Log($"Se obtuvo {amount} x {IngredientDisplayName.Get(ingredient)}");
    }

    public string GetInteractionText()
    {
        return "Recoger";
    }
}
