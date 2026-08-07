using UnityEngine;

public class CookFood : MonoBehaviour, IInteractable
{

    [Header("Configuración del Requisito")]
    [SerializeField] private Ingredientes ingredienteRequerido;
    [SerializeField] private int cantidadRequerida = 1;

    [SerializeField] private Ingredientes PreparedFood;

    [Header("Referencias")]
    [SerializeField] private Inventory playerInventory;

    
    public void Interact()
    {

        ComprobarRequisito();

    }
    public string GetInteractionText()
    {
        return "Interactuar";
    }

    void ComprobarRequisito()
    {
        if (playerInventory == null)
        {
            Debug.LogError("Falta asignar la referencia al Inventario del Jugador en el Inspector.");
            return;
        }

        
        int cantidadActual = playerInventory.ObtenerCantidad(ingredienteRequerido);

        if (cantidadActual >= cantidadRequerida)
        {
             
            playerInventory.UsarIngrediente(ingredienteRequerido, cantidadRequerida);

            AconteceraAlgo();
        }
        else
        {
            int falta = cantidadRequerida - cantidadActual;
            Debug.Log($"No tienes suficiente {ingredienteRequerido}. Tienes: {cantidadActual}, te faltan: {falta}.");
        }
    }

    
    void AconteceraAlgo()
    {
        playerInventory.AñadirIngrediente(PreparedFood, 1);

    }
}
