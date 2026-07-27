using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrillSlotUIItem : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private TMP_Text slotNameText;
    [SerializeField] private TMP_Text ingredientNameText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button collectButton;
    [SerializeField] private Slider cookingProgress;

    private int slotIndex;
    private Action<int> onCollectPressed;

    private void Awake()
    {
        ValidateReferences();

        if (collectButton != null)
        {
            collectButton.onClick.AddListener(HandleCollectButton);
        }
    }

    private void OnDestroy()
    {
        if (collectButton != null)
        {
            collectButton.onClick.RemoveListener(HandleCollectButton);
        }
    }

    public void Setup(int newSlotIndex, Action<int> collectCallback)
    {
        slotIndex = newSlotIndex;
        onCollectPressed = collectCallback;

        if (slotNameText != null)
        {
            slotNameText.text = $"Espacio {slotIndex + 1}";
        }

        ShowEmpty();
    }

    public void ShowEmpty()
    {
        if (ingredientNameText != null)
        {
            ingredientNameText.text = "Libre";
        }

        if (statusText != null)
        {
            statusText.text = "Disponible";
        }

        if (collectButton != null)
        {
            collectButton.interactable = false;
            collectButton.gameObject.SetActive(false);
        }

        if (cookingProgress != null)
        {
            cookingProgress.value = 0f;
            cookingProgress.gameObject.SetActive(false);
        }
    }

    public void ShowCooking(Ingredientes ingredient,float normalizedProgress,float remainingTime)
    {
        if (ingredientNameText != null)
        {
            ingredientNameText.text =IngredientDisplayName.Get(ingredient);
        }

        if (statusText != null)
        {
            statusText.text = $"Cocinando: {Mathf.Max(0f, remainingTime):0.0} s";
        }

        if (collectButton != null)
        {
            collectButton.interactable = false;
            collectButton.gameObject.SetActive(false);
        }

        if (cookingProgress != null)
        {
            cookingProgress.gameObject.SetActive(true);
            cookingProgress.value = Mathf.Clamp01(normalizedProgress);
        }
    }

    public void ShowReady(Ingredientes cookedIngredient)
    {
        if (ingredientNameText != null)
        {
            ingredientNameText.text = IngredientDisplayName.Get(cookedIngredient);
        }

        if (statusText != null)
        {
            statusText.text = "Listo para recoger";
        }

        if (collectButton != null)
        {
            collectButton.gameObject.SetActive(true);
            collectButton.interactable = true;
        }

        if (cookingProgress != null)
        {
            cookingProgress.gameObject.SetActive(true);
            cookingProgress.value = 1f;
        }
    }

    private void HandleCollectButton()
    {
        onCollectPressed?.Invoke(slotIndex);
    }
    private void ValidateReferences()
    {
        if (slotNameText == null)
        {
            Debug.LogError($"{nameof(GrillSlotUIItem)} en {name}: falta Slot Name Text.",this);
        }

        if (ingredientNameText == null)
        {
            Debug.LogError( $"{nameof(GrillSlotUIItem)} en {name}: falta Ingredient Name Text.",this);
        }

        if (statusText == null)
        {
            Debug.LogError($"{nameof(GrillSlotUIItem)} en {name}: falta Status Text.",this);
        }

        if (collectButton == null)
        {
            Debug.LogError($"{nameof(GrillSlotUIItem)} en {name}: falta Collect Button.",this);
        }

        if (cookingProgress == null)
        {
            Debug.LogWarning($"{nameof(GrillSlotUIItem)} en {name}: no se asignó Cooking Progress.",this);
        }
    }
}