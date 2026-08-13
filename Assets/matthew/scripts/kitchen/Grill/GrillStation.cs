using UnityEngine;

public class GrillStation : MonoBehaviour, IInteractable
{
    [Header("Referencias")]
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private GrillUI grillUI;

    [Header("Espacios de cocción")]
    [SerializeField] private GrillCookingSlot[] cookingSlots;

    [Header("Audio")]
    [SerializeField] private AudioSource cookingAudioSource;
    [SerializeField] private AudioClip cookingSound;


    private void Update()
    {
        UpdateCookingSound();
    }

    private void UpdateCookingSound()
    {
        if (cookingAudioSource == null ||cookingSound == null)
        {
            return;
        }

        bool shouldPlay =IsAnythingCooking();

        if (shouldPlay)
        {
            if (!cookingAudioSource.isPlaying)
            {
                cookingAudioSource.clip =cookingSound;

                cookingAudioSource.loop = true;

                cookingAudioSource.Play();
            }
        }
        else
        {
            if (cookingAudioSource.isPlaying)
            {
                cookingAudioSource.Stop();
            }
        }
    }

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

        grillUI.Open(this,playerInventory);
    }

    public string GetInteractionText()
    {
        return "Usar plancha";
    }

    public bool CanCookIngredient(Ingredientes ingredient)
    {
        return TryGetCookedIngredient(ingredient,out _);
    }

    public bool TryCookIngredient(Ingredientes rawIngredient)
    {
        if (playerInventory == null)
        {
            Debug.LogError("Falta asignar Player Inventory en GrillStation.");

            return false;
        }

        if (!TryGetCookedIngredient(rawIngredient,out Ingredientes cookedIngredient))
        {
            Debug.LogWarning($"{rawIngredient} no puede cocinarse en la plancha.");

            return false;
        }

        GrillCookingSlot freeSlot =GetFreeSlot();

        if (freeSlot == null)
        {
            NotificationUI.Instance?.ShowMessage("No hay espacios libres en la plancha.");

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
            Debug.LogError($"El índice {slotIndex} no existe en los Cooking Slots.");

            return false;
        }

        return TryCollectIngredient(cookingSlots[slotIndex]);
    }

    public bool TryCollectIngredient(GrillCookingSlot slot)
    {
        if (slot == null)
        {
            return false;
        }

        
        // comprobamos el espacio antes de retirar la comida.
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
            NotificationUI.Instance?.ShowMessage("No se pudo recoger la comida.");

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
                cookedIngredient =Ingredientes.Carne;
                return true;

            case Ingredientes.huevo:
                cookedIngredient =Ingredientes.huevoCocinado;
                return true;

            default:
                return false;
        }
    }

    private bool ValidateReferences()
    {
        if (playerInventory == null)
        {
            Debug.LogError("Falta asignar Player Inventory en GrillStation.");

            return false;
        }

        if (grillUI == null)
        {
            Debug.LogError("Falta asignar Grill UI en GrillStation.");

            return false;
        }

        if (cookingSlots == null ||cookingSlots.Length == 0)
        {
            Debug.LogError("No hay Cooking Slots asignados en GrillStation.");

            return false;
        }

        return true;
    }

    private bool IsAnythingCooking()
    {
        if (cookingSlots == null)
        {
            return false;
        }

        foreach (GrillCookingSlot slot in cookingSlots)
        {
            if (slot != null &&!slot.IsEmpty &&!slot.IsReady)
            {
                return true;
            }
        }

        return false;
    }
}