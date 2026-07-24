using System.Collections;
using UnityEngine;

public class CookingStation : MonoBehaviour, IInteractable
{
    [Header("Referencias")]
    [SerializeField] private Inventory playerInventory;

    // Se encarga de mostrar el ingrediente sobre la estación.
    [SerializeField] private StationIngredientVisual ingredientVisual;

    [Header("Configuración de cocción")]
    [SerializeField] private float cookingTime = 4f;

    [Header("Efectos opcionales")]
    [Tooltip("Objeto visual como fuego, humo o luz de la cocina.")]
    [SerializeField] private GameObject cookingEffect;

    // Ingrediente que se encuentra actualmente en la estación.
    private Ingredientes currentIngredient;

    // Ingrediente que se obtendrá después de cocinar.
    private Ingredientes resultingIngredient;

    // Indica que existe un ingrediente sobre la estación.
    private bool hasIngredient;

    // Indica que el ingrediente está cocinándose.
    private bool isCooking;

    // Indica que el ingrediente terminó de cocinarse.
    private bool ingredientIsCooked;

    private void Awake()
    {
        // El efecto debe comenzar apagado.
        SetCookingEffect(false);
    }

    
    // Se ejecuta cuando el jugador interactúa con la estación.
    
    public void Interact()
    {
        if (!ValidateReferences())
        {
            return;
        }

        // Si la estación está vacía, intenta colocar un ingrediente.
        if (!hasIngredient)
        {
            TryPlaceIngredient();
            return;
        }

        // No permite nuevas interacciones mientras cocina.
        if (isCooking)
        {
            Debug.Log("El ingrediente todavía se está cocinando.");
            return;
        }

        // Si ya terminó de cocinarse, entrega el resultado.
        if (ingredientIsCooked)
        {
            CollectResult();
            return;
        }

        // Si hay un ingrediente crudo, comienza la cocción.
        StartCoroutine(CookIngredient());
    }

    
    // Busca en el inventario algún ingrediente que pueda cocinarse.
    // El primer ingrediente disponible será colocado en la estación.   
    private void TryPlaceIngredient()
    {
        // Carne cruda -> carne cocinada.
        if (TryUseIngredient(Ingredientes.CarneCruda,Ingredientes.Carne))
        {
            return;
        }

        // Papas cortadas -> papas cocinadas.
        if (TryUseIngredient(Ingredientes.PapasCortadas,Ingredientes.PapasCocinadas))
        {
            return;
        }

        // Plátano verde cortado -> plátano verde cocinado.
        if (TryUseIngredient(Ingredientes.PlatanoVerdeCortado,Ingredientes.PlatanoVerdeCocinado))
        {
            return;
        }

        // Plátano maduro cortado -> plátano maduro cocinado.
        if (TryUseIngredient(Ingredientes.PlatanoMaduroCortado,Ingredientes.PlatanoMaduroCocinado))
        {
            return;
        }

        Debug.Log("No tienes ningún ingrediente que pueda cocinarse.");
    }

    
    // Comprueba si existe un ingrediente en el inventario.
    // Si existe, lo consume y muestra su visual en la estación.
    
    private bool TryUseIngredient(Ingredientes originalIngredient,Ingredientes cookedResult)
    {
        bool ingredientUsed =
            playerInventory.IntentarUsarIngrediente(originalIngredient,1);

        if (!ingredientUsed)
        {
            return false;
        }

        currentIngredient = originalIngredient;
        resultingIngredient = cookedResult;

        // Muestra el ingrediente crudo en la estación.
        bool visualShown =ingredientVisual.ShowIngredient(currentIngredient);

        // Si falta el visual, devuelve el ingrediente al inventario.
        if (!visualShown)
        {
            playerInventory.AñadirIngrediente(originalIngredient,1);

            Debug.LogError($"No se pudo mostrar el visual de " +$"{originalIngredient}.");

            ResetStation();
            return false;
        }

        hasIngredient = true;
        isCooking = false;
        ingredientIsCooked = false;

        Debug.Log($"{originalIngredient} fue colocado en la cocina.");

        return true;
    }

    
    // Cocina el ingrediente durante el tiempo configurado.
    
    private IEnumerator CookIngredient()
    {
        isCooking = true;

        SetCookingEffect(true);

        Debug.Log($"Comenzando a cocinar {currentIngredient}.");

        yield return new WaitForSeconds(cookingTime);

        // Reemplaza el visual crudo por el visual cocinado.
        bool resultVisualShown =
            ingredientVisual.ShowIngredient(resultingIngredient);

        if (!resultVisualShown)
        {
            Debug.LogError($"No existe un visual configurado para " +$"{resultingIngredient}.");

            SetCookingEffect(false);
            isCooking = false;

            yield break;
        }

        SetCookingEffect(false);

        isCooking = false;
        ingredientIsCooked = true;

        Debug.Log($"{currentIngredient} fue convertido en " +$"{resultingIngredient}.");
    }

    
    // Agrega el resultado cocinado al inventario
    // y deja la estación vacía.
    
    private void CollectResult()
    {
        playerInventory.AñadirIngrediente(resultingIngredient,1);

        ingredientVisual.ClearVisual();

        Debug.Log($"Recogiste {resultingIngredient} de la cocina.");

        ResetStation();
    }

    
    // Activa o desactiva el efecto visual de cocción.    
    private void SetCookingEffect(bool active)
    {
        if (cookingEffect != null)
        {
            cookingEffect.SetActive(active);
        }
    }

     
    // Reinicia todas las variables internas de la estación.
   
    private void ResetStation()
    {
        hasIngredient = false;
        isCooking = false;
        ingredientIsCooked = false;

        currentIngredient = default;
        resultingIngredient = default;

        SetCookingEffect(false);
    }

    
    // Comprueba que las referencias importantes estén asignadas.
    
    private bool ValidateReferences()
    {
        if (playerInventory == null)
        {
            Debug.LogError("Falta asignar Player Inventory en CookingStation.");

            return false;
        }

        if (ingredientVisual == null)
        {
            Debug.LogError("Falta asignar StationIngredientVisual " +"en CookingStation.");

            return false;
        }

        return true;
    }
}