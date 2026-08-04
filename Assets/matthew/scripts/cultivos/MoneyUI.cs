using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private TMP_Text moneyText;

    private void OnEnable()
    {
        if (moneyManager != null)
        {
            moneyManager.OnMoneyChanged += Refresh;
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (moneyManager != null)
        {
            moneyManager.OnMoneyChanged -= Refresh;
        }
    }

    private void Refresh()
    {
        if (moneyManager == null ||moneyText == null)
        {
            return;
        }

        moneyText.text =$"₡{moneyManager.CurrentMoney}";
    }
}