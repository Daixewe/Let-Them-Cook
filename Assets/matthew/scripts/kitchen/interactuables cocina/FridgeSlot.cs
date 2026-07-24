using UnityEngine;

public class FridgeSlot : MonoBehaviour
{
    [Header("Punto donde aparece el ingrediente")]
    [SerializeField] private Transform visualPoint;

    private Ingredientes storedIngredient;
    private GameObject currentVisual;
    private bool isOccupied;

    public bool IsOccupied => isOccupied;

    public Ingredientes StoredIngredient =>
        storedIngredient;

    private void Awake()
    {
        if (visualPoint == null)
        {
            visualPoint = transform;
        }
    }

    public bool StoreIngredient(
        Ingredientes ingredient,
        GameObject visualPrefab,
        Vector3 localPosition,
        Vector3 localRotation,
        Vector3 localScale)
    {
        if (isOccupied)
        {
            return false;
        }

        if (visualPrefab == null)
        {
            Debug.LogWarning(
                $"No existe un prefab visual para {ingredient}.",
                this
            );

            return false;
        }

        storedIngredient = ingredient;
        isOccupied = true;

        currentVisual = Instantiate(
            visualPrefab,
            visualPoint
        );

        currentVisual.transform.localPosition =
            localPosition;

        currentVisual.transform.localRotation =
            Quaternion.Euler(localRotation);

        currentVisual.transform.localScale =
            localScale;

        DisablePhysics(currentVisual);

        return true;
    }

    public bool RemoveIngredient(
        out Ingredientes ingredient)
    {
        if (!isOccupied)
        {
            ingredient = default;
            return false;
        }

        ingredient = storedIngredient;

        ClearSlot();

        return true;
    }

    public bool ContainsIngredient(
        Ingredientes ingredient)
    {
        return isOccupied &&
               storedIngredient.Equals(ingredient);
    }

    public void ClearSlot()
    {
        if (currentVisual != null)
        {
            Destroy(currentVisual);
        }

        currentVisual = null;
        storedIngredient = default;
        isOccupied = false;
    }

    private void DisablePhysics(
        GameObject visualObject)
    {
        Rigidbody[] rigidbodies =
            visualObject.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbodyComponent
                 in rigidbodies)
        {
            rigidbodyComponent.isKinematic = true;
            rigidbodyComponent.useGravity = false;
        }

        Collider[] colliders =
            visualObject.GetComponentsInChildren<Collider>();

        foreach (Collider colliderComponent
                 in colliders)
        {
            colliderComponent.enabled = false;
        }

        MonoBehaviour[] behaviours =
            visualObject.GetComponentsInChildren<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is PickupItem ||
                behaviour is EggItem)
            {
                behaviour.enabled = false;
            }
        }
    }
}