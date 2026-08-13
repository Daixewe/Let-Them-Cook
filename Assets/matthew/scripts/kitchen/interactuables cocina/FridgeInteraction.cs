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

    [Header("Interfaz")]
    [SerializeField] private FridgeUI fridgeUI;

    [Header("Audio")]
    [SerializeField] private AudioSource fridgeAudioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    private Quaternion leftClosedRotation;
    private Quaternion rightClosedRotation;

    private Quaternion leftOpenRotation;
    private Quaternion rightOpenRotation;

    private bool isOpen;

    private void Awake()
    {
        if (leftDoorPivot != null)
        {
            leftClosedRotation =leftDoorPivot.localRotation;

            leftOpenRotation =leftClosedRotation *Quaternion.Euler(0f,leftOpenAngle,0f);
        }

        if (rightDoorPivot != null)
        {
            rightClosedRotation =rightDoorPivot.localRotation;

            rightOpenRotation =rightClosedRotation *Quaternion.Euler(0f,rightOpenAngle,0f);
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

        // Si la interfaz ya está abierta,
        // ignoramos nuevas interacciones.
        if (fridgeUI.IsOpen)
        {
            return;
        }

        // El jugador debe tener la mano vacía.
        if (playerPickup.HasItem())
        {
            NotificationUI.Instance?.ShowMessage("Debes tener la mano vacía para usar la refrigeradora.");

            return;
        }

        // Primera interacción:
        // abre las puertas.
        if (!isOpen)
        {
            OpenFridge();
            return;
        }

        // Segunda interacción:
        // abre el almacenamiento.
        OpenStorage();
    }

    public string GetInteractionText()
    {
        if (fridgeUI != null && fridgeUI.IsOpen)
        {
            return "Usando refrigeradora";
        }

        if (!isOpen)
        {
            return "Abrir refrigeradora";
        }

        return "Usar refrigeradora";
    }

    private void OpenFridge()
    {
        if (isOpen)
        {
            return;
        }

        isOpen = true;

        if (fridgeAudioSource != null && openSound != null)
        {
            fridgeAudioSource.PlayOneShot(openSound);
        }
    }

    private void OpenStorage()
    {
        if (fridgeUI == null)
        {
            Debug.LogError("FridgeUI no está asignado en FridgeInteraction.");

            return;
        }

        fridgeUI.Open();
    }

    public void CloseDoors()
    {
        // Evita reproducir el sonido de cierre
        // si ya estaba cerrada.
        if (!isOpen)
        {
            return;
        }

        isOpen = false;

        if (fridgeAudioSource != null && closeSound != null)
        {
            fridgeAudioSource.PlayOneShot(closeSound);
        }
    }

    public void CloseFridge()
    {
        if (fridgeUI != null && fridgeUI.IsOpen)
        {
            fridgeUI.CloseUIOnly();
        }

        CloseDoors();

        Debug.Log("Refrigeradora cerrada.");
    }

    private void RotateDoors()
    {
        if (leftDoorPivot != null)
        {
            Quaternion targetLeft =isOpen? leftOpenRotation: leftClosedRotation;

            leftDoorPivot.localRotation =Quaternion.RotateTowards(leftDoorPivot.localRotation,targetLeft,rotationSpeed *Time.deltaTime);
        }

        if (rightDoorPivot != null)
        {
            Quaternion targetRight =isOpen? rightOpenRotation: rightClosedRotation;

            rightDoorPivot.localRotation =Quaternion.RotateTowards(rightDoorPivot.localRotation,targetRight,rotationSpeed *Time.deltaTime);
        }
    }

    private bool ValidateReferences()
    {
        if (playerPickup == null)
        {
            Debug.LogError("Falta asignar PlayerPickup en FridgeInteraction.");

            return false;
        }

        if (fridgeStorage == null)
        {
            Debug.LogError("Falta asignar FridgeStorage en FridgeInteraction.");

            return false;
        }

        if (fridgeUI == null)
        {
            Debug.LogError("Falta asignar FridgeUI en FridgeInteraction.");

            return false;
        }

        return true;
    }
}