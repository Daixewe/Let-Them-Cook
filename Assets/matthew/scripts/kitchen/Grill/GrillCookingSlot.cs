using System.Collections;
using UnityEngine;

public class GrillCookingSlot : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField]
    private StationIngredientVisual ingredientVisual;

    [Header("Configuración de cocción")]
    [SerializeField]
    private float cookingTime = 4f;

    [Header("Efecto opcional")]
    [SerializeField]
    private GameObject cookingEffect;

    private Ingredientes currentIngredient;
    private Ingredientes resultingIngredient;

    private bool hasIngredient;
    private bool isCooking;
    private bool isCooked;

    private float elapsedCookingTime;

    public bool IsOccupied
    {
        get { return hasIngredient; }
    }

    public bool IsEmpty
    {
        get { return !hasIngredient; }
    }

    public bool IsCurrentlyCooking
    {
        get { return isCooking; }
    }

    public bool IsCooked
    {
        get { return isCooked; }
    }

    public bool IsReady
    {
        get
        {
            return hasIngredient && isCooked;
        }
    }

    public Ingredientes CurrentIngredient
    {
        get { return currentIngredient; }
    }

    public Ingredientes CookedIngredient
    {
        get { return resultingIngredient; }
    }

    public float NormalizedProgress
    {
        get
        {
            if (cookingTime <= 0f)
            {
                return 0f;
            }

            return Mathf.Clamp01(elapsedCookingTime / cookingTime);
        }
    }

    public float RemainingTime
    {
        get
        {
            return Mathf.Max(0f,cookingTime - elapsedCookingTime);
        }
    }

    private void Awake()
    {
        SetCookingEffect(false);
    }

    public bool PlaceIngredient(Ingredientes rawIngredient,Ingredientes cookedIngredient)
    {
        if (!ValidateReferences())
        {
            return false;
        }

        if (hasIngredient)
        {
            Debug.LogWarning($"{gameObject.name} ya tiene un ingrediente.");

            return false;
        }

        bool visualShown =ingredientVisual.ShowIngredient(rawIngredient);

        if (!visualShown)
        {
            Debug.LogError($"No se pudo mostrar el visual de " +$"{rawIngredient} en {gameObject.name}.");

            ResetSlot();
            return false;
        }

        currentIngredient = rawIngredient;
        resultingIngredient = cookedIngredient;

        hasIngredient = true;
        isCooking = false;
        isCooked = false;

        elapsedCookingTime = 0f;

        StartCoroutine(CookIngredient());

        return true;
    }

    private IEnumerator CookIngredient()
    {
        isCooking = true;
        SetCookingEffect(true);

        while (elapsedCookingTime < cookingTime)
        {
            elapsedCookingTime += Time.deltaTime;

            yield return null;
        }

        elapsedCookingTime = cookingTime;

        bool resultVisualShown =ingredientVisual.ShowIngredient(resultingIngredient);

        if (!resultVisualShown)
        {
            Debug.LogError($"No existe un visual configurado para " +$"{resultingIngredient}.");

            isCooking = false;
            SetCookingEffect(false);

            yield break;
        }

        isCooking = false;
        isCooked = true;

        SetCookingEffect(false);
    }

    public bool TryCollectIngredient(
        out Ingredientes collectedIngredient)
    {
        collectedIngredient = default;

        if (!hasIngredient)
        {
            Debug.LogWarning($"{gameObject.name} está vacío.");

            return false;
        }

        if (isCooking)
        {
            Debug.LogWarning($"El ingrediente de {gameObject.name} " +$"todavía se cocina.");

            return false;
        }

        if (!isCooked)
        {
            Debug.LogWarning($"El ingrediente de {gameObject.name} " +$"aún no está listo.");

            return false;
        }

        collectedIngredient = resultingIngredient;

        ingredientVisual.ClearVisual();

        ResetSlot();

        return true;
    }

    private void ResetSlot()
    {
        StopAllCoroutines();

        currentIngredient = default;
        resultingIngredient = default;

        hasIngredient = false;
        isCooking = false;
        isCooked = false;

        elapsedCookingTime = 0f;

        SetCookingEffect(false);
    }

    private void SetCookingEffect(bool active)
    {
        if (cookingEffect != null)
        {
            cookingEffect.SetActive(active);
        }
    }

    private bool ValidateReferences()
    {
        if (ingredientVisual == null)
        {
            Debug.LogError($"Falta asignar StationIngredientVisual " +$"en {gameObject.name}.");

            return false;
        }

        if (cookingTime <= 0f)
        {
            Debug.LogError($"Cooking Time debe ser mayor que cero " +$"en {gameObject.name}.");

            return false;
        }

        return true;
    }
}