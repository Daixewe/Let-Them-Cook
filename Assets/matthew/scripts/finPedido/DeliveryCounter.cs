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

        if (!orderManager.HasActiveOrder)
        {
            Debug.LogWarning("No hay ningún pedido activo para entregar.");

            return;
        }

        bool completed =orderManager.TryCompleteCurrentOrder(playerInventory);

        if (!completed)
        {
            Debug.LogWarning("No tienes el platillo solicitado en el inventario.");

            return;
        }

        Debug.Log("Pedido entregado correctamente.");
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
