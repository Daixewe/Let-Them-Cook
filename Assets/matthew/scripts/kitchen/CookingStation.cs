using System.Collections;
using UnityEngine;

public class CookingStation : MonoBehaviour, IInteractable
{
    [Header("Referencia al inventario")]
    [SerializeField] private Inventory playerInventory;

    [Header("Configuración de cocción")]
    [SerializeField] private float cookingTime = 5f;

    // Indica si la estación está cocinando actualmente.
    private bool isCooking;

    public void Interact()
    {
        // Verificamos que el inventario esté asignado.
        if (playerInventory == null)
        {
            Debug.LogError("Falta asignar el Inventory del jugador en CookingStation.");

            return;
        }

        // Impide comenzar otra cocción mientras la estación está ocupada.
        if (isCooking)
        {
            Debug.Log("La estación todavía está cocinando.");

            return;
        }

        // Buscamos el primer ingrediente disponible que pueda cocinarse.
        TryStartCooking();
    }

    private void TryStartCooking()
    {
        // La estación revisa los ingredientes siguiendo este orden.
        if (TryCookIngredient(Ingredientes.CarneCruda,Ingredientes.Carne))
        {
            return;
        }

        if (TryCookIngredient(Ingredientes.PapasCortadas,Ingredientes.PapasCocinadas))
        {
            return;
        }

        if (TryCookIngredient(Ingredientes.PlatanoVerdeCortado,Ingredientes.PlatanoVerdeCocinado))
        {
            return;
        }

        if (TryCookIngredient(Ingredientes.PlatanoMaduroCortado,Ingredientes.PlatanoMaduroCocinado))
        {
            return;
        }
        if (TryCookIngredient(Ingredientes.huevo, Ingredientes.huevoCocinado))
        {
            return;
        }

        // Aparece cuando el jugador no tiene ningún ingrediente cocinable.
        Debug.LogWarning(
            "No tienes ningún ingrediente que se pueda cocinar."
        );
    }

    private bool TryCookIngredient(Ingredientes ingredienteCrudo,Ingredientes ingredienteCocinado)
    {
        // Revisamos si el jugador tiene el ingrediente necesario.
        if (playerInventory.ObtenerCantidad(ingredienteCrudo) <= 0)
        {
            return false;
        }

        // Intentamos consumir una unidad del ingrediente crudo.
        bool ingredienteConsumido =
            playerInventory.IntentarUsarIngrediente(ingredienteCrudo,1);

        // Si no pudo consumirse, no iniciamos la cocción.
        if (!ingredienteConsumido)
        {
            return false;
        }

        // Iniciamos el proceso de cocción.
        StartCoroutine(CookingProcess(ingredienteCrudo,ingredienteCocinado));
        return true;
    }

    private IEnumerator CookingProcess(Ingredientes ingredienteCrudo,Ingredientes ingredienteCocinado)
    {
        // Marcamos la estación como ocupada.
        isCooking = true;

        Debug.Log($"Comenzando a cocinar {ingredienteCrudo}. " +$"Tiempo necesario: {cookingTime} segundos.");

        // Esperamos el tiempo configurado en el Inspector.
        yield return new WaitForSeconds(cookingTime);

        // Al finalizar, agregamos el ingrediente cocinado.
        playerInventory.AñadirIngrediente(ingredienteCocinado,1);

        // La estación vuelve a quedar disponible.
        isCooking = false;

        Debug.Log($"Terminaste de cocinar {ingredienteCrudo}. " +$"Recibiste 1 de {ingredienteCocinado}.");
    }
}