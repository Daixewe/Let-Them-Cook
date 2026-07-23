using UnityEngine;

public class CuttingStation : MonoBehaviour, IInteractable
{
    [Header("Referencia al inventario")]
    [SerializeField] private Inventory playerInventory;

    public void Interact()
    {
        // Verificamos que el inventario esté asignado.
        if (playerInventory == null)
        {
            Debug.LogError("Falta asignar el Inventory del jugador en CuttingStation.");

            return;
        }

        // La tabla revisa qué ingrediente sin cortar tiene el jugador.
        // Se procesa solamente el primero que encuentre disponible.

        if (IntentarCortar(Ingredientes.TomateSinCortar,Ingredientes.TomateCortado))
        {
            return;
        }

        if (IntentarCortar(Ingredientes.LechugaSinCortar,Ingredientes.LechugaCortada))
        {
            return;
        }

        if (IntentarCortar(Ingredientes.PlatanoVerdeSinCortar,Ingredientes.PlatanoVerdeCortado))
        {
            return;
        }

        if (IntentarCortar(Ingredientes.PlatanoMaduroSinCortar,Ingredientes.PlatanoMaduroCortado))
        {
            return;
        }

        // Este mensaje aparece cuando no hay ningún ingrediente cortable.
        Debug.LogWarning("No tienes ningún ingrediente que se pueda cortar.");
    }

    private bool IntentarCortar(Ingredientes ingredienteSinCortar,Ingredientes ingredienteCortado)
    {
        // Revisamos si el jugador tiene al menos una unidad.
        if (playerInventory.ObtenerCantidad(ingredienteSinCortar) <= 0)
        {
            return false;
        }

        // Consumimos una unidad del ingrediente original.
        bool ingredienteConsumido =
            playerInventory.IntentarUsarIngrediente(ingredienteSinCortar,1);

        // Si por alguna razón no se pudo consumir, cancelamos.
        if (!ingredienteConsumido)
        {
            return false;
        }

        // Agregamos una unidad del ingrediente cortado.
        playerInventory.AñadirIngrediente(ingredienteCortado,1);

        Debug.Log($"Cortaste 1 de {ingredienteSinCortar} " +$"y obtuviste 1 de {ingredienteCortado}.");

        return true;
    }
}