using System;
using UnityEngine;

[Serializable]
public class RecipeIngredient
{
    [SerializeField]private Ingredientes ingredient;

    [SerializeField]private int quantity = 1;

    public Ingredientes Ingredient => ingredient;

    public int Quantity => quantity;
}