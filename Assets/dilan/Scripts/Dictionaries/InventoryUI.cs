using System.Text;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private TMP_Text inventoryText;

    private void OnEnable()
    {
        // Verificamos que el inventario esté asignado.
        if (playerInventory == null)
        {
            Debug.LogError(
                "Falta asignar el Inventory del jugador en InventoryUI."
            );

            return;
        }

        // Nos suscribimos al evento del inventario.
        // Cada vez que cambie, la interfaz se actualizará.
        playerInventory.OnInventoryChanged += UpdateInventoryUI;

        // Actualizamos el texto al iniciar.
        UpdateInventoryUI();
    }

    private void OnDisable()
    {
        // Eliminamos la suscripción cuando se desactiva el objeto.
        // Esto evita errores o llamadas duplicadas.
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= UpdateInventoryUI;
        }
    }

    private void UpdateInventoryUI()
    {
        // Verificamos que el texto esté asignado.
        if (inventoryText == null)
        {
            Debug.LogError(
                "Falta asignar el texto TMP en InventoryUI."
            );

            return;
        }

        // Usamos StringBuilder para construir el texto eficientemente.
        StringBuilder inventoryBuilder = new StringBuilder();

        inventoryBuilder.AppendLine("<b>INVENTARIO</b>");

        bool hasIngredients = false;

        // Recorremos todos los elementos definidos en el enum Ingredientes.
        foreach (Ingredientes ingrediente in
                 System.Enum.GetValues(typeof(Ingredientes)))
        {
            int cantidad =
                playerInventory.ObtenerCantidad(ingrediente);

            // Solo mostramos ingredientes que el jugador tenga.
            if (cantidad > 0)
            {
                hasIngredients = true;

                inventoryBuilder.AppendLine(
                    $"{GetDisplayName(ingrediente)}: {cantidad}"
                );
            }
        }

        // Si el inventario está vacío, mostramos un mensaje.
        if (!hasIngredients)
        {
            inventoryBuilder.AppendLine("Vacío");
        }

        // Asignamos el texto construido al componente TMP.
        inventoryText.text = inventoryBuilder.ToString();
    }

    private string GetDisplayName(Ingredientes ingrediente)
    {
        // Este método permite mostrar nombres más fáciles de leer.
        switch (ingrediente)
        {
            case Ingredientes.CarneCruda:
                return "Carne cruda";

            case Ingredientes.TomateSinCortar:
                return "Tomate sin cortar";

            case Ingredientes.TomateCortado:
                return "Tomate cortado";

            case Ingredientes.LechugaSinCortar:
                return "Lechuga sin cortar";

            case Ingredientes.LechugaCortada:
                return "Lechuga cortada";

            case Ingredientes.PapasCrudas:
                return "Papas crudas";

            case Ingredientes.PapasCortadas:
                return "Papas cortadas";

            case Ingredientes.PapasCocinadas:
                return "Papas cocinadas";

            case Ingredientes.SemillaTomate:
                return "Semillas de tomate";

            case Ingredientes.SemillaLechuga:
                return "Semillas de lechuga";

            case Ingredientes.SemillaPapa:
                return "Semillas de papa";

            case Ingredientes.PlatanoVerdeSinCortar:
                return "Plátano verde sin cortar";

            case Ingredientes.PlatanoVerdeCortado:
                return "Plátano verde cortado";

            case Ingredientes.PlatanoVerdeCocinado:
                return "Plátano verde cocinado";

            case Ingredientes.PlatanoMaduroSinCortar:
                return "Plátano maduro sin cortar";

            case Ingredientes.PlatanoMaduroCortado:
                return "Plátano maduro cortado";

            case Ingredientes.PlatanoMaduroCocinado:
                return "Plátano maduro cocinado";

            case Ingredientes.huevo:
                return "Buebo";

            default:
                // Para los ingredientes sencillos usamos el nombre del enum.
                return ingrediente.ToString();
        }
    }
}