using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrillUI : MonoBehaviour
{
    [Header("Panel principal")]
    [SerializeField] private GameObject panel;

    [Header("Botón de cerrar")]
    [SerializeField] private Button closeButton;

    [Header("Lista de ingredientes")]
    [SerializeField] private Transform ingredientsContent;

    [SerializeField]
    private GrillIngredientUIItem ingredientItemPrefab;

    [Header("Lista de espacios")]
    [SerializeField] private Transform slotsContainer;

    [Header("Bloqueo del jugador")]
    [SerializeField] private MonoBehaviour cameraLookScript;
    [SerializeField] private MonoBehaviour interactorScript;
    [SerializeField] private GameObject crosshairObject;
    [SerializeField] private GameObject interactionTextObject;

    [SerializeField]
    private GrillSlotUIItem slotItemPrefab;

    private GrillStation currentGrill;
    private Inventory playerInventory;

    private readonly List<GrillIngredientUIItem>
        ingredientItems = new();

    private readonly List<GrillSlotUIItem>
        slotItems = new();

    public bool IsOpen
    {
        get
        {
            return panel != null && panel.activeSelf;
        }
    }

    private void Awake()
    {
        ValidateReferences();

        if (panel != null)
        {
            panel.SetActive(false);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }
    }

    private void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(Close);
        }

        UnsubscribeFromInventory();
    }

    private void Update()
    {
        if (!IsOpen)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
            return;
        }

        RefreshSlots();
        RefreshCookButtons();
    }

    public void Open(GrillStation grill,Inventory inventory)
    {
        if (panel == null)
        {
            Debug.LogError("Falta asignar el Panel en GrillUI.");

            return;
        }

        if (grill == null)
        {
            Debug.LogError("GrillStation recibido es null.");

            return;
        }

        if (inventory == null)
        {
            Debug.LogError("Inventory recibido es null.");

            return;
        }

        UnsubscribeFromInventory();

        currentGrill = grill;
        playerInventory = inventory;

        SubscribeToInventory();

        panel.SetActive(true);

        SetPlayerInteractionEnabled(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        RefreshUI();
    }

    public void Close()
    {
        UnsubscribeFromInventory();

        if (panel != null)
        {
            panel.SetActive(false);
        }

        currentGrill = null;
        playerInventory = null;

        SetPlayerInteractionEnabled(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RefreshUI()
    {
        if (currentGrill == null ||playerInventory == null)
        {
            return;
        }

        RefreshIngredients();
        RefreshSlots();
    }

    private void RefreshIngredients()
    {
        if (ingredientItemPrefab == null ||ingredientsContent == null)
        {
            return;
        }

        ClearIngredientItems();

        Dictionary<Ingredientes, int> ingredients =playerInventory.GetIngredients();

        foreach (KeyValuePair<Ingredientes, int> pair in ingredients)
        {
            Ingredientes ingredient = pair.Key;
            int quantity = pair.Value;

            

            if (quantity <= 0)
            {
                continue;
            }

            if (!currentGrill.CanCookIngredient(ingredient))
            {
                continue;
            }

            GrillIngredientUIItem newItem =Instantiate(ingredientItemPrefab,ingredientsContent);

            newItem.Setup(ingredient,quantity,HandleCookPressed);

            newItem.SetCookButtonInteractable( currentGrill.HasFreeSlot());

            ingredientItems.Add(newItem);
        }
    }

    private void RefreshCookButtons()
    {
        if (currentGrill == null)
        {
            return;
        }

        bool hasFreeSlot =currentGrill.HasFreeSlot();

        foreach (GrillIngredientUIItem item in ingredientItems)
        {
            if (item != null)
            {
                item.SetCookButtonInteractable( hasFreeSlot);
            }
        }
    }

    private void RefreshSlots()
    {
        if (currentGrill == null || slotItemPrefab == null || slotsContainer == null)
        {
            return;
        }

        GrillCookingSlot[] cookingSlots =currentGrill.GetCookingSlots();

        if (cookingSlots == null)
        {
            return;
        }

        EnsureSlotItems(cookingSlots.Length);

        for (int i = 0;i < cookingSlots.Length;i++)
        {
            GrillCookingSlot slot =cookingSlots[i];

            GrillSlotUIItem slotUI =slotItems[i];

            if (slot == null || slot.IsEmpty)
            {
                slotUI.ShowEmpty();
                continue;
            }

            if (slot.IsReady)
            {
                slotUI.ShowReady(slot.CookedIngredient);

                continue;
            }

            slotUI.ShowCooking( slot.CurrentIngredient,slot.NormalizedProgress, slot.RemainingTime);
        }
    }

    private void EnsureSlotItems(int requiredAmount)
    {
        while (slotItems.Count < requiredAmount)
        {
            int slotIndex = slotItems.Count;

            GrillSlotUIItem newItem =Instantiate(slotItemPrefab,slotsContainer);

            newItem.Setup(slotIndex,HandleCollectPressed);

            slotItems.Add(newItem);
        }

        for (int i = 0; i < slotItems.Count; i++)
        {
            slotItems[i].gameObject.SetActive( i < requiredAmount);
        }
    }

    private void HandleCookPressed(Ingredientes ingredient)
    {
        if (currentGrill == null)
        {
            return;
        }

        bool cooked =currentGrill.TryCookIngredient(ingredient);

        if (cooked)
        {
            RefreshUI();
        }
    }

    private void HandleCollectPressed(int slotIndex)
    {
        if (currentGrill == null)
        {
            return;
        }

        bool collected =currentGrill.TryCollectIngredient(slotIndex);

        if (collected)
        {
            RefreshUI();
        }
    }

    private void ClearIngredientItems()
    {
        foreach (
            GrillIngredientUIItem item
            in ingredientItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }

        ingredientItems.Clear();
    }

    private void SubscribeToInventory()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged +=RefreshIngredients;
        }
    }

    private void UnsubscribeFromInventory()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -=RefreshIngredients;
        }
    }

    private void ValidateReferences()
    {
        if (panel == null)
        {
            Debug.LogError("Falta asignar Panel en GrillUI." );
        }

        if (closeButton == null)
        {
            Debug.LogError("Falta asignar Close Button en GrillUI.");
        }

        if (ingredientsContent == null)
        {
            Debug.LogError("Falta asignar Ingredients Content " +"en GrillUI.");
        }

        if (ingredientItemPrefab == null)
        {
            Debug.LogError("Falta asignar Ingredient Item Prefab " +"en GrillUI.");
        }

        if (slotsContainer == null)
        {
            Debug.LogError("Falta asignar Slots Container " +"en GrillUI.");
        }

        if (slotItemPrefab == null)
        {
            Debug.LogError("Falta asignar Slot Item Prefab " + "en GrillUI.");
        }
    }
    private void SetPlayerInteractionEnabled(bool enabled)
    {
        if (cameraLookScript != null)
        {
            cameraLookScript.enabled = enabled;
        }

        if (interactorScript != null)
        {
            interactorScript.enabled = enabled;
        }

        if (crosshairObject != null)
        {
            crosshairObject.SetActive(enabled);
        }

        if (interactionTextObject != null)
        {
            interactionTextObject.SetActive(enabled);
        }
    }
}