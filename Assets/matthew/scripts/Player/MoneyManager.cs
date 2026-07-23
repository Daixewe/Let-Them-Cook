using System;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [Header("Dinero del jugador")]
    [SerializeField] private int currentMoney = 100;

    // Evento utilizado para actualizar la interfaz del dinero.
    public event Action OnMoneyChanged;

    // Permite consultar el dinero desde otros scripts.
    public int CurrentMoney => currentMoney;

    /// <summary>
    /// Agrega dinero al jugador.
    /// </summary>
    public void AddMoney(int amount)
    {
        // Evitamos agregar cantidades inválidas.
        if (amount <= 0)
        {
            Debug.LogWarning(
                "La cantidad de dinero debe ser mayor que cero."
            );

            return;
        }

        currentMoney += amount;

        Debug.Log(
            $"Recibiste ${amount}. Dinero actual: ${currentMoney}."
        );

        // Avisamos a la interfaz que el dinero cambió.
        OnMoneyChanged?.Invoke();
    }

    
    /// Intenta gastar dinero.
    /// Devuelve true si la compra pudo realizarse.
   
    public bool TrySpendMoney(int amount)
    {
        // Comprobamos que el precio sea válido.
        if (amount <= 0)
        {
            Debug.LogWarning(
                "El precio debe ser mayor que cero."
            );

            return false;
        }

        // Comprobamos si el jugador tiene suficiente dinero.
        if (currentMoney < amount)
        {
            Debug.Log(
                $"No tienes suficiente dinero. " +
                $"Necesitas ${amount} y tienes ${currentMoney}."
            );

            return false;
        }

        // Restamos el precio.
        currentMoney -= amount;

        Debug.Log(
            $"Gastaste ${amount}. Dinero restante: ${currentMoney}."
        );

        // Avisamos a la interfaz que el dinero cambió.
        OnMoneyChanged?.Invoke();

        return true;
    }
}