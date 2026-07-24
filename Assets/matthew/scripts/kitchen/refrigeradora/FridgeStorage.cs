using System;
using System.Collections.Generic;
using UnityEngine;

public class FridgeStorage : MonoBehaviour
{
    [Serializable]
    private class IngredientVisualData
    {
        public Ingredientes ingredient;
        public GameObject visualPrefab;

        [Header("Transformación dentro del Slot")]
        public Vector3 localPosition;
        public Vector3 localRotation;
        public Vector3 localScale = Vector3.one;
    }

    [Header("Inventario del jugador")]
    [SerializeField] private Inventory playerInventory;

    [Header("Espacios de almacenamiento")]
    [SerializeField] private List<FridgeSlot> slots = new();

    [Header("Visuales de ingredientes")]
    [SerializeField]
    private List<IngredientVisualData> ingredientVisuals = new();

    public event Action OnStorageChanged;

    public int Capacity => slots.Count;

    private void Start()//prueba
    {
        StoreIngredient(Ingredientes.CarneCruda);
        StoreIngredient(Ingredientes.huevo);
    }

    public int OccupiedSlots
    {
        get
        {
            int occupied = 0;

            foreach (FridgeSlot slot in slots)
            {
                if (slot != null &&
                    slot.IsOccupied)
                {
                    occupied++;
                }
            }

            return occupied;
        }
    }

    public int FreeSlots =>
        Capacity - OccupiedSlots;

    public bool StoreIngredient(Ingredientes ingredient)
    {
        if (playerInventory == null)
        {
            Debug.LogError("Falta asignar Player Inventory en FridgeStorage.");

            return false;
        }

        FridgeSlot freeSlot =GetFirstFreeSlot();

        if (freeSlot == null)
        {
            Debug.Log("La refrigeradora está llena.");

            return false;
        }

        IngredientVisualData visualData =
            GetVisualData(ingredient);

        if (visualData == null || visualData.visualPrefab == null)
        {
            Debug.LogWarning($"No existe un visual configurado para {ingredient}.");

            return false;
        }

        bool removedFromInventory =playerInventory.IntentarUsarIngrediente(ingredient,1);

        if (!removedFromInventory)
        {
            Debug.Log(
                $"No tienes {ingredient} en el inventario."
            );

            return false;
        }

        bool storedSuccessfully =freeSlot.StoreIngredient(ingredient,visualData.visualPrefab,visualData.localPosition, visualData.localRotation,visualData.localScale);

        if (!storedSuccessfully)
        {
            // Devolvemos el ingrediente para evitar perderlo.
            playerInventory.AñadirIngrediente(
                ingredient,
                1
            );

            return false;
        }

        OnStorageChanged?.Invoke();

        return true;
    }

    public bool TakeIngredient(Ingredientes ingredient)
    {
        if (playerInventory == null)
        {
            Debug.LogError(
                "Falta asignar Player Inventory en FridgeStorage."
            );

            return false;
        }

        FridgeSlot matchingSlot =
            GetSlotContaining(ingredient);

        if (matchingSlot == null)
        {
            Debug.Log(
                $"No hay {ingredient} dentro de la refrigeradora."
            );

            return false;
        }

        if (!matchingSlot.RemoveIngredient(
                out Ingredientes removedIngredient))
        {
            return false;
        }

        playerInventory.AñadirIngrediente(
            removedIngredient,
            1
        );

        OnStorageChanged?.Invoke();

        return true;
    }

    public bool TakeIngredientFromSlot(FridgeSlot slot)
    {
        if (slot == null ||
            !slot.IsOccupied)
        {
            return false;
        }

        if (playerInventory == null)
        {
            Debug.LogError(
                "Falta asignar Player Inventory en FridgeStorage."
            );

            return false;
        }

        if (!slot.RemoveIngredient(
                out Ingredientes removedIngredient))
        {
            return false;
        }

        playerInventory.AñadirIngrediente(
            removedIngredient,
            1
        );

        OnStorageChanged?.Invoke();

        return true;
    }

    public int GetStoredAmount(Ingredientes ingredient)
    {
        int amount = 0;

        foreach (FridgeSlot slot in slots)
        {
            if (slot != null &&
                slot.ContainsIngredient(ingredient))
            {
                amount++;
            }
        }

        return amount;
    }

    public bool ContainsIngredient(Ingredientes ingredient,int requiredAmount = 1)
    {
        return GetStoredAmount(ingredient) >=
               requiredAmount;
    }

    public Dictionary<Ingredientes, int>GetStoredIngredients()
    {
        Dictionary<Ingredientes, int> contents =new();

        foreach (FridgeSlot slot in slots)
        {
            if (slot == null ||
                !slot.IsOccupied)
            {
                continue;
            }

            Ingredientes ingredient =
                slot.StoredIngredient;

            if (contents.ContainsKey(ingredient))
            {
                contents[ingredient]++;
            }
            else
            {
                contents.Add(ingredient, 1);
            }
        }

        return contents;
    }

    private FridgeSlot GetFirstFreeSlot()
    {
        foreach (FridgeSlot slot in slots)
        {
            if (slot != null &&
                !slot.IsOccupied)
            {
                return slot;
            }
        }

        return null;
    }

    private FridgeSlot GetSlotContaining(
        Ingredientes ingredient)
    {
        foreach (FridgeSlot slot in slots)
        {
            if (slot != null &&
                slot.ContainsIngredient(ingredient))
            {
                return slot;
            }
        }

        return null;
    }

    private IngredientVisualData GetVisualData(
        Ingredientes ingredient)
    {
        foreach (
            IngredientVisualData visualData
            in ingredientVisuals)
        {
            if (visualData.ingredient.Equals(
                    ingredient))
            {
                return visualData;
            }
        }

        return null;
    }

    public bool StoreFirstAvailableIngredient()
    {
        if (playerInventory == null)
        {
            return false;
        }

        foreach (Ingredientes ingredient
                 in Enum.GetValues(typeof(Ingredientes)))
        {
            if (playerInventory.ObtenerCantidad(
                    ingredient) > 0)
            {
                return StoreIngredient(ingredient);
            }
        }

        Debug.Log(
            "El jugador no tiene ingredientes para guardar."
        );

        return false;
    }

    
}