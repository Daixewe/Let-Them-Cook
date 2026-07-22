using System.Collections;
using UnityEngine;

public class CropPlot : MonoBehaviour, IInteractable
{
    [Header("Lugar donde aparece la planta")]
    [SerializeField] private Transform cropSpawnPoint;

    [Header("Prefabs de crecimiento")]
    [SerializeField] private GameObject[] growthStagePrefabs;

    [Header("Objeto cosechado")]
    [SerializeField] private PickupItem harvestPrefab;

    [Header("Configuración")]
    [SerializeField] private float timeBetweenStages = 5f;

    private GameObject currentCropVisual;

    private bool isGrowing;
    private bool isReady;
    private bool isPlanted;

    public void Interact()
    {
        if (!isPlanted)
        {
            StartCoroutine(GrowCrop());
            return;
        }

        if (isReady)
        {
            HarvestCrop();
        }
    }

    private IEnumerator GrowCrop()
    {
        if (growthStagePrefabs == null || growthStagePrefabs.Length == 0)
        {
            Debug.LogWarning("No hay etapas de crecimiento asignadas.");
            yield break;
        }

        if (cropSpawnPoint == null)
        {
            Debug.LogWarning("No se asignó Crop Spawn Point.");
            yield break;
        }

        isPlanted = true;
        isGrowing = true;
        isReady = false;

        for (int i = 0; i < growthStagePrefabs.Length; i++)
        {
            ShowGrowthStage(i);

            if (i < growthStagePrefabs.Length - 1)
            {
                yield return new WaitForSeconds(timeBetweenStages);
            }
        }

        isGrowing = false;
        isReady = true;

        Debug.Log("El cultivo está listo para cosechar.");
    }

    private void ShowGrowthStage(int stageIndex)
    {
        if (currentCropVisual != null)
        {
            Destroy(currentCropVisual);
        }

        GameObject stagePrefab = growthStagePrefabs[stageIndex];

        if (stagePrefab == null)
        {
            Debug.LogWarning($"La etapa {stageIndex} está vacía.");
            return;
        }

        currentCropVisual = Instantiate(stagePrefab, cropSpawnPoint);

        currentCropVisual.transform.localPosition = Vector3.zero;
        currentCropVisual.transform.localRotation = Quaternion.identity;
    }

    private void HarvestCrop()
    {
        if (!isReady)
            return;

        PlayerPickup playerPickup =
            FindFirstObjectByType<PlayerPickup>();

        if (playerPickup == null)
        {
            Debug.LogWarning("No se encontró PlayerPickup.");
            return;
        }

        if (playerPickup.HasItem())
        {
            Debug.Log("Debes tener las manos vacías para cosechar.");
            return;
        }

        if (harvestPrefab == null)
        {
            Debug.LogWarning("No se asignó el Harvest Prefab.");
            return;
        }

        PickupItem harvestedItem = Instantiate(
            harvestPrefab,
            cropSpawnPoint.position,
            cropSpawnPoint.rotation
        );

        playerPickup.PickUp(harvestedItem);

        if (currentCropVisual != null)
        {
            Destroy(currentCropVisual);
            currentCropVisual = null;
        }

        isPlanted = false;
        isGrowing = false;
        isReady = false;

        Debug.Log("Cultivo cosechado. La parcela está disponible.");
    }
}