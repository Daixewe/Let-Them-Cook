using TMPro;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [Header("Interacción")]
    [SerializeField] private Transform interactorSource;
    [SerializeField] private float interactorRange = 3f;

    [Header("Interfaz")]
    [SerializeField] private GameObject interactionMessage;
    [SerializeField] private TMP_Text interactionText;

    private IInteractable currentInteractable;

    private void Start()
    {
        HideInteractionMessage();
    }

    private void Update()
    {
        DetectInteractable();

        if (currentInteractable != null &&
            Input.GetMouseButtonDown(0))
        {
            currentInteractable.Interact();
        }
    }

    private void DetectInteractable()
    {
        currentInteractable = null;

        Ray ray = new Ray(
            interactorSource.position,
            interactorSource.forward
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactorRange))
        {
            currentInteractable =
                hit.collider.GetComponentInParent<IInteractable>();
        }

        if (currentInteractable != null)
        {
            ShowInteractionMessage();
        }
        else
        {
            HideInteractionMessage();
        }
    }

    private void ShowInteractionMessage()
    {
        if (interactionMessage != null)
        {
            interactionMessage.SetActive(true);
        }

        if (interactionText != null)
        {
            interactionText.text = "Click para interactuar";
        }
    }

    private void HideInteractionMessage()
    {
        if (interactionMessage != null)
        {
            interactionMessage.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (interactorSource == null)
            return;

        Gizmos.DrawRay(
            interactorSource.position,
            interactorSource.forward * interactorRange
        );
    }
}