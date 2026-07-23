using System.Collections;
using UnityEngine;

public class ChickenCoop : MonoBehaviour, IInteractable
{
    [Header("Inventario del jugador")]
    [SerializeField] private Inventory playerInventory;

    [Header("Configuración del huevo")]
    [SerializeField] private int eggAmount = 1;

    // Tiempo que tarda el gallinero en producir otro huevo.
    [SerializeField] private float eggProductionTime = 30f;

    [Header("Objeto visual opcional")]
    // Este objeto representa el huevo visible dentro del gallinero.
    // Puede ser un modelo 3D, prefab o cualquier GameObject.
    [SerializeField] private GameObject eggVisual;

    // Indica si actualmente hay un huevo disponible.
    private bool hasEgg = true;

    // Evita iniciar varios temporizadores al mismo tiempo.
    private bool isProducingEgg;

    private void Start()
    {
        // Al comenzar, mostramos u ocultamos el huevo
        // dependiendo del estado inicial.
        UpdateEggVisual();
    }

    public void Interact()
    {
        // Comprobamos que el inventario esté asignado.
        if (playerInventory == null)
        {
            Debug.LogError(
                "Falta asignar el Inventory del jugador en ChickenCoop."
            );

            return;
        }

        // Si todavía no hay huevo, no entregamos nada.
        if (!hasEgg)
        {
            Debug.Log(
                "El gallinero todavía está produciendo otro huevo."
            );

            return;
        }

        CollectEgg();
    }

    private void CollectEgg()
    {
        // Agregamos el huevo directamente al inventario.
        playerInventory.AñadirIngrediente(
            Ingredientes.huevo,
            eggAmount
        );

        // El huevo deja de estar disponible.
        hasEgg = false;

        // Ocultamos solamente el modelo del huevo.
        // El gallinero completo permanece en la escena.
        UpdateEggVisual();

        Debug.Log(
            $"Recogiste {eggAmount} huevo. " +
            $"El próximo estará listo en {eggProductionTime} segundos."
        );

        // Iniciamos la producción del siguiente huevo.
        if (!isProducingEgg)
        {
            StartCoroutine(ProduceEgg());
        }
    }

    private IEnumerator ProduceEgg()
    {
        // Marcamos que el gallinero está produciendo.
        isProducingEgg = true;

        // Esperamos el tiempo configurado en el Inspector.
        yield return new WaitForSeconds(eggProductionTime);

        // El nuevo huevo ya está disponible.
        hasEgg = true;
        isProducingEgg = false;

        // Volvemos a mostrar el huevo visual.
        UpdateEggVisual();

        Debug.Log(
            "El gallinero produjo un nuevo huevo."
        );
    }

    private void UpdateEggVisual()
    {
        // El objeto visual es opcional.
        if (eggVisual != null)
        {
            eggVisual.SetActive(hasEgg);
        }
    }
}