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

    [Header("Almacenamiento")]
    [SerializeField] private Transform eggPlacementPoint;

    private Quaternion leftClosedRotation;
    private Quaternion rightClosedRotation;
    private Quaternion leftOpenRotation;
    private Quaternion rightOpenRotation;

    private bool isOpen;
    private PickupItem storedEgg;

    private void Awake()
    {
        leftClosedRotation = leftDoorPivot.localRotation;
        rightClosedRotation = rightDoorPivot.localRotation;

        leftOpenRotation =
            leftClosedRotation * Quaternion.Euler(0f, leftOpenAngle, 0f);

        rightOpenRotation =
            rightClosedRotation * Quaternion.Euler(0f, rightOpenAngle, 0f);
    }

    private void Update()
    {
        Quaternion targetLeft =
            isOpen ? leftOpenRotation : leftClosedRotation;

        Quaternion targetRight =
            isOpen ? rightOpenRotation : rightClosedRotation;

        leftDoorPivot.localRotation = Quaternion.RotateTowards(
            leftDoorPivot.localRotation,
            targetLeft,
            rotationSpeed * Time.deltaTime
        );

        rightDoorPivot.localRotation = Quaternion.RotateTowards(
            rightDoorPivot.localRotation,
            targetRight,
            rotationSpeed * Time.deltaTime
        );
    }

    public void Interact()
    {
        PlayerPickup playerPickup = FindFirstObjectByType<PlayerPickup>();

        if (playerPickup == null)
        {
            Debug.LogWarning("No se encontró PlayerPickup.");
            return;
        }

        // Si está cerrada, primero se abre.
        if (!isOpen)
        {
            isOpen = true;
            return;
        }

        // Si el jugador sostiene algo, intenta guardarlo.
        if (playerPickup.HasItem())
        {
            TryStoreEgg(playerPickup);
            return;
        }

        // Si tiene las manos vacías y hay un huevo, lo recoge.
        if (storedEgg != null)
        {
            TakeStoredEgg(playerPickup);
            return;
        }

        // Si está abierta, vacía y el jugador no sostiene nada, se cierra.
        isOpen = false;
    }

    private void TryStoreEgg(PlayerPickup playerPickup)
    {
        if (storedEgg != null)
        {
            Debug.Log("Ya hay un huevo dentro de la refrigeradora.");
            return;
        }

        PickupItem heldItem = playerPickup.GetHeldItem();

        if (heldItem == null)
            return;

        if (!heldItem.TryGetComponent(out EggItem eggItem))
        {
            Debug.Log("El objeto sostenido no es un huevo.");
            return;
        }

        PickupItem removedItem = playerPickup.RemoveHeldItem();

        if (removedItem == null)
            return;

        storedEgg = removedItem;

        storedEgg.transform.SetParent(eggPlacementPoint);
        storedEgg.transform.localPosition = Vector3.zero;
        storedEgg.transform.localRotation = Quaternion.identity;

        Rigidbody rb = storedEgg.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = storedEgg.GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = false;
        }

        Debug.Log("Huevo colocado dentro de la refrigeradora.");
    }


    private void TakeStoredEgg(PlayerPickup playerPickup)
    {
        if (storedEgg == null)
            return;

        PickupItem eggToTake = storedEgg;
        storedEgg = null;

        playerPickup.PickUp(eggToTake);

        Debug.Log("Huevo recogido de la refrigeradora.");
    }
}