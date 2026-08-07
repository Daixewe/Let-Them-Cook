using UnityEngine;

public class SeedShop : MonoBehaviour, IInteractable
{
    [Header("Referencias del jugador")]
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private MoneyManager moneyManager;

    [Header("Semilla que vende esta tienda")]
    [SerializeField]
    private Ingredientes seedType =
        Ingredientes.SemillaTomate;

    [SerializeField] private int seedAmount = 1;

    [Header("Precio")]
    [SerializeField] private int seedPrice = 10;

    public void Interact()
    {
        // Comprobamos que el inventario esté asignado.
        if (playerInventory == null)
        {
            Debug.LogError(
                "Falta asignar el Inventory en SeedShop."
            );

            return;
        }

        // Comprobamos que el sistema de dinero esté asignado.
        if (moneyManager == null)
        {
            Debug.LogError(
                "Falta asignar el MoneyManager en SeedShop."
            );

            return;
        }

        BuySeed();
    }

    public string GetInteractionText()
    {
        return "Interactuar";
    }

    private void BuySeed()
    {
        // Evitamos configuraciones incorrectas.
        if (seedAmount <= 0)
        {
            Debug.LogWarning(
                "La cantidad de semillas debe ser mayor que cero."
            );

            return;
        }

        if (seedPrice <= 0)
        {
            Debug.LogWarning(
                "El precio de la semilla debe ser mayor que cero."
            );

            return;
        }

        // Intentamos cobrar el precio.
        bool purchaseSuccessful =moneyManager.TrySpendMoney(seedPrice);

        // Si no había suficiente dinero, cancelamos la compra.
        if (!purchaseSuccessful)
        {
            return;
        }

        // Agregamos las semillas al inventario.
        playerInventory.AñadirIngrediente(seedType,seedAmount);

        Debug.Log($"Compraste {seedAmount} de {seedType} " +$"por ${seedPrice}.");
    }
}