using UnityEngine;

public class PickIngredient : MonoBehaviour, IInteractable
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Ingredientes ingredientName;
    [SerializeField] private int cantidad = 1;

    private bool collected;

    public void Interact()
    {
        if (collected)
            return;

        if (playerInventory == null)
        {
            Debug.LogError("Falta asignar el Inventory del jugador.");
            return;
        }

        collected = true;

        playerInventory.AñadirIngrediente(
            ingredientName,
            cantidad
        );

        Destroy(gameObject);
    }
}
