using System;
using System.Collections.Generic;
using UnityEngine;

public class FridgeUI : MonoBehaviour
{
    [Serializable]
    private class IngredientIconData
    {
        public Ingredientes ingredient;
        public Sprite icon;
    }

    [Header("Referencias principales")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private FridgeStorage fridgeStorage;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private MonoBehaviour playerInteractor;
    [SerializeField] private Player playerController;

    [Header("HUD del jugador")]
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject interactionText;

    [Header("Inventario del jugador")]
    [SerializeField] private Transform playerContentsParent;
    [SerializeField] private GameObject playerEmptyText;

    [Header("Contenido de la refrigeradora")]
    [SerializeField] private Transform fridgeContentsParent;
    [SerializeField] private GameObject fridgeEmptyText;

    [Header("Prefab")]
    [SerializeField] private FridgeUIItem itemPrefab;

    [Header("Iconos")]
    [SerializeField]
    private List<IngredientIconData> ingredientIcons =
        new();

    private readonly List<FridgeUIItem>
        generatedItems = new();

    public bool IsOpen
    {
        get
        {
            return uiPanel != null &&
                   uiPanel.activeSelf;
        }
    }

    private void Awake()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (fridgeStorage != null)
        {
            fridgeStorage.OnStorageChanged += Refresh;
        }

        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged += Refresh;
        }
    }

    private void OnDisable()
    {
        if (fridgeStorage != null)
        {
            fridgeStorage.OnStorageChanged -= Refresh;
        }

        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= Refresh;
        }
    }

    public void Open()
    {
        uiPanel.SetActive(true);

        if (playerInteractor != null)
        {
            playerInteractor.enabled = false;
        }

        if (playerController != null)
        {
            playerController.enabled = false;
        }
        if (crosshair != null)
        {
            crosshair.SetActive(false);
        }

        if (interactionText != null)
        {
            interactionText.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Refresh();
    }

    public void Close()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }

        if (playerInteractor != null)
        {
            playerInteractor.enabled = true;
        }

        if (playerController != null)
        {
            playerController.enabled = true;
        }
        if (crosshair != null)
        {
            crosshair.SetActive(true);
        }

        if (interactionText != null)
        {
            interactionText.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void StoreIngredient(
        Ingredientes ingredient)
    {
        if (fridgeStorage == null)
        {
            return;
        }

        bool storedSuccessfully =
            fridgeStorage.StoreIngredient(
                ingredient
            );

        if (storedSuccessfully)
        {
            Debug.Log(
                $"{ingredient} guardado en la refrigeradora."
            );
        }
    }

    public void TakeIngredient(
        Ingredientes ingredient)
    {
        if (fridgeStorage == null)
        {
            return;
        }

        bool takenSuccessfully =
            fridgeStorage.TakeIngredient(
                ingredient
            );

        if (takenSuccessfully)
        {
            Debug.Log(
                $"{ingredient} retirado de la refrigeradora."
            );
        }
    }

    public void Refresh()
    {
        if (!IsOpen)
        {
            return;
        }

        ClearGeneratedItems();

        GeneratePlayerInventory();
        GenerateFridgeInventory();
    }

    private void GeneratePlayerInventory()
    {
        Dictionary<Ingredientes, int> ingredients =
            playerInventory.GetIngredients();

        bool inventoryIsEmpty =
            ingredients == null ||
            ingredients.Count == 0;

        if (playerEmptyText != null)
        {
            playerEmptyText.SetActive(
                inventoryIsEmpty
            );
        }

        if (inventoryIsEmpty)
        {
            return;
        }

        foreach (
            KeyValuePair<Ingredientes, int> item
            in ingredients)
        {
            if (item.Value <= 0)
            {
                continue;
            }

            CreateUIItem(
                item.Key,
                item.Value,
                playerContentsParent,
                FridgeUIItem.ItemAction.Store
            );
        }
    }

    private void GenerateFridgeInventory()
    {
        Dictionary<Ingredientes, int> ingredients =
            fridgeStorage.GetStoredIngredients();

        bool fridgeIsEmpty =
            ingredients == null ||
            ingredients.Count == 0;

        if (fridgeEmptyText != null)
        {
            fridgeEmptyText.SetActive(
                fridgeIsEmpty
            );
        }

        if (fridgeIsEmpty)
        {
            return;
        }

        foreach (
            KeyValuePair<Ingredientes, int> item
            in ingredients)
        {
            CreateUIItem(
                item.Key,
                item.Value,
                fridgeContentsParent,
                FridgeUIItem.ItemAction.Take
            );
        }
    }

    private void CreateUIItem(
        Ingredientes ingredient,
        int amount,
        Transform parent,
        FridgeUIItem.ItemAction action)
    {
        if (parent == null ||
            itemPrefab == null)
        {
            return;
        }

        FridgeUIItem newItem =
            Instantiate(
                itemPrefab,
                parent
            );

        newItem.Configure(
            ingredient,
            GetIngredientIcon(ingredient),
            amount,
            action,
            this
        );

        generatedItems.Add(newItem);
    }

    private Sprite GetIngredientIcon(
        Ingredientes ingredient)
    {
        foreach (
            IngredientIconData iconData
            in ingredientIcons)
        {
            if (iconData.ingredient ==
                ingredient)
            {
                return iconData.icon;
            }
        }

        return null;
    }

    private void ClearGeneratedItems()
    {
        foreach (
            FridgeUIItem generatedItem
            in generatedItems)
        {
            if (generatedItem != null)
            {
                Destroy(
                    generatedItem.gameObject
                );
            }
        }

        generatedItems.Clear();
    }

    private bool ValidateReferences()
    {
        if (uiPanel == null)
        {
            Debug.LogError(
                "Falta asignar UI Panel en FridgeUI."
            );

            return false;
        }

        if (fridgeStorage == null)
        {
            Debug.LogError(
                "Falta asignar Fridge Storage en FridgeUI."
            );

            return false;
        }

        if (playerInventory == null)
        {
            Debug.LogError(
                "Falta asignar Player Inventory en FridgeUI."
            );

            return false;
        }

        if (playerContentsParent == null)
        {
            Debug.LogError(
                "Falta asignar Player Contents Parent."
            );

            return false;
        }

        if (fridgeContentsParent == null)
        {
            Debug.LogError(
                "Falta asignar Fridge Contents Parent."
            );

            return false;
        }

        if (itemPrefab == null)
        {
            Debug.LogError(
                "Falta asignar Item Prefab."
            );

            return false;
        }

        return true;
    }
}