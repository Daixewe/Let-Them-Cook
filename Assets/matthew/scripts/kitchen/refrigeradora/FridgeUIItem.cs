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
            actionButton.onClick.AddListener(
                ExecuteAction
            );
        }
        else
        {
            Debug.LogError(
                "Falta asignar Action Button en FridgeUIItem."
            );
        }
    }

    public void Configure(
        Ingredientes newIngredient,
        Sprite icon,
        int amount,
        ItemAction action,
        FridgeUI owner)
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
            ingredientNameText.text =
                GetReadableIngredientName(
                    newIngredient
                );
        }

        if (amountText != null)
        {
            amountText.text = $"x{amount}";
        }

        if (actionText != null)
        {
            actionText.text =
                currentAction == ItemAction.Store
                    ? "Guardar"
                    : "Retirar";
        }
    }

    private void ExecuteAction()
    {
        Debug.Log(
         $"CLICK: {ingredient} | Acción: {currentAction}"
     );

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
    /*private void ExecuteAction()
    {
        if (fridgeUI == null)
        {
            Debug.LogError(
                "FridgeUI no está asignado en FridgeUIItem."
            );

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
    }*/

    private string GetReadableIngredientName(
        Ingredientes value)
    {
        switch (value)
        {
            case Ingredientes.Carne:
                return "Carne";

            case Ingredientes.CarneCruda:
                return "Carne cruda";

            case Ingredientes.LechugaCortada:
                return "Lechuga cortada";

            case Ingredientes.LechugaSinCortar:
                return "Lechuga";

            case Ingredientes.TomateCortado:
                return "Tomate cortado";

            case Ingredientes.TomateSinCortar:
                return "Tomate";

            case Ingredientes.PapasCrudas:
                return "Papas crudas";

            case Ingredientes.PapasCortadas:
                return "Papas cortadas";

            case Ingredientes.PapasCocinadas:
                return "Papas cocinadas";

            case Ingredientes.Pan:
                return "Pan";

            case Ingredientes.huevo:
                return "Huevo";

            case Ingredientes.huevoCocinado:
                return "Huevo cocinado";

            case Ingredientes.SemillaTomate:
                return "Semilla de tomate";

            case Ingredientes.SemillaLechuga:
                return "Semilla de lechuga";

            case Ingredientes.SemillaPapa:
                return "Semilla de papa";

            case Ingredientes.PlatanoVerdeSinCortar:
                return "Plátano verde";

            case Ingredientes.PlatanoVerdeCortado:
                return "Plátano verde cortado";

            case Ingredientes.PlatanoVerdeCocinado:
                return "Plátano verde cocinado";

            case Ingredientes.PlatanoMaduroSinCortar:
                return "Plátano maduro";

            case Ingredientes.PlatanoMaduroCortado:
                return "Plátano maduro cortado";

            case Ingredientes.PlatanoMaduroCocinado:
                return "Plátano maduro cocinado";

            default:
                return value.ToString();
        }
    }
}