using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe",menuName = "Let Them Cook/Recipe")]
public class RecipeData : ScriptableObject
{
    [Header("Información de la receta")]
    [SerializeField] private string recipeName;
    [SerializeField] private Sprite recipeIcon;

    [Header("Ingredientes necesarios")]
    [SerializeField]private List<RecipeIngredient> requiredIngredients = new();

    [Header("Resultado")]
    [SerializeField] private Ingredientes resultIngredient;
    [SerializeField] private int resultQuantity = 1;

    public string RecipeName => recipeName;
    public Sprite RecipeIcon => recipeIcon;

    public IReadOnlyList<RecipeIngredient>
        RequiredIngredients => requiredIngredients;

    public Ingredientes ResultIngredient =>resultIngredient;

    public int ResultQuantity =>resultQuantity;
}