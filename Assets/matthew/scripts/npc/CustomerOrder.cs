using UnityEngine;

public class CustomerOrder : MonoBehaviour, IInteractable
{
    [Header("Referencias")]
    [SerializeField] private NPC npc;

    private OrderManager orderManager;

    private bool hasReachedRegister;
    private bool hasRequestedOrder;

    private void Awake()
    {
        if (npc == null)
        {
            npc = GetComponent<NPC>();
        }

        orderManager =FindFirstObjectByType<OrderManager>();

        if (orderManager == null)
        {
            Debug.LogError("No se encontró un OrderManager en la escena.");
        }
    }

    private void OnEnable()
    {
        if (orderManager != null)
        {
            orderManager.OnOrderCompleted +=HandleOrderCompleted;
        }
    }

    private void OnDisable()
    {
        if (orderManager != null)
        {
            orderManager.OnOrderCompleted -=HandleOrderCompleted;
        }
    }

    public void Interact()
    {
        if (!hasReachedRegister)
        {
            Debug.LogWarning("El cliente todavía no ha llegado a la caja.");

            return;
        }

        if (hasRequestedOrder)
        {
            return;
        }

        if (orderManager == null)
        {
            Debug.LogError("No hay un OrderManager disponible.");

            return;
        }

        if (orderManager.HasActiveOrder)
        {
            Debug.LogWarning("Ya existe un pedido activo.");

            return;
        }

        bool orderCreated =orderManager.CreateRandomOrder();

        if (!orderCreated)
        {
            return;
        }

        hasRequestedOrder = true;

        if (npc != null &&orderManager.CurrentOrder != null)
        {
            npc.StartPatience(orderManager.CurrentOrder.PatienceTime);
        }
    }

    public string GetInteractionText()
    {
        return "Interactuar";
    }

    public void SetReachedRegister()
    {
        hasReachedRegister = true;
    }

    private void HandleOrderCompleted(OrderData completedOrder)
    {
        if (!hasRequestedOrder)
        {
            return;
        }

        if (npc != null)
        {
            npc.StopPatience();
            npc.StartLeaving();
        }
    }

    public void HandleCustomerPatienceExpired()
    {
        if (orderManager != null && orderManager.HasActiveOrder)
        {
            orderManager.FailCurrentOrderFromCustomer();
        }
    }
}