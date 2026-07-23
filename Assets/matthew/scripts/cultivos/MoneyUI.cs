using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private TMP_Text moneyText;

    private void OnEnable()
    {
        // Comprobamos que el sistema de dinero exista.
        if (moneyManager == null)
        {
            Debug.LogError(
                "Falta asignar MoneyManager en MoneyUI."
            );

            return;
        }

        // Nos suscribimos a los cambios de dinero.
        moneyManager.OnMoneyChanged += UpdateMoneyUI;

        // Mostramos el dinero inicial.
        UpdateMoneyUI();
    }

    private void OnDisable()
    {
        // Eliminamos la suscripción para evitar llamadas duplicadas.
        if (moneyManager != null)
        {
            moneyManager.OnMoneyChanged -= UpdateMoneyUI;
        }
    }

    private void UpdateMoneyUI()
    {
        // Comprobamos que el texto esté asignado.
        if (moneyText == null)
        {
            Debug.LogError(
                "Falta asignar MoneyText en MoneyUI."
            );

            return;
        }

        moneyText.text =
            $"Dinero: ${moneyManager.CurrentMoney}";
    }
}
