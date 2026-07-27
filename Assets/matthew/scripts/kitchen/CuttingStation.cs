using System.Collections;
using UnityEngine;

public class CuttingStation : MonoBehaviour, IInteractable
{
    [Header("Referencias")]
    [SerializeField] private Inventory playerInventory;

    // Componente encargado de mostrar el ingrediente sobre la tabla.
    [SerializeField] private StationIngredientVisual ingredientVisual;

    [SerializeField] private PlayerPickup playerPickup;

    [Header("Configuración del corte")]
    [SerializeField] private float cuttingTime = 2f;

    

    // Ingrediente colocado actualmente sobre la tabla.
    private Ingredientes currentIngredient;

    // Resultado que se obtendrá después de cortar.
    private Ingredientes resultingIngredient;

    // Indica si actualmente hay algo colocado sobre la tabla.
    private bool hasIngredient;

    // Indica si el ingrediente ya terminó de cortarse.
    private bool ingredientIsCut;

    // Evita interactuar varias veces mientras se está cortando.
    private bool isCutting;

    
    // Se ejecuta cuando el jugador interactúa con la tabla.
   
    public void Interact()
    {
        // Verificamos que las referencias necesarias estén asignadas.
        if (!ValidateReferences())
        {
            return;
        }

        // Si la tabla está vacía, intenta colocar un ingrediente.
        if (!hasIngredient)
        {
            TryPlaceIngredient();
            return;
        }

        // Si ya terminó el corte, entrega el resultado al inventario.
        if (ingredientIsCut)
        {
            CollectResult();
            return;
        }

        // Si todavía no está cortado, inicia el proceso de corte.
        if (!isCutting)
        {
            if (TryGetHeldKnife(out KnifeTool knifeTool))
            {
                StartCoroutine(
                    CutIngredient(knifeTool)
                );
            }
        }
    }

    
    // Intenta encontrar y consumir un ingrediente que pueda cortarse.
    
    private void TryPlaceIngredient()
    {
        // Se prueban los ingredientes en este orden.
        // El primero disponible será colocado sobre la tabla.

        if (TryUseIngredient(Ingredientes.TomateSinCortar,Ingredientes.TomateCortado))
        {
            return;
        }

        if (TryUseIngredient(Ingredientes.LechugaSinCortar,Ingredientes.LechugaCortada))
        {
            return;
        }

        if (TryUseIngredient(Ingredientes.PlatanoVerdeSinCortar,Ingredientes.PlatanoVerdeCortado))
        {
            return;
        }

        Debug.Log("No tienes ningún ingrediente que pueda cortarse.");
    }

    
    // Comprueba si el jugador tiene el ingrediente indicado.
    // Si lo tiene, lo consume y muestra su representación visual.
    
    private bool TryUseIngredient(
        Ingredientes originalIngredient,
        Ingredientes cutResult)
    {
        // Usa el método auxiliar que agregamos al inventario.
        bool ingredientUsed =
            playerInventory.IntentarUsarIngrediente(originalIngredient,1);

        if (!ingredientUsed)
        {
            return false;
        }

        // Guardamos la receta activa de esta tabla.
        currentIngredient = originalIngredient;
        resultingIngredient = cutResult;

        // Intentamos mostrar el ingrediente sobre la tabla.
        bool visualShown =ingredientVisual.ShowIngredient(currentIngredient);

        // Si el visual no pudo crearse, devolvemos el ingrediente
        // para evitar que el jugador lo pierda.
        if (!visualShown)
        {
            playerInventory.AñadirIngrediente(originalIngredient,1);

            Debug.LogError($"No se pudo mostrar el visual de " +$"{originalIngredient}.");

            return false;
        }

        hasIngredient = true;
        ingredientIsCut = false;

        Debug.Log($"{originalIngredient} fue colocado sobre la tabla.");

        return true;
    }

    
    // Simula el tiempo necesario para cortar el ingrediente.
    
    private IEnumerator CutIngredient(KnifeTool knifeTool)
    {
        isCutting = true;

        Debug.Log($"Comenzando a cortar {currentIngredient}.");

        
        // reproducimos la animación del cuchillo.
        yield return StartCoroutine(knifeTool.PlayCutAnimation());

        yield return new WaitForSeconds(Mathf.Max(0f, cuttingTime - 0.5f));

        bool resultVisualShown =ingredientVisual.ShowIngredient(resultingIngredient);

        if (!resultVisualShown)
        {
            Debug.LogError($"No existe un visual configurado para " +$"{resultingIngredient}.");

            isCutting = false;
            yield break;
        }

        ingredientIsCut = true;
        isCutting = false;

        Debug.Log($"{currentIngredient} fue convertido en " +$"{resultingIngredient}.");
    }

    
    // Retira el resultado de la tabla y lo agrega al inventario.
    
    private void CollectResult()
    {
        playerInventory.AñadirIngrediente(resultingIngredient,1);

        // Eliminamos solamente el objeto visual.
        ingredientVisual.ClearVisual();

        Debug.Log($"Recogiste {resultingIngredient} de la tabla.");

        ResetStation();
    }

    
    // Restablece el estado interno para que la tabla pueda reutilizarse.
    
    private void ResetStation()
    {
        hasIngredient = false;
        ingredientIsCut = false;
        isCutting = false;

        currentIngredient = default;
        resultingIngredient = default;
    }

   
    // Comprueba las referencias configuradas desde el Inspector.
  
    private bool ValidateReferences()
    {
        if (playerInventory == null)
        {
            Debug.LogError("Falta asignar Player Inventory en CuttingStation.");

            return false;
        }

        if (ingredientVisual == null)
        {
            Debug.LogError("Falta asignar StationIngredientVisual " +"en CuttingStation.");

            return false;
        }
        if (playerPickup == null)
        {
            Debug.LogError("Falta asignar PlayerPickup en CuttingStation.");

            return false;
        }

        return true;
    }

    private bool TryGetHeldKnife(
    out KnifeTool knifeTool)
    {
        knifeTool = null;

        if (playerPickup == null)
        {
            Debug.LogError("Falta asignar PlayerPickup en CuttingStation.");

            return false;
        }

        // Comprobamos si el jugador tiene algún objeto.
        if (!playerPickup.HasItem())
        {
            Debug.Log("Necesitas sostener un cuchillo para cortar." );

            return false;
        }

        PickupItem heldItem =
            playerPickup.GetHeldItem();

        if (heldItem == null)
        {
            Debug.Log("No se encontró el objeto sostenido.");

            return false;
        }

        ToolItem toolItem =
            heldItem.GetComponent<ToolItem>();

        // El objeto sostenido debe ser una herramienta.
        if (toolItem == null)
        {
            Debug.Log("El objeto que sostienes no es una herramienta.");

            return false;
        }

        // La herramienta debe ser específicamente un cuchillo.
        if (!toolItem.IsTool(ToolItem.ToolType.Knife))
        {
            Debug.Log("Necesitas un cuchillo para cortar.");

            return false;
        }

        knifeTool =heldItem.GetComponent<KnifeTool>();

        if (knifeTool == null)
        {
            Debug.LogError("El cuchillo no tiene el componente KnifeTool.");

            return false;
        }

        return true;
    }
}