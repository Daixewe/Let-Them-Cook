using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreparationUI : MonoBehaviour
{
    [Header("Panel principal")]
    [SerializeField] private GameObject panel;

    [Header("Lista de recetas")]
    [SerializeField] private Transform recipeContent;
    [SerializeField] private PreparationRecipeUIItem recipeItemPrefab;

    [Header("Botón para cerrar")]
    [SerializeField] private Button closeButton;

    [Header("Controles del jugador")]
    [SerializeField] private MonoBehaviour cameraLookScript;
    [SerializeField] private MonoBehaviour interactorScript;
    [SerializeField] private GameObject crosshairObject;
    [SerializeField] private GameObject interactionTextObject;

    private PreparationStation currentStation;

    private readonly List<PreparationRecipeUIItem> createdItems = new();

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    private void Update()
    {
        if (panel == null || !panel.activeSelf)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    public void Open(PreparationStation station)
    {
        if (station == null)
        {
            Debug.LogError("PreparationUI recibió una estación nula.");
            return;
        }

        if (panel == null)
        {
            Debug.LogError("PreparationUI no tiene asignado el Panel.");
            return;
        }

        if (panel.activeSelf)
        {
            return;
        }

        currentStation = station;

        panel.SetActive(true);

        SetPlayerInteractionEnabled(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        RefreshRecipes();
    }

    public void Close()
    {
        currentStation = null;

        if (panel != null)
        {
            panel.SetActive(false);
        }

        SetPlayerInteractionEnabled(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RefreshRecipes()
    {
        ClearRecipeItems();

        if (currentStation == null)
        {
            Debug.LogError("No hay una estación de preparación abierta.");
            return;
        }

        if (recipeItemPrefab == null)
        {
            Debug.LogError("Recipe Item Prefab no está asignado.");
            return;
        }

        if (recipeContent == null)
        {
            Debug.LogError("Recipe Content no está asignado.");
            return;
        }

        IReadOnlyList<RecipeData> recipes = currentStation.GetAvailableRecipes();

        foreach (RecipeData recipe in recipes)
        {
            if (recipe == null)
            {
                continue;
            }

            PreparationRecipeUIItem newItem =
                Instantiate(recipeItemPrefab, recipeContent, false);

            newItem.gameObject.SetActive(true);
            newItem.transform.localScale = Vector3.one;

            bool canPrepare = currentStation.CanPrepareRecipe(recipe);

            newItem.Configure(
                recipe,
                canPrepare,
                PrepareRecipe
            );

            createdItems.Add(newItem);
        }
    }

    private void PrepareRecipe(RecipeData recipe)
    {
        if (currentStation == null)
        {
            Debug.LogError("No hay una mesa de preparación abierta.");
            return;
        }

        bool prepared = currentStation.TryPrepareRecipe(recipe);

        if (!prepared)
        {
            Debug.LogWarning("No se pudo preparar la receta.");
        }

        RefreshRecipes();
    }

    private void ClearRecipeItems()
    {
        foreach (PreparationRecipeUIItem item in createdItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }

        createdItems.Clear();
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