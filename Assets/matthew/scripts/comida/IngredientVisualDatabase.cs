using System;
using System.Collections.Generic;
using UnityEngine;


// Relaciona cada ingrediente del inventario con el prefab
// que debe mostrarse visualmente sobre una estación.

[CreateAssetMenu(
    fileName = "IngredientVisualDatabase",
    menuName = "Restaurant/Ingredient Visual Database"
)]
public class IngredientVisualDatabase : ScriptableObject
{
    [Serializable]
    public class IngredientVisualEntry
    {
        [Header("Ingrediente")]
        public Ingredientes ingredient;

        [Header("Prefab visual")]
        public GameObject visualPrefab;

        [Header("Ajuste de posición")]
        public Vector3 localPosition;

        [Header("Ajuste de rotación")]
        public Vector3 localRotation;

        [Header("Ajuste de escala")]
        public Vector3 localScale = Vector3.one;
    }

    [Header("Visuales disponibles")]
    [SerializeField] private List<IngredientVisualEntry> ingredientVisuals = new List<IngredientVisualEntry>();

    
    // Busca la configuración visual correspondiente
    // al ingrediente solicitado.
    
    public bool TryGetVisual(Ingredientes ingredient,out IngredientVisualEntry visualEntry)
    {
        foreach (IngredientVisualEntry entry in ingredientVisuals)
        {
            if (entry.ingredient == ingredient)
            {
                visualEntry = entry;
                return true;
            }
        }

        visualEntry = null;
        return false;
    }
}
