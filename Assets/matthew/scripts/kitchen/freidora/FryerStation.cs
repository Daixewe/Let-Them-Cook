using UnityEngine;

public class FryerStation : MonoBehaviour, IInteractable
{
    [Header("Referencias")]
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private FryerUI fryerUI;

    [Header("Espacios de cocción")]
    [SerializeField] private GrillCookingSlot[] cookingSlots;

    public void Interact()
    {
        if (!ValidateReferences())
        {
            return;
        }

        if (fryerUI.IsOpen)
        {
            return;
        }

        fryerUI.Open(this, playerInventory);
    }

    public string GetInteractionText()
    {
        return "Usar freidora";
    }

    public bool CanCookIngredient(Ingredientes ingredient)
    {
        return TryGetCookedIngredient(ingredient,out _);
    }

    public bool TryCookIngredient(
        Ingredientes rawIngredient)
    {
        if (playerInventory == null)
        {
            Debug.LogError("Falta asignar Player Inventory en FryerStation.");

            return false;
        }

        if (!TryGetCookedIngredient(rawIngredient,out Ingredientes cookedIngredient))
        {
            Debug.LogWarning($"{rawIngredient} no puede cocinarse en la freidora.");

            return false;
        }

        GrillCookingSlot freeSlot = GetFreeSlot();

        if (freeSlot == null)
        {
            Debug.LogWarning("No hay espacios libres en la freidora.");

            return false;
        }

        bool ingredientConsumed =playerInventory.IntentarUsarIngrediente(rawIngredient,1);

        if (!ingredientConsumed)
        {
            return false;
        }

        bool ingredientPlaced =freeSlot.PlaceIngredient(rawIngredient,cookedIngredient);

        if (!ingredientPlaced)
        {
            playerInventory.AñadirIngrediente(rawIngredient,1);

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
            Debug.LogError($"El índice {slotIndex} no existe en los espacios de la freidora.");

            return false;
        }

        GrillCookingSlot slot =cookingSlots[slotIndex];

        if (slot == null)
        {
            return false;
        }

        if (!playerInventory.HasSpace(1))
        {
            NotificationUI.Instance?.ShowMessage("Inventario lleno. No puedes recoger la comida.");

            return false;
        }

        bool collected =slot.TryCollectIngredient(out Ingredientes cookedIngredient);

        if (!collected)
        {
            return false;
        }

        bool added =playerInventory.AñadirIngrediente(cookedIngredient,1);

        if (!added)
        {
            Debug.LogWarning("No se pudo recoger el alimento porque el inventario está lleno.");

            return false;
        }

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
            if (slot != null &&
                !slot.IsOccupied)
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
            case Ingredientes.PapasCortadas:
                cookedIngredient =
                    Ingredientes.PapasCocinadas;
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
            Debug.LogError("Falta asignar Player Inventory en FryerStation.");

            return false;
        }

        if (fryerUI == null)
        {
            Debug.LogError("Falta asignar Fryer UI en FryerStation.");

            return false;
        }

        if (cookingSlots == null ||cookingSlots.Length == 0)
        {
            Debug.LogError("No hay espacios asignados en FryerStation.");

            return false;
        }

        return true;
    }
}