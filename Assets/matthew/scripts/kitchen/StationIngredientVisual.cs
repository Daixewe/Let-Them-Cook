using UnityEngine;


// Controla el ingrediente visual que aparece sobre una estación.

// El objeto mostrado es solamente una representación.
// El ingrediente real continúa siendo administrado por Inventory.

public class StationIngredientVisual : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField]
    private IngredientVisualDatabase visualDatabase;

    [Header("Punto donde aparecerá el ingrediente")]
    [SerializeField]
    private Transform visualPoint;

    // Guarda la representación visual actualmente mostrada.
    private GameObject currentVisual;


    // Muestra el prefab visual correspondiente al ingrediente.


   
    public bool ShowIngredient(Ingredientes ingredient)
    {
        // Primero eliminamos cualquier visual anterior.
        ClearVisual();

        if (visualDatabase == null)
        {
            Debug.LogError($"Falta asignar IngredientVisualDatabase en {name}.");

            return false;
        }

        if (visualPoint == null)
        {
            Debug.LogError($"Falta asignar Visual Point en {name}.");

            return false;
        }

        // Buscamos la configuración del ingrediente.
        bool visualFound =visualDatabase.TryGetVisual(ingredient,out IngredientVisualDatabase.IngredientVisualEntry visualEntry);

        if (!visualFound)
        {
            Debug.LogWarning($"No existe un prefab visual configurado " +$"para {ingredient}.");

            return false;
        }

        if (visualEntry.visualPrefab == null)
        {
            Debug.LogWarning($"El ingrediente {ingredient} no tiene " +$"un prefab visual asignado.");

            return false;
        }

        // Creamos la representación como hija del VisualPoint.
        currentVisual = Instantiate(visualEntry.visualPrefab,visualPoint);

        // Aplicamos los ajustes guardados en la base de datos.
        currentVisual.transform.localPosition =visualEntry.localPosition;

        currentVisual.transform.localRotation =Quaternion.Euler(visualEntry.localRotation);

        currentVisual.transform.localScale =visualEntry.localScale;
        return true;
    }

     
    // Elimina el ingrediente visual actual.
    // No modifica el inventario.
   
    public void ClearVisual()
    {
        if (currentVisual != null)
        {
            Destroy(currentVisual);
            currentVisual = null;
        }
    }

    
    // Indica si actualmente hay un ingrediente visible.
    
    public bool HasVisual()
    {
        return currentVisual != null;
    }
}