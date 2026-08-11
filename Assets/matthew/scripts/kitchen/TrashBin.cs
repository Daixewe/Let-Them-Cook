using UnityEngine;

public class TrashBin :
    MonoBehaviour,
    IInteractable
{
    [Header("Referencias")]
    [SerializeField] private Inventory playerInventory;

    public void Interact()
    {
        if (playerInventory == null)
        {
            Debug.LogError("Falta asignar Player Inventory en TrashBin.");

            return;
        }

        bool removed =playerInventory.RemoveFirstIngredient(out Ingredientes removedIngredient);

        if (!removed)
        {
            NotificationUI.Instance?.ShowMessage("El inventario está vacío.");

            return;
        }

        NotificationUI.Instance?.ShowMessage($"Desechaste {IngredientDisplayName.Get(removedIngredient)}.");
    }

    public string GetInteractionText()
    {
        return "Desechar ingrediente";
    }
}