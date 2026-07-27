using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FridgeUIItem : MonoBehaviour
{
    public enum ItemAction
    {
        Store,
        Take
    }

    [Header("Referencias de interfaz")]
    [SerializeField] private Image ingredientIcon;
    [SerializeField] private TMP_Text ingredientNameText;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private TMP_Text actionText;
    [SerializeField] private Button actionButton;

    private Ingredientes ingredient;
    private ItemAction currentAction;
    private FridgeUI fridgeUI;

    private void Awake()
    {
        if (actionButton != null)
        {
            actionButton.onClick.AddListener(ExecuteAction);
        }
        else
        {
            Debug.LogError("Falta asignar Action Button en FridgeUIItem.");
        }
    }

    public void Configure(Ingredientes newIngredient,Sprite icon,int amount,ItemAction action,FridgeUI owner)
    {
        ingredient = newIngredient;
        currentAction = action;
        fridgeUI = owner;

        if (ingredientIcon != null)
        {
            ingredientIcon.sprite = icon;
            ingredientIcon.enabled = icon != null;
        }

        if (ingredientNameText != null)
        {
            ingredientNameText.text = IngredientDisplayName.Get(newIngredient);
        }

        if (amountText != null)
        {
            amountText.text = $"x{amount}";
        }

        if (actionText != null)
        {
            actionText.text =currentAction == ItemAction.Store? "Guardar": "Retirar";
        }
    }

    private void ExecuteAction()
    {
        

        if (fridgeUI == null)
        {
            Debug.LogError("FridgeUI no está asignado.");
            return;
        }

        if (currentAction == ItemAction.Store)
        {
            fridgeUI.StoreIngredient(ingredient);
        }
        else
        {
            fridgeUI.TakeIngredient(ingredient);
        }
    }
}