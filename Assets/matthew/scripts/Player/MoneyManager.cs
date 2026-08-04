using System;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [Header("Dinero")]
    [SerializeField] private int currentMoney;

    public int CurrentMoney => currentMoney;

    public event Action OnMoneyChanged;

    public void AddMoney(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("La cantidad de dinero debe ser mayor que cero.");

            return;
        }

        currentMoney += amount;

        OnMoneyChanged?.Invoke();
    }

    public bool TrySpendMoney(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (currentMoney < amount)
        {
            return false;
        }

        currentMoney -= amount;

        OnMoneyChanged?.Invoke();

        return true;
    }
}