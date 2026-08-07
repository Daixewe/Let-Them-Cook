using UnityEngine;

public class PlateFood : MonoBehaviour, IInteractable
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private Ingredientes ingredient1;
    [SerializeField] private Ingredientes ingredient2;
    [SerializeField] private Ingredientes cookedFood;

    public void Interact()
    {
        TryToCook();
    }

    public string GetInteractionText()
    {
        return "Interactuar";
    }

    void TryToCook()
    {
        Ingredientes ing1 = ingredient1;
        Ingredientes ing2 = ingredient2;

        // Usamos el método que creamos en el inventario
        if (playerInventory.TieneIngredientesEspecificos(ing1, ing2))
        {
            // ¡AQUÍ PASA ALGO! (El código que querías ejecutar)
            Debug.Log("¡Felicidades! Tienes los ingredientes necesarios.");
            _ = CookingProcess(ing1, ing2);
        }
        else
        {
            // Qué pasa si no los tiene
            Debug.LogWarning($"No puedes cocinar. Te falta {ing1} o {ing2} (o ambos).");
        }
    }

    private bool CookingProcess(Ingredientes ing1, Ingredientes ing2)
    {

        playerInventory.ConsumirIngredientes(ing1, ing2);
        playerInventory.AñadirIngrediente(cookedFood, 1);

        
        return true;
    }
}
