using UnityEngine;

public class ChickenCoopInteraction : MonoBehaviour, IInteractable
{
    [Header("Huevo")]
    [SerializeField] private PickupItem eggPrefab;

    [Header("Opcional")]
    [SerializeField] private float productionCooldown = 3f;

    private float nextEggTime;

    public void Interact()
    {
        if (Time.time < nextEggTime)
            return;

        PlayerPickup playerPickup = FindFirstObjectByType<PlayerPickup>();

        if (playerPickup == null)
        {
            Debug.LogWarning("No se encontró PlayerPickup.");
            return;
        }

        if (playerPickup.HasItem())
        {
            Debug.Log("El jugador ya tiene un objeto en la mano.");
            return;
        }

        PickupItem newEgg = Instantiate(eggPrefab);

        playerPickup.PickUp(newEgg);

        nextEggTime = Time.time + productionCooldown;
    }
}