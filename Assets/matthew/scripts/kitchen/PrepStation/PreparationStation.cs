using System.Collections.Generic;
using UnityEngine;

public class PreparationStation : MonoBehaviour, IInteractable
{
    [Header("Inventario del jugador")]
    [SerializeField] private Inventory playerInventory;

    [Header("Interfaz")]
    [SerializeField] private PreparationUI preparationUI;

    [Header("Recetas disponibles")]
    [SerializeField]private List<RecipeData> availableRecipes = new();

    public void Interact()
    {
        if (preparationUI == null)
        {
            Debug.LogError("PreparationStation no tiene asignado PreparationUI.");

            return;
        }

        preparationUI.Open(this);
    }

    public IReadOnlyList<RecipeData> GetAvailableRecipes()
    {
        return availableRecipes;
    }

    public bool CanPrepareRecipe(RecipeData recipe)
    {
        if (recipe == null)
        {
            Debug.LogError("No se recibió una receta válida.");

            return false;
        }

        if (playerInventory == null)
        {
            Debug.LogError("PreparationStation no tiene asignado el Inventory.");

            return false;
        }

        foreach (RecipeIngredient requiredIngredient in recipe.RequiredIngredients)
        {
            int availableQuantity =playerInventory.ObtenerCantidad(requiredIngredient.Ingredient);

            if (availableQuantity < requiredIngredient.Quantity)
            {
                return false;
            }
        }

        return true;
    }

    public bool TryPrepareRecipe(RecipeData recipe)
    {
        if (!CanPrepareRecipe(recipe))
        {
            Debug.LogWarning("No tienes todos los ingredientes necesarios.");

            return false;
        }

        foreach (RecipeIngredient requiredIngredient in recipe.RequiredIngredients)
        {
            bool ingredientConsumed =playerInventory.IntentarUsarIngrediente(requiredIngredient.Ingredient,requiredIngredient.Quantity);

            if (!ingredientConsumed)
            {
                Debug.LogError("No se pudo consumir uno de los ingredientes.");

                return false;
            }
        }

        playerInventory.AñadirIngrediente(recipe.ResultIngredient,recipe.ResultQuantity);

        return true;
    }
}