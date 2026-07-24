using UnityEngine;

public class FridgeInteraction : MonoBehaviour, IInteractable
{
    [Header("Puertas")]
    [SerializeField] private Transform leftDoorPivot;
    [SerializeField] private Transform rightDoorPivot;

    [Header("Rotación de puertas")]
    [SerializeField] private float leftOpenAngle = -100f;
    [SerializeField] private float rightOpenAngle = 100f;
    [SerializeField] private float rotationSpeed = 180f;

    [Header("Jugador")]
    [SerializeField] private PlayerPickup playerPickup;

    [Header("Almacenamiento")]
    [SerializeField] private FridgeStorage fridgeStorage;

    private Quaternion leftClosedRotation;
    private Quaternion rightClosedRotation;
    private Quaternion leftOpenRotation;
    private Quaternion rightOpenRotation;

    private bool isOpen;

    private void Awake()
    {
        if (leftDoorPivot != null)
        {
            leftClosedRotation = leftDoorPivot.localRotation;

            leftOpenRotation =
                leftClosedRotation *
                Quaternion.Euler(
                    0f,
                    leftOpenAngle,
                    0f
                );
        }

        if (rightDoorPivot != null)
        {
            rightClosedRotation = rightDoorPivot.localRotation;

            rightOpenRotation =
                rightClosedRotation *
                Quaternion.Euler(
                    0f,
                    rightOpenAngle,
                    0f
                );
        }
    }

    private void Update()
    {
        RotateDoors();
    }

    public void Interact()
    {
        if (!ValidateReferences())
        {
            return;
        }

        if (playerPickup.HasItem())
        {
            Debug.Log(
                "Debes tener la mano vacía para usar la refrigeradora."
            );

            return;
        }

        // Primera interacción: abre la refri.
        if (!isOpen)
        {
            OpenFridge();
            return;
        }

        // Segunda interacción:
        // guarda un ingrediente en el primer Slot disponible.
        TryStoreIngredient();
    }

    private void OpenFridge()
    {
        isOpen = true;

        Debug.Log("Refrigeradora abierta.");
    }

    private void TryStoreIngredient()
    {
        bool storedSuccessfully =
            fridgeStorage.StoreFirstAvailableIngredient();

        if (storedSuccessfully)
        {
            Debug.Log(
                "Ingrediente guardado en la refrigeradora."
            );
        }
        else
        {
            Debug.Log(
                "No se pudo guardar ningún ingrediente."
            );
        }
    }

    public void CloseFridge()
    {
        isOpen = false;

        Debug.Log("Refrigeradora cerrada.");
    }

    private void RotateDoors()
    {
        if (leftDoorPivot != null)
        {
            Quaternion targetLeft =
                isOpen
                    ? leftOpenRotation
                    : leftClosedRotation;

            leftDoorPivot.localRotation =
                Quaternion.RotateTowards(
                    leftDoorPivot.localRotation,
                    targetLeft,
                    rotationSpeed * Time.deltaTime
                );
        }

        if (rightDoorPivot != null)
        {
            Quaternion targetRight =
                isOpen
                    ? rightOpenRotation
                    : rightClosedRotation;

            rightDoorPivot.localRotation =
                Quaternion.RotateTowards(
                    rightDoorPivot.localRotation,
                    targetRight,
                    rotationSpeed * Time.deltaTime
                );
        }
    }

    private bool ValidateReferences()
    {
        if (playerPickup == null)
        {
            Debug.LogError(
                "Falta asignar PlayerPickup en FridgeInteraction."
            );

            return false;
        }

        if (fridgeStorage == null)
        {
            Debug.LogError(
                "Falta asignar FridgeStorage en FridgeInteraction."
            );

            return false;
        }

        return true;
    }
}