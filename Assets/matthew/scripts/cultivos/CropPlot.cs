using System.Collections;
using UnityEngine;

public class CropPlot : MonoBehaviour, IInteractable
{
    [Header("Referencias")]
    [SerializeField] private Transform cropSpawnPoint;
    [SerializeField] private Inventory playerInventory;

    [Header("Etapas de crecimiento")]
    [SerializeField] private GameObject[] growthStagePrefabs;
    [SerializeField] private float timeBetweenStages = 5f;

    [Header("Semilla necesaria")]
    [SerializeField] private Ingredientes requiredSeed;
    [SerializeField] private int requiredSeedAmount = 1;

    [Header("Resultado de la cosecha")]
    [SerializeField] private Ingredientes harvestedIngredient;
    [SerializeField] private int harvestAmount = 1;

    private GameObject currentCropVisual;

    private bool isPlanted;
    private bool isGrowing;
    private bool isReady;

    public void Interact()
    {
        if (!isPlanted)
        {
            TryPlant();
            return;
        }

        if (isReady)
        {
            HarvestCrop();
            return;
        }

        if (isGrowing)
        {
            Debug.Log("La planta todavía está creciendo.");
        }
    }

    private void TryPlant()
    {
        if (playerInventory == null)
        {
            Debug.LogError(
                "Falta asignar el Inventory del jugador."
            );

            return;
        }

        if (cropSpawnPoint == null)
        {
            Debug.LogError(
                "Falta asignar Crop Spawn Point."
            );

            return;
        }

        if (growthStagePrefabs == null ||
            growthStagePrefabs.Length == 0)
        {
            Debug.LogError(
                "No hay etapas de crecimiento asignadas."
            );

            return;
        }

        bool usedSeed =
            playerInventory.IntentarUsarIngrediente(
                requiredSeed,
                requiredSeedAmount
            );

        if (!usedSeed)
        {
            Debug.Log(
                $"Necesitas {requiredSeedAmount} de " +
                $"{requiredSeed} para plantar."
            );

            return;
        }

        StartCoroutine(GrowCrop());
    }

    private IEnumerator GrowCrop()
    {
        isPlanted = true;
        isGrowing = true;
        isReady = false;

        for (
            int i = 0;
            i < growthStagePrefabs.Length;
            i++
        )
        {
            ShowGrowthStage(i);

            if (i < growthStagePrefabs.Length - 1)
            {
                yield return new WaitForSeconds(
                    timeBetweenStages
                );
            }
        }

        isGrowing = false;
        isReady = true;

        Debug.Log(
            $"{harvestedIngredient} está listo para cosechar."
        );
    }

    private void ShowGrowthStage(int stageIndex)
    {
        if (currentCropVisual != null)
        {
            Destroy(currentCropVisual);
        }

        GameObject stagePrefab =
            growthStagePrefabs[stageIndex];

        if (stagePrefab == null)
        {
            Debug.LogWarning(
                $"La etapa {stageIndex} no tiene prefab."
            );

            return;
        }

        currentCropVisual = Instantiate(
            stagePrefab,
            cropSpawnPoint.position,
            cropSpawnPoint.rotation,
            cropSpawnPoint
        );

        currentCropVisual.transform.localPosition =
            Vector3.zero;

        currentCropVisual.transform.localRotation =
            Quaternion.identity;
    }

    private void HarvestCrop()
    {
        if (!isReady)
            return;

        if (playerInventory == null)
        {
            Debug.LogError(
                "Falta asignar el Inventory del jugador."
            );

            return;
        }

        playerInventory.AñadirIngrediente(
            harvestedIngredient,
            harvestAmount
        );

        if (currentCropVisual != null)
        {
            Destroy(currentCropVisual);
            currentCropVisual = null;
        }

        isPlanted = false;
        isGrowing = false;
        isReady = false;

        Debug.Log(
            $"Cosechaste {harvestAmount} de " +
            $"{harvestedIngredient}."
        );
    }
}