using Unity.Mathematics;
using UnityEngine;

public class CostumerScript : MonoBehaviour, IInteractable
{

    [Header("Configuración del Requisito")]
    [SerializeField] private Ingredientes ComidaRequerida;
    [SerializeField] private int cantidadRequerida = 1;
   

    [Header("Referencias")]
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private GameManager gameManager;

    

    public void Interact()
    {

        ComprobarRequisito();

    }

    void ComprobarRequisito()
    {
        if (playerInventory == null)
        {
            Debug.LogError("Falta asignar la referencia al Inventario del Jugador en el Inspector.");
            return;
        }


        int cantidadActual = playerInventory.ObtenerCantidad(ComidaRequerida);

        if (cantidadActual >= cantidadRequerida)
        {

            playerInventory.UsarIngrediente(ComidaRequerida, cantidadRequerida);

            ComidaAceptada();
        }
        else
        {
            int falta = cantidadRequerida - cantidadActual;
            Debug.Log($"No tienes suficiente {ComidaRequerida}. Tienes: {cantidadActual}, te faltan: {falta}.");
        }
    }


    void ComidaAceptada()
    {
        gameManager.OrdenCompletada();
        Debug.Log($"Orden Completada");
        

    }
}


