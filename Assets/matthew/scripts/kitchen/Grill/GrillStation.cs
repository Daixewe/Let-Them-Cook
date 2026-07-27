using UnityEngine;

public class GrillStation : MonoBehaviour, IInteractable
{
    [Header("Referencias")]
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private GrillUI grillUI;

    [Header("Espacios de cocción")]
    [SerializeField] private GrillCookingSlot[] cookingSlots;

    public void Interact()
    {
        if (!ValidateReferences())
        {
            return;
        }

        if (grillUI.IsOpen)
        {
            return;
        }

        grillUI.Open(this, playerInventory);
    }

    public bool CanCookIngredient(Ingredientes ingredient)
    {
        return TryGetCookedIngredient(ingredient,out _);
    }

    public bool TryCookIngredient(Ingredientes rawIngredient)
    {
        if (playerInventory == null)
        {
            Debug.LogError("Falta asignar Player Inventory " +"en GrillStation.");

            return false;
        }

        if (!TryGetCookedIngredient(rawIngredient,out Ingredientes cookedIngredient))
        {
            Debug.LogWarning($"{rawIngredient} no puede cocinarse " +"en la plancha.");

            return false;
        }

        GrillCookingSlot freeSlot = GetFreeSlot();

        if (freeSlot == null)
        {
            Debug.LogWarning("No hay espacios libres en la plancha.");

            return false;
        }

        bool ingredientConsumed =playerInventory.IntentarUsarIngrediente(rawIngredient,1);

        if (!ingredientConsumed)
        {
            return false;
        }

        bool ingredientPlaced =freeSlot.PlaceIngredient(rawIngredient,cookedIngredient );

        if (!ingredientPlaced)
        {
            playerInventory.AñadirIngrediente( rawIngredient,1);

            return false;
        }

        return true;
    }

    public bool TryCollectIngredient(int slotIndex)
    {
        if (cookingSlots == null)
        {
            return false;
        }

        if (slotIndex < 0 ||slotIndex >= cookingSlots.Length)
        {
            Debug.LogError($"El índice {slotIndex} no existe " +"en los Cooking Slots.");

            return false;
        }

        return TryCollectIngredient(
            cookingSlots[slotIndex]
        );
    }

    public bool TryCollectIngredient(
        GrillCookingSlot slot)
    {
        if (slot == null)
        {
            return false;
        }

        bool collected =
            slot.TryCollectIngredient(
                out Ingredientes cookedIngredient
            );

        if (!collected)
        {
            return false;
        }

        playerInventory.AñadirIngrediente(
            cookedIngredient,
            1
        );

        return true;
    }

    public GrillCookingSlot[] GetCookingSlots()
    {
        return cookingSlots;
    }

    public bool HasFreeSlot()
    {
        return GetFreeSlot() != null;
    }

    private GrillCookingSlot GetFreeSlot()
    {
        if (cookingSlots == null)
        {
            return null;
        }

        foreach (GrillCookingSlot slot in cookingSlots)
        {
            if (slot != null && !slot.IsOccupied)
            {
                return slot;
            }
        }

        return null;
    }

    private bool TryGetCookedIngredient(Ingredientes rawIngredient,out Ingredientes cookedIngredient)
    {
        cookedIngredient = default;

        switch (rawIngredient)
        {
            case Ingredientes.CarneCruda:
                cookedIngredient = Ingredientes.Carne;
                return true;

            case Ingredientes.PapasCortadas:
                cookedIngredient =
                    Ingredientes.PapasCocinadas;
                return true;

            case Ingredientes.huevo:
                cookedIngredient =
                    Ingredientes.huevoCocinado;
                return true;

            case Ingredientes.PlatanoVerdeCortado:
                cookedIngredient =
                    Ingredientes.PlatanoVerdeCocinado;
                return true;

            default:
                return false;
        }
    }

    private bool ValidateReferences()
    {
        if (playerInventory == null)
        {
            Debug.LogError("Falta asignar Player Inventory " + "en GrillStation.");

            return false;
        }

        if (grillUI == null)
        {
            Debug.LogError("Falta asignar Grill UI " +"en GrillStation.");

            return false;
        }

        if (cookingSlots == null ||cookingSlots.Length == 0)
        {
            Debug.LogError("No hay Cooking Slots asignados " +"en GrillStation.");

            return false;
        }

        return true;
    }
}