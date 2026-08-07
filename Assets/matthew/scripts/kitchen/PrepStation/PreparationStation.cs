using System.Collections.Generic;
using UnityEngine;

public class PreparationStation :MonoBehaviour,IInteractable
{
    [Header("Inventario del jugador")]
    [SerializeField] private Inventory playerInventory;

    [Header("Interfaz")]
    [SerializeField] private PreparationUI preparationUI;

    [Header("Recetas disponibles")]
    [SerializeField]
    private List<RecipeData> availableRecipes = new();

    public void Interact()
    {
        if (preparationUI == null)
        {
            Debug.LogError("PreparationStation no tiene asignado PreparationUI.");

            return;
        }

        preparationUI.Open(this);
    }

    public string GetInteractionText()
    {
        return "Preparar platillo";
    }

    public IReadOnlyList<RecipeData>GetAvailableRecipes()
    {
        return availableRecipes;
    }

    public bool CanPrepareRecipe(RecipeData recipe)
    {
        if (!ValidateRecipe(recipe))
        {
            return false;
        }

        if (playerInventory == null)
        {
            Debug.LogError("PreparationStation no tiene asignado Inventory.");

            return false;
        }

        foreach (RecipeIngredient requiredIngredient in recipe.RequiredIngredients)
        {
            int availableQuantity =playerInventory.ObtenerCantidad(requiredIngredient.Ingredient);

            if (availableQuantity <requiredIngredient.Quantity)
            {
                return false;
            }
        }

        return HasSpaceForResult(recipe);
    }

    public bool TryPrepareRecipe(RecipeData recipe)
    {
        if (!ValidateRecipe(recipe))
        {
            return false;
        }

        if (playerInventory == null)
        {
            Debug.LogError("PreparationStation no tiene asignado Inventory.");

            return false;
        }

        if (!HasAllRequiredIngredients(recipe))
        {
            Debug.LogWarning("No tienes todos los ingredientes necesarios.");

            return false;
        }

        if (!HasSpaceForResult(recipe))
        {
            Debug.LogWarning("No hay suficiente espacio en el inventario " +"para recibir el platillo.");

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

        bool resultAdded =playerInventory.AñadirIngrediente(recipe.ResultIngredient,recipe.ResultQuantity);

        if (!resultAdded)
        {
            Debug.LogError("No se pudo agregar el resultado de la receta.");

            return false;
        }

        Debug.Log($"Preparaste {recipe.ResultQuantity} x " +$"{IngredientDisplayName.Get(recipe.ResultIngredient)}.");

        return true;
    }

    private bool HasAllRequiredIngredients(RecipeData recipe)
    {
        foreach (RecipeIngredient requiredIngredient in recipe.RequiredIngredients)
        {
            int availableQuantity = playerInventory.ObtenerCantidad(requiredIngredient.Ingredient);

            if (availableQuantity <requiredIngredient.Quantity)
            {
                return false;
            }
        }

        return true;
    }

    private bool HasSpaceForResult( RecipeData recipe)
    {
        int ingredientsConsumed = 0;

        foreach (RecipeIngredient requiredIngredient in recipe.RequiredIngredients)
        {
            ingredientsConsumed += requiredIngredient.Quantity;
        }

        int finalInventoryCount =playerInventory.GetCurrentItemCount() - ingredientsConsumed + recipe.ResultQuantity;

        return finalInventoryCount <=playerInventory.MaxSlots;
    }

    private bool ValidateRecipe(RecipeData recipe)
    {
        if (recipe == null)
        {
            Debug.LogError("No se recibió una receta válida.");

            return false;
        }

        if (recipe.RequiredIngredients == null ||recipe.RequiredIngredients.Count == 0)
        {
            Debug.LogError($"La receta {recipe.name} no tiene ingredientes.");

            return false;
        }

        if (recipe.ResultQuantity <= 0)
        {
            Debug.LogError($"La receta {recipe.name} tiene una cantidad " +"de resultado inválida.");

            return false;
        }

        foreach (
            RecipeIngredient ingredient in recipe.RequiredIngredients)
        {
            if (ingredient.Quantity <= 0)
            {
                Debug.LogError($"La receta {recipe.name} contiene una " +"cantidad de ingrediente inválida.");

                return false;
            }
        }

        return true;
    }
}