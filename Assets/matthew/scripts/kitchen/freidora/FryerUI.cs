using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FryerUI : MonoBehaviour
{
    [Header("Panel principal")]
    [SerializeField] private GameObject panel;

    [Header("Botón de cerrar")]
    [SerializeField] private Button closeButton;

    [Header("Lista de ingredientes")]
    [SerializeField] private Transform ingredientsContent;
    [SerializeField] private GrillIngredientUIItem ingredientItemPrefab;

    [Header("Lista de espacios")]
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private GrillSlotUIItem slotItemPrefab;

    [Header("Bloqueo del jugador")]
    [SerializeField] private MonoBehaviour cameraLookScript;
    [SerializeField] private MonoBehaviour interactorScript;
    [SerializeField] private GameObject crosshairObject;
    [SerializeField] private GameObject interactionTextObject;

    private FryerStation currentFryer;
    private Inventory playerInventory;

    private readonly List<GrillIngredientUIItem>
        ingredientItems = new();

    private readonly List<GrillSlotUIItem>
        slotItems = new();

    public bool IsOpen =>
        panel != null &&
        panel.activeSelf;

    private void Awake()
    {
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

    public void Open(FryerStation fryer,Inventory inventory)
    {
        if (panel == null ||fryer == null ||inventory == null)
        {
            Debug.LogError("Faltan referencias para abrir FryerUI.");

            return;
        }

        UnsubscribeFromInventory();

        currentFryer = fryer;
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

        currentFryer = null;
        playerInventory = null;

        SetPlayerInteractionEnabled(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RefreshUI()
    {
        if (currentFryer == null ||playerInventory == null)
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
            if (pair.Value <= 0)
            {
                continue;
            }

            if (!currentFryer.CanCookIngredient(pair.Key))
            {
                continue;
            }

            GrillIngredientUIItem newItem =Instantiate(ingredientItemPrefab,ingredientsContent,false);

            newItem.Setup(pair.Key,pair.Value,HandleCookPressed);

            newItem.SetCookButtonInteractable(currentFryer.HasFreeSlot());

            ingredientItems.Add(newItem);
        }
    }

    private void RefreshCookButtons()
    {
        if (currentFryer == null)
        {
            return;
        }

        bool hasFreeSlot =currentFryer.HasFreeSlot();

        foreach (GrillIngredientUIItem item in ingredientItems)
        {
            if (item != null)
            {
                item.SetCookButtonInteractable(hasFreeSlot);
            }
        }
    }

    private void RefreshSlots()
    {
        if (currentFryer == null ||slotItemPrefab == null ||slotsContainer == null)
        {
            return;
        }

        GrillCookingSlot[] cookingSlots =currentFryer.GetCookingSlots();

        if (cookingSlots == null)
        {
            return;
        }

        EnsureSlotItems(cookingSlots.Length);

        for (int i = 0;i < cookingSlots.Length;i++)
        {
            GrillCookingSlot slot =cookingSlots[i];

            GrillSlotUIItem slotUI =slotItems[i];

            if (slot == null ||slot.IsEmpty)
            {
                slotUI.ShowEmpty();
                continue;
            }

            if (slot.IsReady)
            {
                slotUI.ShowReady(slot.CookedIngredient);

                continue;
            }

            slotUI.ShowCooking(slot.CurrentIngredient,slot.NormalizedProgress,slot.RemainingTime);
        }
    }

    private void EnsureSlotItems(int requiredAmount)
    {
        while (slotItems.Count <requiredAmount)
        {
            int slotIndex =slotItems.Count;

            GrillSlotUIItem newItem =Instantiate(slotItemPrefab,slotsContainer,false);

            newItem.Setup(slotIndex,HandleCollectPressed);

            slotItems.Add(newItem);
        }

        for (int i = 0; i < slotItems.Count;i++)
        {
            slotItems[i].gameObject.SetActive(i < requiredAmount);
        }
    }

    private void HandleCookPressed(
        Ingredientes ingredient)
    {
        if (currentFryer == null)
        {
            return;
        }

        if (currentFryer.TryCookIngredient(
                ingredient))
        {
            RefreshUI();
        }
    }

    private void HandleCollectPressed(
        int slotIndex)
    {
        if (currentFryer == null)
        {
            return;
        }

        if (currentFryer.TryCollectIngredient(
                slotIndex))
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
            playerInventory.OnInventoryChanged += RefreshIngredients;
        }
    }

    private void UnsubscribeFromInventory()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= RefreshIngredients;
        }
    }

    private void SetPlayerInteractionEnabled(
        bool enabled)
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
