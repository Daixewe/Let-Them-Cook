using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrillIngredientUIItem : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private Image ingredientIcon;
    [SerializeField] private TMP_Text ingredientNameText;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Button cookButton;

    private Ingredientes ingredient;
    private Action<Ingredientes> onCookPressed;

    private void Awake()
    {
        ValidateReferences();

        if (cookButton != null)
        {
            cookButton.onClick.AddListener(HandleCookButton);
        }
    }

    private void OnDestroy()
    {
        if (cookButton != null)
        {
            cookButton.onClick.RemoveListener(HandleCookButton);
        }
    }

    public void Setup(Ingredientes newIngredient,int quantity,Action<Ingredientes> cookCallback,Sprite icon = null)
    {
        ingredient = newIngredient;
        onCookPressed = cookCallback;

        if (ingredientNameText != null)
        {
            ingredientNameText.text = IngredientDisplayName.Get(ingredient);
        }

        if (quantityText != null)
        {
            quantityText.text = $"Cantidad: {quantity}";
        }

        if (ingredientIcon != null)
        {
            ingredientIcon.sprite = icon;
            ingredientIcon.gameObject.SetActive(icon != null);
        }

        SetCookButtonInteractable(quantity > 0);
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantityText != null)
        {
            quantityText.text = $"Cantidad: {quantity}";
        }

        SetCookButtonInteractable(quantity > 0);
    }

    public void SetCookButtonInteractable(bool interactable)
    {
        if (cookButton != null)
        {
            cookButton.interactable = interactable;
        }
    }

    private void HandleCookButton()
    {
        onCookPressed?.Invoke(ingredient);
    }

    private string GetDisplayName(Ingredientes value)
    {
        return value.ToString().Replace("_", " ");
    }

    private void ValidateReferences()
    {
        if (ingredientNameText == null)
        {
            Debug.LogError($"{nameof(GrillIngredientUIItem)} en {name}: falta Ingredient Name Text.",this);
        }

        if (quantityText == null)
        {
            Debug.LogError($"{nameof(GrillIngredientUIItem)} en {name}: falta Quantity Text.",this);
        }

        if (cookButton == null)
        {
            Debug.LogError($"{nameof(GrillIngredientUIItem)} en {name}: falta Cook Button.",this);
        }
    }
}