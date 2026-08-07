using UnityEngine;

public class DeliveryCounter : MonoBehaviour, IInteractable
{
    [Header("Referencias")]
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private OrderManager orderManager;

    public void Interact()
    {
        if (!ValidateReferences())
        {
            return;
        }

        // No existe ningún pedido activo.
        if (!orderManager.HasActiveOrder ||orderManager.CurrentOrder == null)
        {
            NotificationUI.Instance?.ShowMessage("No hay ningún pedido activo.");

            return;
        }

        // Intentamos completar el pedido.
        bool completed =orderManager.TryCompleteCurrentOrder(playerInventory);

        // El jugador no tiene la comida necesaria.
        if (!completed)
        {
            NotificationUI.Instance?.ShowMessage("No tienes el platillo solicitado.");

            return;
        }
    }

    public string GetInteractionText()
    {
        if (orderManager == null ||
            !orderManager.HasActiveOrder)
        {
            return "No hay pedido activo";
        }

        return "Entregar pedido";
    }

    private bool ValidateReferences()
    {
        if (playerInventory == null)
        {
            Debug.LogError("Falta asignar Player Inventory en DeliveryCounter.");

            return false;
        }

        if (orderManager == null)
        {
            Debug.LogError("Falta asignar Order Manager en DeliveryCounter.");

            return false;
        }

        return true;
    }
}