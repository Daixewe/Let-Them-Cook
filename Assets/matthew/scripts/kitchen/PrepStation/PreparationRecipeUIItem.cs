using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreparationRecipeUIItem : MonoBehaviour
{
    [Header("Elementos de la interfaz")]
    [SerializeField] private TMP_Text recipeNameText;
    [SerializeField] private TMP_Text ingredientsText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Image recipeIcon;
    [SerializeField] private Button prepareButton;

    private RecipeData currentRecipe;
    private Action<RecipeData> onPreparePressed;

    public void Configure(RecipeData recipe,bool canPrepare,Action<RecipeData> prepareAction)
    {
        currentRecipe = recipe;
        onPreparePressed = prepareAction;

        if (currentRecipe == null)
        {
            Debug.LogError("PreparationRecipeUIItem recibió una receta nula.");

            return;
        }

        recipeNameText.text = currentRecipe.RecipeName;
        ingredientsText.text = BuildIngredientsText();

        if (recipeIcon != null)
        {
            recipeIcon.sprite = currentRecipe.RecipeIcon;

            recipeIcon.gameObject.SetActive(currentRecipe.RecipeIcon != null);
        }

        if (canPrepare)
        {
            statusText.text = "Disponible";
            prepareButton.interactable = true;
        }
        else
        {
            statusText.text = "Faltan ingredientes";
            prepareButton.interactable = false;
        }

        prepareButton.onClick.RemoveAllListeners();
        prepareButton.onClick.AddListener(PrepareRecipe);
    }

    private string BuildIngredientsText()
    {
        string result = "";

        foreach (RecipeIngredient ingredient in currentRecipe.RequiredIngredients)
        {
            result +=
                $"{IngredientDisplayName.Get(ingredient.Ingredient)} x{ingredient.Quantity}\n";
        }

        return result.TrimEnd();
    }

    private void PrepareRecipe()
    {
        if (currentRecipe == null)
        {
            Debug.LogError("No existe una receta asignada al botón.");

            return;
        }

        onPreparePressed?.Invoke(currentRecipe);
 
    }
}
