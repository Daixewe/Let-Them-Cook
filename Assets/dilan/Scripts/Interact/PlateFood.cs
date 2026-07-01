using UnityEngine;

public class PlateFood : MonoBehaviour, IInteractable
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private string ingredient1;
    [SerializeField] private string ingredient2;
    [SerializeField] private string cookedFood;

    public void Interact()
    {
        TryToCook();
    }

    void TryToCook()
    {
        string ing1 = ingredient1;
        string ing2 = ingredient2;

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

    private bool CookingProcess(string ing1, string ing2)
    {

        playerInventory.ConsumirIngredientes(ing1, ing2);
        playerInventory.AñadirIngrediente(cookedFood, 1);

        Debug.Log("¡Has cocinado una deliciosa Pizza! 🍕");
        return true;
    }
}
