using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderUI : MonoBehaviour
{
    [Header("Sistema de pedidos")]
    [SerializeField] private OrderManager orderManager;

    [Header("Panel")]
    [SerializeField] private GameObject orderPanel;

    [Header("Contenido")]
    [SerializeField] private TMP_Text dishNameText;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Image dishIcon;
    [SerializeField] private Slider patienceSlider;

    private void Awake()
    {
        if (orderPanel != null)
        {
            orderPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (orderManager != null)
        {
            orderManager.OnOrderChanged += Refresh;
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (orderManager != null)
        {
            orderManager.OnOrderChanged -= Refresh;
        }
    }

    private void Update()
    {
        if (orderManager == null ||!orderManager.HasActiveOrder ||orderManager.CurrentOrder == null)
        {
            return;
        }

        UpdateTimer();
    }

    private void Refresh()
    {
        if (orderManager == null)
        {
            return;
        }

        bool hasActiveOrder =orderManager.HasActiveOrder &&orderManager.CurrentOrder != null;

        if (orderPanel != null)
        {
            orderPanel.SetActive(hasActiveOrder);
        }

        if (!hasActiveOrder)
        {
            return;
        }

        OrderData order = orderManager.CurrentOrder;

        if (dishNameText != null)
        {
            dishNameText.text = order.OrderName;
        }

        if (amountText != null)
        {
            amountText.text =$"Cantidad: {order.RequestedAmount}";
        }

        if (dishIcon != null)
        {
            dishIcon.sprite = order.OrderIcon;dishIcon.gameObject.SetActive(order.OrderIcon != null);
        }

        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (orderManager == null ||orderManager.CurrentOrder == null)
        {
            return;
        }

        if (timeText != null)
        {
            timeText.text =$"Tiempo: {orderManager.RemainingTime:0} s";
        }

        if (patienceSlider != null)
        {
            patienceSlider.value =orderManager.NormalizedRemainingTime;
        }
    }
}