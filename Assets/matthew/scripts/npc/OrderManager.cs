using System;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    [Header("Pedidos disponibles")]
    [SerializeField] private List<OrderData> availableOrders = new();

    [Header ("Dinero")]
    [SerializeField] private MoneyManager moneyManager;

    private OrderData currentOrder;
    private float remainingTime;
    private bool hasActiveOrder;

    public event Action OnOrderChanged;
    public event Action<OrderData> OnOrderCompleted;
    public event Action<OrderData> OnOrderFailed;

    public bool HasActiveOrder =>hasActiveOrder;

    public OrderData CurrentOrder =>currentOrder;

    public float RemainingTime =>remainingTime;

    public float NormalizedRemainingTime
    {
        get
        {
            if (currentOrder == null ||currentOrder.PatienceTime <= 0f)
            {
                return 0f;
            }

            return Mathf.Clamp01(remainingTime /currentOrder.PatienceTime);
        }
    }

    private void Update()
    {
        if (!hasActiveOrder ||currentOrder == null)
        {
            return;
        }

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            FailCurrentOrder();
        }
    }

    public bool CreateRandomOrder()
    {
        if (hasActiveOrder)
        {
            Debug.LogWarning("Ya existe un pedido activo.");

            return false;
        }

        if (availableOrders == null ||availableOrders.Count == 0)
        {
            Debug.LogError("No hay pedidos disponibles en OrderManager.");

            return false;
        }

        List<OrderData> validOrders = new();

        foreach (OrderData order in availableOrders)
        {
            if (order != null)
            {
                validOrders.Add(order);
            }
        }

        if (validOrders.Count == 0)
        {
            Debug.LogError("La lista de pedidos solo contiene elementos vacíos.");

            return false;
        }

        int randomIndex =UnityEngine.Random.Range(0,validOrders.Count);

        return StartOrder(validOrders[randomIndex]);
    }

    public bool StartOrder(OrderData newOrder)
    {
        if (newOrder == null)
        {
            Debug.LogError("No se recibió un pedido válido.");

            return false;
        }

        if (hasActiveOrder)
        {
            Debug.LogWarning("No se puede comenzar otro pedido mientras hay uno activo.");

            return false;
        }

        currentOrder = newOrder;
        Debug.Log($"Nuevo pedido: {currentOrder.OrderName}");
        remainingTime = newOrder.PatienceTime;
        hasActiveOrder = true;

        OnOrderChanged?.Invoke();

        return true;
    }

    public bool IsCorrectDish(Ingredientes dish,int amount = 1)
    {
        if (!hasActiveOrder ||currentOrder == null)
        {
            return false;
        }

        return dish ==currentOrder.RequestedDish &&amount >=currentOrder.RequestedAmount;
    }

    public bool TryCompleteCurrentOrder(Inventory playerInventory)
    {
        if (!hasActiveOrder ||currentOrder == null)
        {
            Debug.LogWarning("No existe un pedido activo.");

            return false;
        }

        if (playerInventory == null)
        {
            Debug.LogError("No se recibió el inventario del jugador.");

            return false;
        }

        int availableAmount =playerInventory.ObtenerCantidad(currentOrder.RequestedDish);

        if (availableAmount <currentOrder.RequestedAmount)
        {
            Debug.LogWarning("El jugador no tiene el platillo solicitado.");

            return false;
        }

        bool consumed =playerInventory.IntentarUsarIngrediente(currentOrder.RequestedDish,currentOrder.RequestedAmount);

        if (!consumed)
        {
            return false;
        }

        CompleteCurrentOrder();

        return true;
    }

    public void FailCurrentOrderFromCustomer()
    {
        if (!hasActiveOrder ||currentOrder == null)
        {
            return;
        }

        FailCurrentOrder();
    }

    private void CompleteCurrentOrder()
    {
        OrderData completedOrder = currentOrder;

        Debug.Log($"Pedido completado: {completedOrder.OrderName}");
        Debug.Log($"Recompensa: {completedOrder.Reward}");

        if (moneyManager != null)
        {
            moneyManager.AddMoney(completedOrder.Reward);
            Debug.Log("Dinero agregado.");
        }
        else
        {
            Debug.LogError("Falta asignar MoneyManager en OrderManager.");
            Debug.LogError("MoneyManager es NULL.");
        }

        ClearCurrentOrder();

        OnOrderCompleted?.Invoke(completedOrder);
        OnOrderChanged?.Invoke();
    }

    private void FailCurrentOrder()
    {
        OrderData failedOrder =currentOrder;

        ClearCurrentOrder();

        OnOrderFailed?.Invoke(failedOrder);

        OnOrderChanged?.Invoke();
    }

    private void ClearCurrentOrder()
    {
        currentOrder = null;
        remainingTime = 0f;
        hasActiveOrder = false;
    }
}